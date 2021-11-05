using GodSharp.Opc.Ua;
using GodSharpOpcUaClientSample.Types;

using Mapster;

using Opc.Ua;

using System;
using System.IO;

namespace GodSharpOpcUaClientSample
{
    public class ProsysOpcUaSimulatorRunner : OpcUaTestRunner
    {
        public ProsysOpcUaSimulatorRunner(OpcUaClient client) : base(client)
        {
        }

        public override void Run()
        {
            Bool("ns=5;s=BooleanDataItem");

            Byte("ns=5;s=ByteDataItem");
            Short("ns=5;s=Int16DataItem");
            Int("ns=5;s=Int32DataItem");
            Long("ns=5;s=Int64DataItem");

            SByte("ns=5;s=SByteDataItem");
            UShort("ns=5;s=UInt16DataItem");
            UInt("ns=5;s=UInt32DataItem");
            ULong("ns=5;s=UInt64DataItem");

            Float("ns=5;s=FloatDataItem");

            Double("ns=5;s=DoubleDataItem");

            String("ns=5;s=StringDataItem");

            DateTime("ns=5;s=DateTimeDataItem");

            Guid("ns=5;s=GUIDDataItem");

            RunEncodeableObject(new ProsysVector()
            {
                X = Random.NextDouble(),
                Y = Random.NextDouble(),
                Z = Random.NextDouble()
            },
                "ns=3;i=1007",
                x => $"X:{x.X},Y:{x.Y},X:{x.Z}",
                (x1, x2) => x1.X == x2.X && x1.Y == x2.Y && x1.Z == x2.Z
             );
        }
    }

    /// <summary>
    /// ThreeDVector
    /// </summary>
    public class ProsysVector : VectorThree<double>
    {
        public ProsysVector() : base()
        {
            //TypeIdNamespace = "nsu=http://opcfoundation.org/UA/;i=18808";
            //BinaryEncodingIdNamespace = "nsu=http://opcfoundation.org/UA/;i=18817";
        }
    }
}
