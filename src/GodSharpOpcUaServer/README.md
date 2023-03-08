# GodSharp.Opc.Ua.Server

![Azure DevOps builds (main)](https://img.shields.io/azure-devops/build/godsharp/public/4/main?label=azure%20pipelines&style=flat-square)

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
  PM> Install-Package GodSharp.Opc.Ua.Server
  ```
  
### Configuration
  
  ```csharp 
  // Auto Generate ApplicationConfigurationFile
  GodSharp.Opc.Ua.OpcUaHelper.GenerateApplicationConfigurationFile =  true;
  
  // Configuration for UA Server
  ApplicationConfiguration? configuration = null;
  configuration = await   GodSharp.Opc.Ua.OpcUaHelper.ServerConfigurationAsync(
      "GodSharpOpcUaServer",
      "GodSharp Opc Ua Server",
      new string[] { Utils.Format(@"opc.tcp://{0}:4840", "localhost") }
      //,(s, e) => CertificateValidationHandler(configuration, s, e)
      , CertificateValidationHandler
      );
  
  // Instance UA Server
  var server = new OpcUaStandardServer(new  SampleServerAuthorizationFilter ());
  
  // Start server
  server.Start(configuration);
  ```

### Instance
  
  ```csharp 
  // Instance UA Server
  var server = new OpcUaStandardServer(new  SampleServerAuthorizationFilter ());
  
  // Start server
  server.Start(configuration);
  
  // Stop server
  server.Stop();
  ```

### ServerAuthorizationFilter

  Used to authorized user token.

  ```csharp
  public class SampleServerAuthorizationFilter :   IServerAuthorizationFilter
  {
      public ServerAuthorizationResult Verify(UserNameIdentityToken  token)
      {
          string userName = token.UserName;
          string decryptedPassword = token.DecryptedPassword;
          if (userName == "admin" && decryptedPassword ==   "admin@password")
          {
              return new ServerAuthorizationResult(Authorized: true,   Administrator: true);
          }
  
          if (userName == "user" && decryptedPassword == "user@password")
          {
              return new ServerAuthorizationResult(Authorized: true);
          }
  
          return new ServerAuthorizationResult(Authorized: false);
      }
  }
  ```

## License

  **Free!**