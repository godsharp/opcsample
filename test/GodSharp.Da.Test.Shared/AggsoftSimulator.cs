using GodSharp.Opc.Da;

namespace GodSharp.Da.Test;

internal class AggsoftSimulator : Runner
{
    protected override IOpcDaClient Create()
    {
        return DaClientFactory.Instance.CreateOpcAutomationClient(new DaClientOptions()
        {
            Data = new Opc.Da.Options.ServerData()
            {
                Host = "localhost",
                ProgId = "opcserversim.Instance.1"
            },
            OnDataChangedHandler = OnDataChangedHandler,
            OnServerShutdownHandler = OnShoutdownHandler,
            OnAsyncReadCompletedHandler = OnAsyncReadCompletedHandler,
            OnAsyncWriteCompletedHandler = OnAsyncWriteCompletedHandler
        });
    }

    protected override void Running(IOpcDaClient client)
    {
        var items = new string[]
        {
            "BooleanValue",
            "ByteValue",
            "DateTimeValue",
            "DateValue",
            "DoubleValue",
            "Int64Value",
            "IntegerValue",
            "LongWordValue",
            "ShortIntValue",
            "SingleValue",
            "SmallIntValue",
            "TimeValue",
            "WordValue",
            "StringValue"
        };

        try
        {
            Console.WriteLine("Add Tags ...");
            var tags = items.Select((x, i) => new Tag(x, i + 1)).ToArray();
            client.Current.Add(tags);
            Console.WriteLine("Add Tags Finished");

            foreach (var item in client.Current.Tags)
            {
                Console.WriteLine($"Added tag {item.Value.ClientHandle}:{item.Value.ItemName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}