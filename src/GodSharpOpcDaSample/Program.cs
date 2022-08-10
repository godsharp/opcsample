using GodSharp.Opc.Da;
using GodSharp.Opc.Da.Options;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GodSharpOpcDaSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /*
             * KEPware.KEPServerEx.V4
             * Kepware.KEPServerEX.V6
             * Mitsubishi.MXOPC.7
             */
            Console.WriteLine("Hello GodSharpOpcDaSample!");
            var write = 1000;

            // initial with data info
            // The group `Name`, `ClientHandle` is unique and required, `UpdateRate` is required too.
            // The tag `ItemName`, `ClientHandle` is unique and required.
  var groups = new List<GroupData>
  {
      new GroupData
      {
          Name = "default", UpdateRate = 100, ClientHandle = 010, IsSubscribed = true,
          Tags = new List<Tag>
          {
              new Tag("Test.Simulator.Booleans.B0001", 011),
              new Tag("Test.Simulator.Numbers.N0001", 012),
              new Tag("Test.Simulator.Characters.C0001", 013)
          }
      },
      new GroupData
      {
          Name = "group1", UpdateRate = 100, ClientHandle = 100,IsSubscribed = true,
          Tags = new List<Tag>
          {
              new Tag("Test.Simulator.Booleans.B0002", 101),
              new Tag("Test.Simulator.Numbers.N0002", 102),
              new Tag("Test.Simulator.Characters.C0002", 103)
          }
      },
      new GroupData
      {
          Name = "group2", UpdateRate = 100, ClientHandle = 200,IsSubscribed = false,
          Tags = new List<Tag>
          {
              new Tag("Test.Simulator.Booleans.B0003", 201),
              new Tag("Test.Simulator.Numbers.N0003", 202),
              new Tag("Test.Simulator.Characters.C0003", 203)
          }
      }
  };

            var server = new ServerData
            {
                Host = "127.0.0.1",
                ProgId = "Kepware.KEPServerEX.V6",
                // initial with data info,after connect will be add to client auto
                // if this is null,you should add group and tag manually
                Groups = groups
            };

            var client = DaClientFactory.Instance.CreateOpcNetApiClient(new DaClientOptions(
                server,
                OnDataChangedHandler,
                OnShoutdownHandler,
                OnAsyncReadCompletedHandler,
                OnAsyncWriteCompletedHandler));

            Console.WriteLine("connect to server ...");
            client.Connect();

            /*
             // add group and tag manual
            Console.WriteLine($"connect to server {client.Server.ProgId}:{client.Connected}");

            Console.WriteLine($"add group to server ...");
            client.Add(new Group() {Name = "default", UpdateRate = 100, ClientHandle = 010});
            client.Add(new Group() {Name = "group1", UpdateRate = 100, ClientHandle = 100});
            client.Add(new Group() {Name = "group2", UpdateRate = 100, ClientHandle = 200});

            Console.WriteLine($"add items to groups ...");
            client.Groups["default"].Add(new Tag("Channel1.Device1.Bool", 001));
            client.Groups["default"].Add(new Tag("Channel1.Device1.Heartbeat", 010));
            client.Groups["default"].Add(new Tag("Channel1.Device1.RunMode", 011));

            client.Groups["group1"].Add(
                new Tag("Channel1.Device1.Group1.Code", 100),
                new Tag("Channel1.Device1.Group1.Number", 101));

            client.Groups["group2"].Add(new Tag("Channel1.Device1.Group2.Code", 200));
            client.Groups["group2"].Add(new Tag("Channel1.Device1.Group2.Number", 201));
            */

            //var props = client.GetItemProperties("Test.Simulator.Booleans.B0001");

            Console.WriteLine($"waitting Reads ...");
            Console.ReadLine();

            foreach (var group in client.Groups.Values)
            {
                if (group.Tags.Count == 0) continue;
                var results = group.Reads(group.Tags.Values.Select(x => x.ItemName)?.ToArray());

                foreach (var item in results)
                {
                    Console.WriteLine(
                        $"Sync Read {item.Result.ItemName}:{item.Result.Value},{item.Result.Quality} / {item.Result.Timestamp} / {item.Ok}|{item.Code}");
                }
            }

            Console.WriteLine($"waitting ReadsAsync ...");
            Console.ReadLine();
            foreach (var group in client.Groups.Values)
            {
                if (group.Tags.Count == 0) continue;
                var results = group.ReadsAsync(group.Tags.Values.Select(x => x.ItemName)?.ToArray());

                foreach (var item in results)
                {
                    Console.WriteLine($"Async Read {item.Result.ItemName} / {item.Ok}|{item.Code}");
                }
            }

            Console.WriteLine($"waitting Writes ...");
            Console.ReadLine();
            foreach (var group in client.Groups.Values)
            {
                if (group.Tags.Count == 0) continue;
                var results = group.Writes(group.Tags.Values.Select(x => new KeyValuePair<string, object>(x.ItemName, write++))?.ToArray());

                foreach (var item in results)
                {
                    Console.WriteLine($"Sync Wtite {item.Result.Key} / {item.Ok}|{item.Code}");
                }
            }

            Console.WriteLine($"waitting WritesAsync ...");
            Console.ReadLine();
            foreach (var group in client.Groups.Values)
            {
                if (group.Tags.Count == 0) continue;
                var results = group.WritesAsync(group.Tags.Values.Select(x => new KeyValuePair<string, object>(x.ItemName, write++))?.ToArray());

                foreach (var item in results)
                {
                    Console.WriteLine($"Async Wtite {item.Result.Key} / {item.Ok}|{item.Code}");
                }
            }

            Console.WriteLine($"waitting RefreshAsync ...");
            Console.ReadLine();

            foreach (var group in client.Groups)
            {
                group.Value.RefreshAsync();
            }

            Console.WriteLine($"RefreshAsync");

            Console.WriteLine($"waitting disconnect ...");

            client.Disconnect();
            client.Dispose();

            Console.WriteLine($"Disposed");
            Console.ReadLine();
        }

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
    }
}