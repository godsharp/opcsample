using GodSharp.Opc.Ua;
using GodSharp.Opc.Ua.Client;
using GodSharp.Opc.Ua.Client.Extensions;

using Opc.Ua;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GodSharpOpcUaClientSample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello GodSharp OpcUa Client Sample!");

            var url = "opc.tcp://127.0.0.1:53530/OPCUA/SimulationServer";
            url = "opc.tcp://127.0.0.1:48020/";
            url = "opc.tcp://127.0.0.1:49320/";
            url = "opc.tcp://192.168.1.11:4840";
            url = "opc.tcp://10.0.0.20:4840";
            url = "opc.tcp://192.168.250.1:4840";

            #region # - Discovery

            var _discovery = false;
            if (_discovery)
            {
                OpcUaServerDiscovery discovery = new OpcUaServerDiscovery();

                Console.WriteLine("discovery Discovery");
                var servers = discovery.Discovery(url);
                foreach (var item in servers)
                {
                    foreach (var durl in item.DiscoveryUrls)
                    {
                        Console.WriteLine($"{durl}");
                        var endpoints = discovery.GetEndpoints(durl);
                        if (endpoints == null) continue;
                        foreach (var endpoint in endpoints)
                        {
                            Console.WriteLine($"\t- {endpoint.EndpointUrl}/{endpoint.SecurityMode}/{endpoint.SecurityPolicyUri}");
                        }
                    }
                }

                Console.WriteLine("discovery DiscoveryUrls");
                var discoverys = discovery.DiscoveryUrls(url);
                if (discoverys != null)
                {
                    foreach (var discoveryUrl in discoverys)
                    {
                        Console.WriteLine($"{discoveryUrl}");
                        var endpoints = discovery.GetEndpoints(discoveryUrl);
                        if (endpoints == null) continue;
                        foreach (var endpoint in endpoints)
                        {
                            Console.WriteLine($"\t- {endpoint.EndpointUrl}/{endpoint.SecurityMode}/{endpoint.SecurityPolicyUri}");
                        }
                    }
                }

                Console.WriteLine("discovery finished");
                Console.ReadLine();
            }

            #endregion # - Discovery

            #region 0 - Initial

            var builder = new OpcUaClientBuilder()
                .WithEndpoint(url)
                .WithAnonymous()
                .WithSecurity(MessageSecurityMode.None, SecurityPolicies.None)
                //.WithAccount("root","secret")
                //.WithAccount("root","secret12345678")
                //.WithCertificate(cert2)
                //.WithSecurity(MessageSecurityMode.SignAndEncrypt,SecurityPolicies.Basic256Sha256)
                .WithClientId("GodSharpOpcUaClientConsole");

            var slim = new ManualResetEventSlim(false);
            var connectToServerWhenBuild = false;
            var client = await builder.BuildAsync(connectToServerWhenBuild);

            client.OnSessionOpenFailureTryAgainHandle += (s, e) =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] OnSessionOpenFailureTryAgainHandle:{s}:{e.Message}");
            };
            client.OnSessionConnectNotification += (s, t) =>
            {
                switch (t)
                {
                    case SessionConnectionState.Connected:
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:connected");
                        slim.Set();
                        break;

                    case SessionConnectionState.Reconnecting:
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:reconnecting");
                        break;

                    case SessionConnectionState.Disconnecting:
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:disconnecting");
                        break;

                    case SessionConnectionState.Disconnected:
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:disconnected");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(t), t, null);
                }
            };
            client.OnSessionKeepAlive += (s, e) =>
            {
                //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:{e.CurrentState}");
            };
            client.OnSessionSubscriptionChanged += subscription =>
            {
                foreach (var item in subscription.Notifications)
                {
                    //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{item..SessionName}:{e.CurrentState}");
                }
            };
            client.OnMonitoredItemNotification += (n, i, e) =>
            {
                foreach (var value in i.DequeueValues())
                {
                    Console.WriteLine("{0}->{1} : {2}, {3}, {4}", n, i.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
                }
            };
            client.OnSubscribePollingChanged += (e) =>
            {
                foreach (var value in e.Values)
                {
                    Console.WriteLine("{0}->{1} : {2}", e.Name, value.Node.ToString(), value.Value);
                }
            };

            #endregion 0 - Initial

            #region 1 - StartAsync

            Console.WriteLine("1 - StartAsync ...");
            try
            {
                var connected = connectToServerWhenBuild ? client.Connected : client.Open();
                Console.WriteLine($"connect {connected}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("connect failed,waiting for connect");
                slim.Wait();
            }

            //Console.WriteLine("1.1 - ComplexTypeSystem Load ...");
            //var complexTypeSystem = new ComplexTypeSystem(client.Session);
            //await complexTypeSystem.Load().ConfigureAwait(false);
            //Console.WriteLine("1.1 - ComplexTypeSystem Load");

            #endregion 1 - StartAsync

            #region 2 - Browse / BrowseTree

            var _browse = false;
            if (_browse)
            {
                Console.WriteLine("2 - Press any key to test Browse...");
                Console.ReadLine();

                var all = client.Browse();
                //var browse = client.Session.Browse(new NodeId("ns=4;s=Demo.Static"));
                Console.WriteLine(" DisplayName, BrowseName, NodeClass");
                foreach (var obj in all)
                {
                    Console.WriteLine(" {0}, {1}, {2}", obj.DisplayName, obj.BrowseName, obj.NodeClass);

                    var browse2 = client.Browse((NodeId)obj.NodeId);
                    foreach (var refd in browse2)
                    {
                        Console.WriteLine("   + {0}, {1}, {2}", refd.DisplayName, refd.BrowseName, refd.NodeClass);
                    }
                }

                var tree = client.BrowseTree("ns=5;i=1");
                //var tree = client.BrowseTree(new NodeId("ns=4;s=Demo.Static"));
                Browse(tree);

                static void Browse(IEnumerable<ReferenceBrowseDescription> refs, int level = -1)
                {
                    level++;
                    foreach (var description in refs)
                    {
                        Console.WriteLine("{0}+{1}, {2},{3}",
                            new string('\t', level),
                            //Formatter.FormatAttributeValue(attribute.ValueId.AttributeId, attribute.Value)}
                            //description.Node.BrowseName,
                            description.GetFormatText(),
                            description.Node.NodeClass,
                            description.Node.NodeId
                        );
                        if (description.Children != null)
                        {
                            Browse(description.Children, level);
                        }
                    }
                }
            }

            #endregion 2 - Browse / BrowseTree

            #region 3 - GetAttributes / GetProperties

            var _attributes = false;
            if (_attributes)
            {
                Console.WriteLine("3 - Press any key to test GetAttributes / GetProperties ...");
                Console.ReadLine();
                var node = new NodeId("ns=5;i=1");
                var attributes = client.GetAttributes(node);
                foreach (var attribute in attributes)
                {
                    Console.WriteLine($"{attribute.Name}:{attribute.ValueText}");
                }

                var properties = client.GetProperties(node);
                if (properties != null)
                {
                    foreach (var attribute in properties)
                    {
                        Console.WriteLine($"{attribute.Name}:{GodSharp.Opc.Ua.Client.OpcUaHelper.FormatAttributeValue(attribute.ValueId.AttributeId, attribute.Value, client.Session)}");
                    }
                }
            }

            #endregion 3 - GetAttributes / GetProperties

            #region 4 - Subscribe / Unsubscribe

            var _subscribe = false;
            if (_subscribe)
            {
                Console.WriteLine("Press any key to Subscribe");
                Console.ReadLine();

                var sub_name = "andon";
                var subscribes = new string[] { "ns=0;i=2258", "ns=0;i=2259" };
                // 2258 Server.ServerStatus.CurrentTime
                // 2259 Server.ServerStatus.State

                client.Subscribe(sub_name, subscribes);

                Console.WriteLine("Press any key to Unsubscribe");
                Console.ReadLine();

                client.Unsubscribe(sub_name);
            }

            #endregion 4 - Subscribe / Unsubscribe

            #region 4.1 - Subscribe Event / Unsubscribe Event

            var _event = false;
            if (_event)
            {
                Console.WriteLine("Press any key to Subscribe Event");
                Console.ReadLine();

                var sub_name = "andon";
                var subscribes = new string[] { "ns=0;i=2258", "ns=0;i=2259" };
                // 2258 Server.ServerStatus.CurrentTime
                // 2259 Server.ServerStatus.State

                client.Subscribe(sub_name, subscribes);

                Console.WriteLine("Press any key to Unsubscribe Event");
                Console.ReadLine();

                client.Unsubscribe(sub_name);
            }

            #endregion 4.1 - Subscribe Event / Unsubscribe Event

            Console.WriteLine("5 - Press any key to test read/write ...");

            Console.WriteLine("6 - Press any key to close session...");
            Console.ReadLine();

            var ret = client.Close();
            Console.WriteLine($"Exit {ret}");
        }
    }
}