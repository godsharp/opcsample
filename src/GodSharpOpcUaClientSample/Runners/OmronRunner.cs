using GodSharp.Opc.Ua;

using Opc.Ua;

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GodSharpOpcUaClientSample
{
    public class OmronRunner : OpcUaTestRunner
    {
        public OmronRunner(OpcUaClient client) : base(client)
        {
        }

        public override void Run()
        {
            /*
                Data type	"Number of bytes"	Range
                BOOL	2	0 or 1
                BYTE	1	16#00 to FF
                WORD	2	16#0000 to FFFF
                DWORD	4	16#00000000 to FFFFFFFF
                LWORD	8	16#0000000000000000 to FFFFFFFFFFFFFFFF
                SINT	1	-128 to +127
                INT	    2	-32768 to +32767
                DINT	4	-2147483648 to +2147483647
                LINT	8	-9223372036854775808 to +9223372036854775807
                USINT	1	0 to +255
                UINT	2	0 to +65535
                UDINT	4	0 to +4294967295
                ULINT	8	0 to +18446744073709551615
                REAL	4	-3.40282347e+38 to 3.40282347e+38
                LREAL	8	-1.7976931348623157e+308 to 1.7976931348623157e+308
             */

            Bool("ns=4;s=GouBool");
            
            // WORD
            Byte("ns=4;s=GouByte");
            UShort("ns=4;s=GouWord");
            UInt("ns=4;s=GouDWord");
            ULong("ns=4;s=GouLWord");

            // INT
            SByte("ns=4;s=GouSInt");
            Short("ns=4;s=GouInt");
            Int("ns=4;s=GouDInt");
            Long("ns=4;s=GouLInt");

            // UINT
            Byte("ns=4;s=GouUSInt");
            UShort("ns=4;s=GouUInt");
            UInt("ns=4;s=GouUDInt");
            ULong("ns=4;s=GouULInt");

            // REAL
            Float("ns=4;s=GouReal");

            // LREAL
            Double("ns=4;s=GouLReal");

            String("ns=4;s=GouString", Path.GetRandomFileName().Substring(0,Random.Next(6,10)));

            DateTime("ns=4;s=GouDateTime");

            //Guid("ns=4;s=GUIDDataItem");

            var ia = new short[10];
            for (int i = 0; i < ia.Length; i++) ia[i] = (short)Random.Next(short.MinValue, short.MaxValue);
            Run(ia, "ns=4;s=GouIntArray", (x1, x2) => Enumerable.SequenceEqual(x1, x2), a => string.Join(' ', a));

            //RunEncodeableObject(new OmronGouSocketAddress()
            //{
            //    PortNo = (ushort)Random.Next(0, ushort.MaxValue),
            //    IpAdr = $"{Random.Next(0, 255)}.{Random.Next(0, 255)}.{Random.Next(0, 255)}.{Random.Next(0, 255)}"
            //},
            //"ns=4;s=GouSocketAddress",
            //s => $"PortNo:{s.PortNo},IpAdr:{s.IpAdr}",
            //(s1, s2) => s1.PortNo == s2.PortNo && s1.IpAdr == s2.IpAdr
            //);

            //RunEncodeableObject(new OmronGouPoint()
            //{
            //    X = (float)(Random.NextDouble() * 100),
            //    Y = (float)(Random.NextDouble() * -100)
            //},
            //"ns=4;s=Gou_Point",
            //s => $"X:{s.X},Y:{s.Y}",
            //(s1, s2) => s1.X == s2.X && s1.Y == s2.Y
            //);

            //GouSocketAddress
            //RunStruct(new GouSocketAddress()
            //{
            //    PortNo = (ushort)Random.Next(0, ushort.MaxValue),
            //    IpAdr = $"{Random.Next(0, 255)}.{Random.Next(0, 255)}.{Random.Next(0, 255)}.{Random.Next(0, 255)}"
            //},
            //"ns=4;s=GouSocketAddress",
            //s => $"PortNo:{s.PortNo},IpAdr:{s.IpAdr}",
            //(s1, s2) => s1.PortNo == s2.PortNo && s1.IpAdr == s2.IpAdr
            //);


            //GouPoint
            RunStruct(new GouPoint()
            {
                X = (float)(Random.NextDouble() * 100),
                Y = (float)(Random.NextDouble() * -100)
            },
            "ns=4;s=Gou_Point",
            s => $"X:{s.X},Y:{s.Y}",
            (s1, s2) => s1.X == s2.X && s1.Y == s2.Y
            );

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct GouSocketAddress
        {
            public ushort PortNo;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst =256)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public string IpAdr;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct GouPoint
        {
            //[MarshalAs(UnmanagedType.R4)]
            public float X;
            //[MarshalAs(UnmanagedType.R4)]
            public float Y;
        }

        /// <summary>
        /// Encodeable GouPoint
        /// </summary>
        public class OmronGouPoint : EncodeableObject
        {
            public float X;
            public float Y;

            public override ExpandedNodeId XmlEncodingId => NodeId.Null;
            public override ExpandedNodeId TypeId => ExpandedNodeId.Parse("nsu=urn:OMRON:NxOpcUaServer:FactoryAutomation;i=5004");
            public override ExpandedNodeId BinaryEncodingId => ExpandedNodeId.Parse("nsu=urn:OMRON:NxOpcUaServer:FactoryAutomation;i=5005");

            public OmronGouPoint() : base()
            {
            }

            public override void Encode(IEncoder encoder)
            {
                encoder.WriteFloat("X", this.X);
                encoder.WriteFloat("Y", this.Y);
            }

            public override void Decode(IDecoder decoder)
            {
                X = decoder.ReadFloat("X");
                Y = decoder.ReadFloat("Y");
            }

            public override string ToString() => $"{{ X={this.X}; Y={this.Y}; }}";
        }

        /// <summary>
        /// Encodeable GouSocketAddress
        /// </summary>
        public class OmronGouSocketAddress : EncodeableObject
        {
            public ushort PortNo;
            public string IpAdr;

            public override ExpandedNodeId XmlEncodingId => NodeId.Null;
            public override ExpandedNodeId TypeId => ExpandedNodeId.Parse("nsu=urn:OMRON:NxOpcUaServer:FactoryAutomation;i=5007");
            public override ExpandedNodeId BinaryEncodingId => ExpandedNodeId.Parse("nsu=urn:OMRON:NxOpcUaServer:FactoryAutomation;i=5008");

            public OmronGouSocketAddress() : base()
            {
            }

            public override void Encode(IEncoder encoder)
            {
                encoder.WriteUInt16("PortNo", this.PortNo);
                encoder.WriteString("IpAdr", this.IpAdr);
            }

            public override void Decode(IDecoder decoder)
            {
                PortNo = decoder.ReadUInt16("PortNo");
                IpAdr = decoder.ReadString("IpAdr");
            }

            public override string ToString() => $"{{ PortNo={this.PortNo}; IpAdr={this.IpAdr}; }}";
        }
    }
}
