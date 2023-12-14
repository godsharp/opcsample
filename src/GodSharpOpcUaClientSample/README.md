# GodSharp.Opc.Ua

[![continuous](https://github.com/godsharp/opcua/actions/workflows/continuous.yml/badge.svg?branch=main)](https://github.com/godsharp/opcua/actions/workflows/continuous.yml)
[![build](https://github.com/godsharp/opcua/actions/workflows/continuous.build.yml/badge.svg)](https://github.com/godsharp/opcua/actions/workflows/continuous.build.yml)

## Package Version

|Name|NuGet|MyGet|
|---|---|---|
| GodSharp.Opc.Ua.Common | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Ua.Common?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Ua.Common) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Opc.Ua.Common?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Opc.Ua.Common) |
| GodSharp.Opc.Ua.Client | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Ua.Client?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Ua.Client) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Opc.Ua.Client?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Opc.Ua.Client) |
| GodSharp.Opc.Ua.ProxyServer | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Ua.ProxyServer?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Ua.ProxyServer) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Opc.Ua.ProxyServer?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Opc.Ua.ProxyServer) |
| GodSharp.Opc.Ua.Server | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Ua.Server?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Ua.Server) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Opc.Ua.Server?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Opc.Ua.Server) |
| GodSharp.Opc.Ua.Server.AspNetCore | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Ua.Server.AspNetCore?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Ua.Server.AspNetCore) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Opc.Ua.Server.AspNetCore?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Opc.Ua.Server.AspNetCore) |
| GodSharp.Opc.Ua.ComplexTypes | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Ua.ComplexTypes?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Ua.ComplexTypes) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Opc.Ua.ComplexTypes?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Opc.Ua.ComplexTypes) |

## Introduction

|Package|Uasge|
|---:|:---|
| GodSharp.Opc.Ua.Common | a opc ua common lib |
| GodSharp.Opc.Ua.Client | a opc ua client |
| GodSharp.Opc.Ua.ProxyServer | a proxy for opc ua server |
| GodSharp.Opc.Ua.Server | a opc ua server |
| GodSharp.Opc.Ua.Server.AspNetCore | opc ua server for asp.net core |
| GodSharp.Opc.Ua.ComplexTypes | custom generate encode/decode method for `EncodeableObject` class |

## Getting Started

  ```ps
  PM> Install-Package GodSharp.Opc.Ua.Client
  ```

### Discovery Server

  Sample code:

  ```c#
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
  ```

### Initial : Create opc ua client
  
  Sample code:

  ```c#
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
  ```

### Connect to Opc Server

  Sample code:

  ```c#
  Console.WriteLine("1 - StartAsync ...");
  var connected = connectToServerWhenBuild ? client.Connected : client.Open();
  Console.WriteLine($"connect {reconnectedt}");
  ```

### Browse / BrowseTree

- **Browse**: To browse node list with specialized node or default.
- **BrowseTree**: To browse node tree with specialized node or default.

  Sample code:

  ```c#
  // To browse node list with specialized node or default 
  var all = client.Browse();
  //var browse = client.Session.Browse(new NodeId  ("ns=4;s=Demo.Static"));
  Console.WriteLine(" DisplayName, BrowseName, NodeClass");
  foreach (var obj in all)
  {
      Console.WriteLine(" {0}, {1}, {2}", obj.DisplayName,   obj.BrowseName, obj.NodeClass);
  
      var browse2 = client.Browse((NodeId)obj.NodeId);
      foreach (var refd in browse2)
      {
          Console.WriteLine("   + {0}, {1}, {2}",   refd.DisplayName, refd.BrowseName, refd.NodeClass);
      }
  }
  
  // To browse node tree with specialized node or default
  var tree = client.BrowseTree();
  //var tree = client.BrowseTree(new NodeId("ns=4;s=Demo.Static"));
  Browse(tree);
  
  static void Browse(IEnumerable<ReferenceBrowseDescription> refs, int level = -1)
  {
      level++;
      foreach (var description in refs)
      {
          Console.WriteLine("{0}+{1}, {2},{3}",
              new string('\t', level),
              //Formatter.FormatAttributeValue  (attribute.ValueId.AttributeId,   attribute.Value)}
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
  ```

### GetAttributes / GetProperties

  Sample code:

  ```c#
  var node = new NodeId("ns=0;i=2258"); // 2258   Server.ServerStatus.CurrentTime
  var attributes = client.GetAttributes(node);
  foreach (var attribute in attributes)
  {
      Console.WriteLine($"{attribute.Name}:  {attribute.ValueText}");
  }
  
  var properties = client.GetProperties(node);
  if (properties != null)
  {
      foreach (var attribute in properties)
      {
          Console.WriteLine($"{attribute.Name}:{GodSharp.Opc.Ua.Client.OpcUaHelper.FormatAttributeValue(attribute.ValueId.AttributeId, attribute.Value, client.Session)}");
      }
  }
  ```

### Subscribe / Unsubscribe

  Sample code:

  ```c#
  var sub_name = "andon";
  var subscribes = new string[] { "ns=0;i=2258",   "ns=0;i=2259" };
  // 2258 Server.ServerStatus.CurrentTime
  // 2259 Server.ServerStatus.State
  
  client.Subscribe(sub_name, subscribes);
  
  Console.WriteLine("Press any key to Unsubscribe");
  Console.ReadLine();
  
  client.Unsubscribe(sub_name);
  ```

### Read Node

  Sample code:

  ```c#
  T val = client.Read<T>(node);
  // or
  DataValue val = client.Read(node);
  ```

### Write Node

  Sample code:

  ```c#
  var ret = client.Write(node, value);
  ```

### Disconnect to Opc Server

  Sample code:

  ```
  var ret = client.Close();
  ```

## License

  **Free!**