// See https://aka.ms/new-console-template for more information
using GodSharp.Opc.Ua.Server;

using Opc.Ua;

Console.WriteLine("Hello, GodSharpOpcUaServer!");

// Auto Generate ApplicationConfigurationFile
GodSharp.Opc.Ua.OpcUaHelper.GenerateApplicationConfigurationFile = true;

// Configuration for UA Server
ApplicationConfiguration? configuration = null;
configuration = await GodSharp.Opc.Ua.OpcUaHelper.ServerConfigurationAsync(
    "GodSharpOpcUaServer",
    "GodSharp Opc Ua Server",
    new string[] { Utils.Format(@"opc.tcp://{0}:4840", "localhost") }
    //,(s, e) => CertificateValidationHandler(configuration, s, e)
    , CertificateValidationHandler
    );

// Instance UA Server
var server = new OpcUaStandardServer(new SampleServerAuthorizationFilter());

// Start server
server.Start(configuration);

Console.WriteLine("GodSharpOpcUaServer Started!");
string? input = null;

do
{
    input = Console.ReadLine();

    if (input == null) continue;
} while (input != "q");

// Stop server
server.Stop();

static void CertificateValidationHandler(
    //ApplicationConfiguration configuration,
    CertificateValidator sender, CertificateValidationEventArgs e)
{
    if (e.Error.StatusCode.Code == StatusCodes.BadCertificateUntrusted)
    {
        e.Accept = true;
        //return;
        //if (!Directory.Exists(configuration.SecurityConfiguration.RejectedCertificateStore.StorePath))
        //{
        //    Directory.CreateDirectory(configuration.SecurityConfiguration.RejectedCertificateStore.StorePath);
        //}

        //if (configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
        //{
        //    using (var store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore())
        //    {
        //        store.Add(e.Certificate);
        //    }
        //    e.Accept = true;
        //    return;
        //}
    }
}