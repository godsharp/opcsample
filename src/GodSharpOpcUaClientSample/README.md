# GodSharp.Opc.Ua

![Azure DevOps builds (main)](https://img.shields.io/azure-devops/build/godsharp/public/4/main?label=azure%20pipelines&style=flat-square)

## Package Version

|Name|NuGet|MyGet|
|---|---|---|
| GodSharp.Extensions.Opc.Ua | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Extensions.Opc.Ua?style=flat-square)](https://www.nuget.org/packages/GodSharp.Extensions.Opc.Ua) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Extensions.Opc.Ua?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Extensions.Opc.Ua) |
| GodSharp.Extensions.Opc.Ua.Client | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Extensions.Opc.Ua.Client?style=flat-square)](https://www.nuget.org/packages/GodSharp.Extensions.Opc.Ua.Client) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Extensions.Opc.Ua.Client?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Extensions.Opc.Ua.Client) |
| GodSharp.Extensions.Opc.Ua.Generator | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Extensions.Opc.Ua.Generator?style=flat-square)](https://www.nuget.org/packages/GodSharp.Extensions.Opc.Ua.Generator) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Extensions.Opc.Ua.Generator?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Extensions.Opc.Ua.Generator) |
| GodSharp.Extensions.Opc.Ua.CodeGenerator | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Extensions.Opc.Ua.CodeGenerator?style=flat-square)](https://www.nuget.org/packages/GodSharp.Extensions.Opc.Ua.CodeGenerator) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Extensions.Opc.Ua.CodeGenerator?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Extensions.Opc.Ua.CodeGenerator) |
| GodSharp.Extensions.Opc.Ua.MsBuild | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Extensions.Opc.Ua.MsBuild?style=flat-square)](https://www.nuget.org/packages/GodSharp.Extensions.Opc.Ua.MsBuild) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Extensions.Opc.Ua.MsBuild?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Extensions.Opc.Ua.MsBuild) |
| GodSharp.Opc.Ua.Client | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Ua.Client?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Ua.Client) | [![MyGet](https://img.shields.io/myget/godsharp/v/GodSharp.Opc.Ua.Client?style=flat-square&label=myget)](https://www.myget.org/feed/godsharp/package/nuget/GodSharp.Opc.Ua.Client) |

## Introduction

|Package|Uasge|
|---:|:---|
| GodSharp.Extensions.Opc.Ua | extensions library for opc ua |
| GodSharp.Extensions.Opc.Ua.Client | extensions library for opc ua client |
| GodSharp.Extensions.Opc.Ua.Generator | attributes for `CodeGenerator` |
| GodSharp.Extensions.Opc.Ua.CodeGenerator | generate encode/decode method for `EncodeableObject` class by DIY |
| GodSharp.Extensions.Opc.Ua.MsBuild | generate code for which class with `ComplexObjectGenerator` attribute |
| GodSharp.Opc.Ua.Client | a Sample opc ua client |

## Getting Started

  ```ps
  PM> Install-Package GodSharp.Opc.Ua.Client
  ```

### Discovery Server

  Sample code:

  ```c#
  var url = "opc.tcp://127.0.0.1:4840";
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
              Console.WriteLine($"\t- {endpoint.EndpointUrl}/ {endpoint.SecurityMode}/  {endpoint.SecurityPolicyUri}");
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
              Console.WriteLine($"\t- {endpoint.EndpointUrl}/ {endpoint.SecurityMode}/  {endpoint.SecurityPolicyUri}");
          }
      }
  }
  
  Console.WriteLine("discovery finished");
  ```

### Initial : Create opc ua client
  
  Sample code:

  ```c#
  OpcUaClientBuider buider = new OpcUaClientBuider();

  var cert = new X509Certificate2(@".\.opc\cert  \godsharpopcuacert.der", "123456",  X509KeyStorageFlags.MachineKeySet |   X509KeyStorageFlags.Exportable);
  
  var cert2 =   CertificateFactory.CreateCertificateWithPEMPrivateKey(cert, File.ReadAllBytes(@".\.opc\cert  \godsharpopcuacert_key.pem"));
  
  buider
      .WithEndpoint(url)
      .WithAnonymous()
      //.WithAccount("root","secret")
      //.WithAccount("root","secret12345678")
      //.WithCertificate(cert2)
      //.WithSecurity  (MessageSecurityMode.SignAndEncrypt,SecurityPolicies.Ba s ic256Sha256)
      .WithClientId("GodSharpOpcUaClientConsole");
  
  var client = buider.Build();
  {
      client.OnSessionConnectNotification = (s, t) =>
      {
          switch (t)
          {
              case ClientSessionConnectionState.Connected:
                  Console.WriteLine  ($"[{DateTime.Now:HH:mm:ss.fff}]  {s.SessionName}:connected");
                  break;
              case ClientSessionConnectionState.Reconnecting:
                  Console.WriteLine  ($"[{DateTime.Now:HH:mm:ss.fff}]  {s.SessionName}:reconnecting");
                  break;
              case ClientSessionConnectionState.Disconnecting:
                  Console.WriteLine  ($"[{DateTime.Now:HH:mm:ss.fff}]  {s.SessionName}:disconnecting");
                  break;
              case ClientSessionConnectionState.Disconnected:
                  Console.WriteLine  ($"[{DateTime.Now:HH:mm:ss.fff}]  {s.SessionName}:disconnected");
                  break;
              default:
                  throw new ArgumentOutOfRangeException(nameof  (t), t, null);
          }
      };
      client.OnSessionKeepAlive = (s, e) =>
      {
          //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]  {s.SessionName}:{e.CurrentState}");
      };
      client.OnSessionSubscriptionChanged = subscription =>
      {
          foreach (var item in subscription.Notifications)
          {
              //Console.WriteLine  ($"[{DateTime.Now:HH:mm:ss.fff}]  {item..SessionName}:{e.CurrentState}");
          }
      };
      client.OnMonitoredItemNotification = (n, i, e) =>
      {
          foreach (var value in i.DequeueValues())
          {
              Console.WriteLine("{0}->{1} : {2}, {3}, {4}", n,   i.DisplayName, value.Value,   value.SourceTimestamp, value.StatusCode);
          }
      };
  }
  ```

### Connect to Opc Server

  Sample code:

  ```c#
  Console.WriteLine("1 - StartAsync ...");
  bool ret = await client.StartAsync();
  Console.WriteLine($"connect {ret}");
  ```

### Browse / BrowseTree

- **Browse**: To browse node list with specialized node or default.
- **BrowseTree**: To browse node tree with specialized node or default.

  Sample code:

  ```c#
  // To browse node list with specialized node or default 
  var all = client.Session.Browse();
  //var browse = client.Session.Browse(new NodeId  ("ns=4;s=Demo.Static"));
  Console.WriteLine(" DisplayName, BrowseName, NodeClass");
  foreach (var obj in all)
  {
      Console.WriteLine(" {0}, {1}, {2}", obj.DisplayName,   obj.BrowseName, obj.NodeClass);
  
      var browse2 = client.Session.Browse((NodeId)obj.NodeId);
      foreach (var refd in browse2)
      {
          Console.WriteLine("   + {0}, {1}, {2}",   refd.DisplayName, refd.BrowseName, refd.NodeClass);
      }
  }
  
  // To browse node tree with specialized node or default
  var tree = client.Session.BrowseTree();
  //var tree = client.Session.BrowseTree(new NodeId  ("ns=4;s=Demo.Static"));
  Browse(tree);
  
  static void Browse(IEnumerable<ReferenceBrowseDescription>   refs, int level = -1)
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
  var attributes = client.Session.GetAttributes(node);
  foreach (var attribute in attributes)
  {
      Console.WriteLine($"{attribute.Name}:  {attribute.ValueText}");
  }
  
  var properties = client.Session.GetProperties(node);
  if (properties != null)
  {
      foreach (var attribute in properties)
      {
          Console.WriteLine($"{attribute.Name}:  {Formatter.FormatAttributeValue  (attribute.ValueId.AttributeId, attribute.Value,   client.Session)}");
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
  T val = client.Session.Read<T>(node);
  // or
  DataValue val = client.Session.Read(node);
  ```

### Write Node

  Sample code:

  ```c#
  var ret = client.Session.Write(node, value);
  ```

### Disconnect to Opc Server

  Sample code:

  ```
  var ret = await client.StopAsync();
  ```

## MsBuild

Generate code for which class is customized with `ComplexObjectGenerator` attribute, the class generated is implement `ComplexObject`.

  ```ps
  PM> Install-Package GodSharp.Extensions.Opc.Ua.Generator
  PM> Install-Package GodSharp.Extensions.Opc.Ua.MsBuild
  ```

 `ComplexObject` class types:

  ```C#
  [DefaultValue(ComplexObjectType.EncodeableObject)]
  public enum ComplexObjectType
  {
      EncodeableObject,
      SwitchField,
      OptionalField
  }
  ```

  Sample code:

  ```C#
  [ComplexObjectGenerator(ComplexObjectType.SwitchField,   EncodingMethodType.Factory)]
  public partial class UaAnsiUnion
  {
      [SwitchField(1)]
      public int Int32;
      [SwitchField(2, 4)]
      public string String;
  
      public UaAnsiUnion()
      {
          TypeIdNamespace = "nsu=http://  www.unifiedautomation.com/DemoServer/;i=3006";
          BinaryEncodingIdNamespace = "nsu=http://  www.unifiedautomation.com/DemoServer/;i=5003";
      }
  }
  ```

Just build project, code will be generated by msbuild task in the file with extension `{filename}.uamgen.cs` by default.

like this:

  ```C#
  //----------------------------------------------------------- - ------------------
  // <auto-generated>
  //     Generated by MSBuild generator.
  //     Source: UaAnsiUnion.cs
  // </auto-generated>
  //----------------------------------------------------------- - ------------------
  
  using GodSharp.Extensions.Opc.Ua.Types;
  using Opc.Ua;
  using static   GodSharp.Extensions.Opc.Ua.Types.Encodings.EncodingFactory;
  
  namespace GodSharpOpcUaClientSample.Types
  {
  	public partial class UaAnsiUnion : ComplexObject 
  	{
  		public uint SwitchField;
  
  		public override void Encode(IEncoder encoder)
  		{
  			base.Encode(encoder);
  			encoder.WriteUInt32("SwitchField",SwitchField);
  			switch (SwitchField)
  			{
  				case 1:
  					Encoding.Write(encoder, Int32, nameof  (Int32));
  					break;
  				case 2:
  				case 4:
  					Encoding.Write(encoder, String, nameof  (String));
  					break;
  				default:
  					break;
  			}
  		}
  
  		public override void Decode(IDecoder decoder)
  		{
  			base.Decode(decoder);
  			SwitchField = decoder.ReadUInt32("SwitchField");
  			switch (SwitchField)
  			{
  				case 1:
  					Encoding.Read(decoder,ref Int32, nameof  (Int32));
  					break;
  				case 2:
  				case 4:
  					Encoding.Read(decoder,ref String, nameof  (String));
  					break;
  				default:
  					break;
  			}
  		}
  	}
  }
  ```

## Register Custom Types


### Register Custom Type Namespace

You can register custom type namespace by hard code in `constructor`.

  Sample code:

  ```C#
  [ComplexObjectGenerator(ComplexObjectType.SwitchField,   EncodingMethodType.Factory)]
  public partial class UaAnsiUnion
  {
      [SwitchField(1)]
      public int Int32;
      [SwitchField(2, 4)]
      public string String;
  
      public UaAnsiUnion()
      {
          TypeIdNamespace = "nsu=http://  www.unifiedautomation.com/DemoServer/;i=3006";
          BinaryEncodingIdNamespace = "nsu=http://  www.unifiedautomation.com/DemoServer/;i=5003";
      }
  }
  ```

Also can support by configuration from file or db,and others.

  Sample code:

  ```C#
  EncodingFactory.Instance.RegisterTypeNamespace(
  new TypeNamespace()
  {
      Type = typeof(UaAnsiVector).AssemblyQualifiedName,
      TypeId = "nsu=http://www.unifiedautomation.com/  DemoServer/;i=3002",
      BinaryEncodingId = "nsu=http://www.unifiedautomation.com/  DemoServer/;i=5054"
  }
  );
  ```

### Register Custom Type to System

  Sample code:

  ```C#
  EncodingFactory.Instance.RegisterEncodeableTypes(typeof(UaAnsiVector), typeof(UaAnsiVector));
  EncodingFactory.Instance.RegisterEncodeableTypes(Assembly.GetEntryAssembly(),Assembly.GetExecutingAssembly());
  ```

## License

  **Free!**