// See https://aka.ms/new-console-template for more information
using GodSharp.Opc.Ua;

using Opc.Ua;

Console.WriteLine("Hello, GodSharpOpcUaDiscoveryServer!");
// Auto Generate ApplicationConfigurationFile
// GodSharp.Opc.Ua.OpcUaHelper.GenerateApplicationConfigurationFile = true;

// Configuration for UA Discovery Server
ApplicationConfiguration? configuration = null;
configuration = await GodSharp.Opc.Ua.OpcUaHelper.DiscoveryServerConfigurationAsync(
    "GodSharpOpcUaDiscoveryServer",
    "GodSharp Opc Ua Discovery Server",
    new string[] { Utils.Format(@"opc.tcp://{0}:4840", "localhost") }
    //,(s, e) => CertificateValidationHandler(configuration, s, e)
    , CertificateValidationHandler
    );

// Instance UA Server
var ds = new OpcUaDiscoveryServer(TimeSpan.FromSeconds(15));

// Start UA Discovery Server
ds.Start(configuration);
Console.WriteLine("GodSharpOpcUaDiscoveryServer Started!");
Console.ReadLine();

// Start UA Discovery Server
ds.Stop();

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