using GodSharp.Opc.Ua;

using Opc.Ua;

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GodSharpOpcUaClientSample.Runners
{
    internal class MiscTestRunner : OpcUaTestRunner
    {
        public MiscTestRunner(OpcUaClient client) : base(client)
        {
        }

        public override void Run()
        {
            var client = Client;

            Node node = client.Session.ReadNode("ns=3;i=1007");

            if (node.NodeClass == NodeClass.Variable)
            {
                //Get the node id of node's data type
                VariableNode variableNode = (VariableNode)node.DataLock;
                NodeId nodeId = new NodeId(variableNode.DataType.Identifier, variableNode.DataType.NamespaceIndex);

                //Browse for HasEncoding
                ReferenceDescriptionCollection refDescCol;
                byte[] continuationPoint;
                client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HasEncoding, true, 0, out continuationPoint, out refDescCol);

                //Check For found reference
                if (refDescCol.Count == 0)
                {
                    Exception ex = new Exception("No data type to encode. Could be a build-in data type you want to read.");
                    throw ex;
                }

                //Check for HasEncoding reference with name "Default Binary"
                bool dataTypeFound = false;
                foreach (ReferenceDescription refDesc in refDescCol)
                {
                    if (refDesc.DisplayName.Text == "Default Binary")
                    {
                        nodeId = new NodeId(refDesc.NodeId.Identifier, refDesc.NodeId.NamespaceIndex);
                        dataTypeFound = true;
                    }
                    else if (dataTypeFound == false)
                    {
                        Exception ex = new Exception("No default binary data type found.");
                        throw ex;
                    }
                }

                //Browse for HasDescription
                refDescCol = null;
                client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HasDescription, true, 0, out continuationPoint, out refDescCol);

                //Check For found reference
                if (refDescCol.Count == 0)
                {
                    Exception ex = new Exception("No data type description found in address space.");
                    throw ex;
                }

                //Read from node id of the found description to get a value to parse for later on
                nodeId = new NodeId(refDescCol[0].NodeId.Identifier, refDescCol[0].NodeId.NamespaceIndex);
                DataValue resultValue = client.Session.ReadValue(nodeId);
                var parseString = resultValue.Value.ToString();

                //Browse for ComponentOf from last browsing result inversly
                refDescCol = null;
                client.Session.Browse(null, null, nodeId, 0u, BrowseDirection.Inverse, ReferenceTypeIds.HasComponent, true, 0, out continuationPoint, out refDescCol);

                //Check if reference was found
                if (refDescCol.Count == 0)
                {
                    Exception ex = new Exception("Data type isn't a component of parent type in address space. Can't continue decoding.");
                    throw ex;
                }

                //Read from node id of the found HasCompoment reference to get a XML file (as HEX string) containing struct/UDT information

                nodeId = new NodeId(refDescCol[0].NodeId.Identifier, refDescCol[0].NodeId.NamespaceIndex);
                resultValue = client.Session.ReadValue(nodeId);

                //Convert the HEX string to ASCII string
                String xmlString = ASCIIEncoding.ASCII.GetString((byte[])resultValue.Value);

                //Return the dictionary as ASCII string
                Console.WriteLine(xmlString);
            }

            //node = "ns=3;s=\"DB2\".\"X1\"";
            //var b1 = random.Next(0, 10) % 2 == 0;
            //var res1 = client.Write(node, b1);
            //Console.WriteLine($"Write> {node}={b1}:{res1}");
            //var x1 = client.Read<bool>(node);
            //Console.WriteLine($"Read> {node}={x1}/{x1 == b1}");

            //node = "ns=3;s=\"DB2\".\"ia\"";
            //var ia = new int[11];
            //for(int i=0;i<ia.Length;i++) ia[i]=random.Next(0, 100);
            //var res11 = client.Write(node, ia);
            //Console.WriteLine($"Write> {node}={string.Join(' ', ia)}:{res11}");
            //var ia1 = client.Read<int[]>(node);
            //Console.WriteLine($"Read> {node}={string.Join(' ', ia1)}/{string.Join(' ', ia1) == string.Join(' ', ia)}");

            //node = "ns=3;s=\"DB2\".\"Time\"";
            ////node = "ns=0;i=2266";
            //var t1 = DateTime.Now;
            //var res2 = client.Write(node, t1);
            //Console.WriteLine($"Write> {node}={t1:yyyy-MM-dd HH:mm:ss}:{res1}");
            ////var tm1 = client.Read<DateTime>(node);
            //raw = client.Read(node);
            //var tm1 = Helper.ByteToStruct<DateAndTime>(raw.Value as byte[]);
            //Console.WriteLine($"Read> {node}={tm1:yyyy-MM-dd HH:mm:ss}/{(short)t1.Year == tm1.Year}");
        }



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct DateAndTime
        {
            public short Year;
            public byte Mouth;
            public byte Day;
            public byte Hour;
            public byte Minute;
            public short Second;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct Vector
        {
            //[MarshalAs(UnmanagedType.R4,SizeConst =1)]
            public float X;
            //[MarshalAs(UnmanagedType.R4, SizeConst = 1)]
            public float Y;
            //[MarshalAs(UnmanagedType.R4, SizeConst = 1)]
            public float Z;
        }

        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        //public struct GouPoint2
        //{
        //    public float X;
        //    public float Y;
        //    public float Z;
        //}

        public class Vector2 : EncodeableObject
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

            public override ExpandedNodeId XmlEncodingId => NodeId.Null;
            //nsu=http://www.siemens.com/simatic-s7-opcua;s=TE_"DB2"."Location3d"
            //nsu=http://opcfoundation.org/UA/;i=18830
            public override ExpandedNodeId TypeId => ExpandedNodeId.Parse("nsu=http://www.siemens.com/simatic-s7-opcua;s=VT_\"V3\"");
            public override ExpandedNodeId BinaryEncodingId => ExpandedNodeId.Parse("nsu=http://www.siemens.com/simatic-s7-opcua;s=TD_\"V3\"");

            public Vector2() : base()
            {
            }

            public override void Encode(IEncoder encoder)
            {
                encoder.WriteDouble("X", this.X);
                encoder.WriteDouble("Y", this.Y);
                encoder.WriteDouble("Z", this.Z);
            }

            public override void Decode(IDecoder decoder)
            {
                X = decoder.ReadDouble("X");
                Y = decoder.ReadDouble("Y");
                Z = decoder.ReadDouble("Z");
            }

            public override string ToString() => $"{{ X={this.X}; Y={this.Y}; Z={this.Z}; }}";
        }
    }
}
