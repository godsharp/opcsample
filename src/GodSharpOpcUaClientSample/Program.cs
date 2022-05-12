using GodSharp.Extensions.Opc.Ua.Client;
using GodSharp.Extensions.Opc.Ua.Types;
using GodSharp.Extensions.Opc.Ua.Types.Encodings;
using GodSharp.Extensions.Opc.Ua.Utilities;
using GodSharp.Opc.Ua;

using Opc.Ua;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GodSharpOpcUaClientSample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello GodSharp OpcUa Client Sample!");
            // // register type namespace
            EncodingFactory.Instance.RegisterTypeNamespace(
                new TypeNamespace()
                {
                    Type = typeof(UaAnsiVector).AssemblyQualifiedName,
                    TypeId = "nsu=http://www.unifiedautomation.com/DemoServer/;i=3002",
                    BinaryEncodingId = "nsu=http://www.unifiedautomation.com/DemoServer/;i=5054"
                }
                //, new TypeNamespace()
                //{
                //    Type = $"{typeof(ProsysVector).Namespace}.{typeof(ProsysVector).Name}",
                //    TypeId = "nsu=http://opcfoundation.org/UA/;i=18808",
                //    BinaryEncodingId = "nsu=http://opcfoundation.org/UA/;i=18817"
                //}
            );
            EncodingFactory.Instance.RegisterEncodeableTypes(typeof(UaAnsiVector), typeof(UaAnsiVector));
            EncodingFactory.Instance.RegisterEncodeableTypes(Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly());
            //EncodeableFactory.GlobalFactory.AddEncodeableTypes(Assembly.GetEntryAssembly());
            EncodeableFactory.GlobalFactory.AddEncodeableType(typeof(UaAnsiVector));
            //EncodeableFactory.GlobalFactory.AddEncodeableType(typeof(ProsysVector));

            var url = "opc.tcp://127.0.0.1:53530/OPCUA/SimulationServer";
            url = "opc.tcp://127.0.0.1:48020/";
            url = "opc.tcp://127.0.0.1:49320/";
            //url = "opc.tcp://10.0.0.20:4840";
            //url = "opc.tcp://192.168.250.1:4840";

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

            OpcUaClientBuider buider = new OpcUaClientBuider();

            //var cert = new X509Certificate2(@"F:\Temp\.opc\cert\godsharpopcuacert.der", "123456", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);

            //var cert2 = CertificateFactory.CreateCertificateWithPEMPrivateKey(cert, File.ReadAllBytes(@"F:\Temp\.opc\cert\godsharpopcuacert_key.pem"));

            buider
                .WithEndpoint(url)
                .WithAnonymous()
                //.WithAccount("root","secret")
                //.WithAccount("root","secret12345678")
                //.WithCertificate(cert2)
                //.WithSecurity(MessageSecurityMode.SignAndEncrypt,SecurityPolicies.Basic256Sha256)
                .WithClientId("GodSharpOpcUaClientConsole");

            var client = buider.Build();
            {
                client.OnSessionConnectNotification = (s, t) =>
                {
                    switch (t)
                    {
                        case ClientSessionConnectionState.Connected:
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:connected");
                            break;

                        case ClientSessionConnectionState.Reconnecting:
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:reconnecting");
                            break;

                        case ClientSessionConnectionState.Disconnecting:
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:disconnecting");
                            break;

                        case ClientSessionConnectionState.Disconnected:
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:disconnected");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(t), t, null);
                    }
                };
                client.OnSessionKeepAlive = (s, e) =>
                {
                    //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{s.SessionName}:{e.CurrentState}");
                };
                client.OnSessionSubscriptionChanged = subscription =>
                {
                    foreach (var item in subscription.Notifications)
                    {
                        //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]{item..SessionName}:{e.CurrentState}");
                    }
                };
                client.OnMonitoredItemNotification = (n, i, e) =>
                {
                    foreach (var value in i.DequeueValues())
                    {
                        Console.WriteLine("{0}->{1} : {2}, {3}, {4}", n, i.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
                    }
                };
            }

            #endregion 0 - Initial

            #region 1 - StartAsync

            Console.WriteLine("1 - StartAsync ...");
            bool ret = await client.StartAsync();
            Console.WriteLine($"connect {ret}");

            #endregion 1 - StartAsync

            #region 2 - Browse / BrowseTree

            var _browse = true;
            if (_browse)
            {
                Console.WriteLine("2 - Press any key to test Browse...");
                Console.ReadLine();

                var all = client.Session.Browse();
                //var browse = client.Session.Browse(new NodeId("ns=4;s=Demo.Static"));
                Console.WriteLine(" DisplayName, BrowseName, NodeClass");
                foreach (var obj in all)
                {
                    Console.WriteLine(" {0}, {1}, {2}", obj.DisplayName, obj.BrowseName, obj.NodeClass);

                    var browse2 = client.Session.Browse((NodeId)obj.NodeId);
                    foreach (var refd in browse2)
                    {
                        Console.WriteLine("   + {0}, {1}, {2}", refd.DisplayName, refd.BrowseName, refd.NodeClass);
                    }
                }

                // if node is too many,you should set `depth` parameter with appropriate value.
                var tree = client.Session.BrowseTree(depth: 4);
                //var tree = client.Session.BrowseTree(new NodeId("ns=4;s=Demo.Static"));
                Browse(tree);

                static void Browse(IEnumerable<ReferenceBrowseDescription> refs, int level = -1)
                {
                    level++;
                    foreach (var description in refs)
                    {
                        Console.WriteLine("{0}{4}+{1}, {2},{3}",
                            new string('\t', level),
                            //Formatter.FormatAttributeValue(attribute.ValueId.AttributeId, attribute.Value)}
                            //description.Node.BrowseName,
                            description.GetFormatText(),
                            description.Node.NodeClass,
                            description.Node.NodeId,
                            level
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
                var node = new NodeId("ns=0;i=2258"); // 2258 Server.ServerStatus.CurrentTime
                var attributes = client.Session.GetAttributes(node);
                foreach (var attribute in attributes)
                {
                    Console.WriteLine($"{attribute.Name}:{attribute.ValueText}");
                }

                var properties = client.Session.GetProperties(node);
                if (properties != null)
                {
                    foreach (var attribute in properties)
                    {
                        Console.WriteLine($"{attribute.Name}:{Formatter.FormatAttributeValue(attribute.ValueId.AttributeId, attribute.Value, client.Session)}");
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

            Console.WriteLine("5 - Press any key to test read/write ...");
            Console.ReadLine();
            string input = null;
            do
            {
                try
                {
                    //new OmronRunner(client).Run();
                    //new ProsysOpcUaSimulatorRunner(client).Run();
                    new UaAnsiCServerRunner(client).Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine("5 - Press q key exit ...");
                input = Console.ReadLine();
            } while (input != "q");

            Console.WriteLine("6 - Press any key to close session...");
            Console.ReadLine();

            ret = await client.StopAsync();
            Console.WriteLine($"Exit {ret}");
        }
    }
}