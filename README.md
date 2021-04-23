# GodSharp.Opc

![Azure DevOps builds (main)](https://img.shields.io/azure-devops/build/godsharp/public/4/main?label=azure%20pipelines&style=flat-square)

## Package Version

|Name|NuGet|MyGet|
|---|---|---|
| GodSharp.Opc.Da | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Da?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Da) | [![MyGet](https://img.shields.io/myget/godsharplab/v/GodSharp.Opc.Da?style=flat-square&label=myget)](https://www.myget.org/feed/godsharplab/package/nuget/GodSharp.Opc.Da) |
| GodSharp.Opc.Da.OpcAutomation | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Da?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Da.OpcAutomation) | [![MyGet](https://img.shields.io/myget/godsharplab/v/GodSharp.Opc.Da.OpcAutomation?style=flat-square&label=myget)](https://www.myget.org/feed/godsharplab/package/nuget/GodSharp.Opc.Da.OpcAutomation) |
| GodSharp.Opc.DaOpcNetApi | [![NuGet](https://img.shields.io/nuget/v/GodSharp.Opc.Da?style=flat-square)](https://www.nuget.org/packages/GodSharp.Opc.Da.DaOpcNetApi) | [![MyGet](https://img.shields.io/myget/godsharplab/v/GodSharp.Opc.Da?style=flat-square&label=myget)](https://www.myget.org/feed/godsharplab/package/nuget/GodSharp.Opc.Da.DaOpcNetApi) |

## Introduction

  `OpcDaClient` has two implementations, which one to choose depends on your own preferences.

  >- **OpcAutomation** : `GodSharp.Opc.Da.OpcAutomation`
  >- **OpcNetApi** : `GodSharp.Opc.DaOpcNetApi`

## Getting Started

  This tutorial uses `GodSharp.Opc.Da.OpcAutomation` library, you should install this package first.

  ```
  PM> Install-Package GodSharp.OpcDa.OpcAutomation
  ```

### Initial : Create opc client
  
  Sample Code :
  ```
  // initial with data info
  // The group `Name`, `ClientHandle` is unique and required, `UpdateRate` is required too.
  // The tag `ItemName`, `ClientHandle` is unique and required.
  var groups = new List<GroupData>
  {
      new GroupData
      {
          Name = "default", UpdateRate = 100, ClientHandle = 010,
          Tags = new List<Tag>
          {
              new Tag("Channel1.Device1.BoolArray", 001),
              new Tag("Channel1.Device1.Heartbeat", 010),
              new Tag("Channel1.Device1.RunMode", 011)
          }
      },
      new GroupData
      {
          Name = "group1", UpdateRate = 100, ClientHandle = 100,
          Tags = new List<Tag>
          {
              new Tag("Channel1.Device1.Group1.EngineCode", 100),
              new Tag("Channel1.Device1.Group1.RackNumber", 101)
          }
      },
      new GroupData
      {
          Name = "group2", UpdateRate = 100, ClientHandle = 200,
          Tags = new List<Tag>
          {
              new Tag("Channel1.Device1.Group2.EngineCode", 200),
              new Tag("Channel1.Device1.Group2.RackNumber", 201)
          }
      }
  };

  var server = new ServerData
  {
      Host = "127.0.0.1",
      ProgId = "Kepware.KEPServerEX.V6",
      // initial with data info,after connect will be add to client
      // if this is null,you should add group and tag manually
      Groups = groups
  };
  
  var client = DaClientFactory.Instance.CreateOpcAutomationClient(new DaClientOptions(
          server, 
          OnDataChangedHandler, 
          OnShoutdownHandler,
          OnAsyncReadCompletedHandler, 
          OnAsyncWriteCompletedHandler));
  ```

  >- The group `Name`, `ClientHandle` is unique and required, `UpdateRate` is required too.
  >- The tag `ItemName`, `ClientHandle` is unique and required.

### Connect to Opc Server

  ```
  var connected = client.Connect();
  ```

### Add Group to Client

  ```c#
  client.Add(new Group() {Name = "default", UpdateRate = 100, ClientHandle = 010});
  client.Add(new Group() {Name = "group1", UpdateRate = 100, ClientHandle = 100});
  client.Add(new Group() {Name = "group2", UpdateRate = 100, ClientHandle = 200});
  ```

  > **Important!** 
  > *The group `Name`, `ClientHandle` is unique and required, `UpdateRate` is required too.*

### Add Tag to Group

  ```c#
  // add one by one
  client.Groups["group1"].Add(new Tag("Channel1.Device1.Group1.Tag1", 100));
    client.Groups["group1"].Add(new Tag("Channel1.Device1.Group1.Tag2", 101));
    
  // add multi one time
  client.Groups["group1"].Add(
      new Tag("Channel1.Device1.Group1.Code", 100),
      new Tag("Channel1.Device1.Group1.Number", 101));
  ```

  > **Important!**
  > *The tag `ItemName`, `ClientHandle` is unique and required.*

### Event Handles

  ```c#
  public static void OnDataChangedHandler(DataChangedOutput output)
  {
      Console.WriteLine($"{output.Data.ItemName}:{output.Data.Value},{output.Data.Quality} / {output.Data.Timestamp}");
  }
  
  public static void OnAsyncReadCompletedHandler(AsyncReadCompletedOutput output)
  {
      Console.WriteLine(
          $"Async Read {output.Data.Result.ItemName}:{output.Data.Result.Value},{output.Data.Result.Quality} / {output.Data.Result.Timestamp} / {output.Data.Code}");
  }
  
  public static void OnAsyncWriteCompletedHandler(AsyncWriteCompletedOutput output)
  {
      Console.WriteLine($"Async Write {output.Data.Result.ItemName}:{output.Data.Code}");
  }
  
  public static void OnShoutdownHandler(Server server, string reason)
  {
  }
  ```

### Read/Write Tag

  See [sample code](./src/GodSharpOpcDaSample/Program.cs).

### Disconnect to Opc Server

  ```
  client.Disconnect();
  ```

## License

  **Free!**