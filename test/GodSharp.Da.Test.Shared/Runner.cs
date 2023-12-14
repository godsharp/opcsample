using GodSharp.Opc.Da;

namespace GodSharp.Da.Test;

internal abstract class Runner : IRunner
{
    public virtual void Run()
    {
        using var client =Create();

        Console.WriteLine("Connect Opc Server ...");
        client.Connect();
        Console.WriteLine("Connect Opc Server :" + client.Connected);

        if (client.Connected)
        {
            client.Add(new Group()
            {
                Name = "default",
                IsSubscribed = true,
                UpdateRate = 100
            });

            Running(client);

            var input = string.Empty;
            do
            {
                input = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(input)) continue;
            } while (input != "q");
        }
    }

    protected abstract IOpcDaClient Create();
    protected abstract void Running(IOpcDaClient client);

    protected static void OnDataChangedHandler(DataChangedOutput output)
    {
        Console.WriteLine($"{output.Data.ItemName}:{output.Data.Value},{output.Data.Quality} / {output.Data.Timestamp}");
    }

    protected static void OnAsyncReadCompletedHandler(AsyncReadCompletedOutput output)
    {
        Console.WriteLine(
            $"Async Read {output.Data.Result.ItemName}:{output.Data.Result.Value},{output.Data.Result.Quality} / {output.Data.Result.Timestamp} / {output.Data.Code}");
    }

    protected static void OnAsyncWriteCompletedHandler(AsyncWriteCompletedOutput output)
    {
        Console.WriteLine($"Async Write {output.Data.Result.ItemName}:{output.Data.Code}");
    }

    protected static void OnShoutdownHandler(Server server, string reason)
    {
        Console.WriteLine($"{server.Name}/{server.ProgId} Shoutdown : {reason}");
    }
}