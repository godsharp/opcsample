using GodSharp.Extensions.Opc.Ua.Types;

namespace GodSharpOpcUaClientSample.Types
{
[ComplexObjectGenerator(ComplexObjectType.SwitchField, EncodingMethodType.Factory)]
public partial class UaAnsiUnion
{
    [SwitchField(1)]
    public int Int32;
    [SwitchField(2, 4)]
    public string String;

    public UaAnsiUnion()
    {
        //TypeIdNamespace = "nsu=http://www.unifiedautomation.com/DemoServer/;i=3006";
        //BinaryEncodingIdNamespace = "nsu=http://www.unifiedautomation.com/DemoServer/;i=5003";
    }
}
}
