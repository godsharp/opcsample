using GodSharp.Opc.Da;

namespace GodSharpOpcDaBenchmark.Automation;

internal class Program
{
    private static void Main(string[] args)
    {
        var client = DaClientFactory.Instance.CreateOpcAutomationClient(new DaClientOptions()
        {
            Data = new GodSharp.Opc.Da.Options.ServerData()
            {
                Host = "localhost",
                ProgId = "Kepware.KEPServerEX.V6"
            }
        });
        client.Connect();
        var tester = new OpcDaClientTester(client);
        tester.Run();
        client.Disconnect();
    }
}