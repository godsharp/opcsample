
using GodSharp.Opc.Ua;

using GodSharpOpcUaClientSample.Types;

using System.Linq;

namespace GodSharpOpcUaClientSample
{
    public class UaAnsiCServerRunner : OpcUaTestRunner
    {
        public UaAnsiCServerRunner(OpcUaClient client) : base(client)
        {
        }

        public override void Run()
        {
            Bool("ns=4;s=Demo.Static.Scalar.Boolean");

            Byte("ns=4;s=Demo.Static.Scalar.Byte");
            Short("ns=4;s=Demo.Static.Scalar.Int16");
            Int("ns=4;s=Demo.Static.Scalar.Int32");
            Long("ns=4;s=Demo.Static.Scalar.Int64");

            SByte("ns=4;s=Demo.Static.Scalar.SByte");
            UShort("ns=4;s=Demo.Static.Scalar.UInt16");
            UInt("ns=4;s=Demo.Static.Scalar.UInt32");
            ULong("ns=4;s=Demo.Static.Scalar.UInt64");

            Float("ns=4;s=Demo.Static.Scalar.Float");

            Double("ns=4;s=Demo.Static.Scalar.Double");

            String("ns=4;s=Demo.Static.Scalar.String");

            Run(
                GetRandomArray(r => (byte)r.Next(byte.MinValue, byte.MaxValue + 1), 8),
                "ns=4;s=Demo.Static.Scalar.ByteString",
                (s1, s2) => Enumerable.SequenceEqual(s1, s2),
                x => string.Join(' ', x.Select(s => s.ToString("x2")))
            );

            //DateTime("ns=4;s=Demo.Static.Scalar.DateTime");
            DateTimeUtc("ns=4;s=Demo.Static.Scalar.UtcTime");

            Guid("ns=4;s=Demo.Static.Scalar.Guid");

            RunEncodeableObject(new UaAnsiVector()
            {
                X = Random.NextDouble(),
                Y = Random.NextDouble(),
                Z = Random.NextDouble()
            },
                "ns=4;s=Demo.Static.Scalar.Vector",
                x => $"X:{x.X},Y:{x.Y},X:{x.Z}",
                (x1, x2) => x1.X == x2.X && x1.Y == x2.Y && x1.Z == x2.Z
            );
        }
    }

    /// <summary>
    /// ThreeDVector
    /// </summary>
    public class UaAnsiVector : VectorThree<double>
    {
        public UaAnsiVector() : base()
        {
            //TypeIdNamespace = "nsu=http://www.unifiedautomation.com/DemoServer/;i=3002";
            //BinaryEncodingIdNamespace = "nsu=http://www.unifiedautomation.com/DemoServer/;i=5054";
        }
    }
}
