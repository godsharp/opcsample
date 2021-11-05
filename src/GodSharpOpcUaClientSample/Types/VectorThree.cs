using GodSharp.Extensions.Opc.Ua.Types;

namespace GodSharpOpcUaClientSample.Types
{
    /// <summary>
    /// ThreeDVector
    /// </summary>
    [ComplexObjectGenerator(ComplexObjectType.EncodeableObject)]
    public partial class VectorThree<T> : ComplexObject where T : struct
    {
        public T X;
        public T Y;
        public T Z;

        // public override ExpandedNodeId XmlEncodingId => NodeId.Null;
        // public override ExpandedNodeId TypeId => ExpandedNodeId.Parse(TypeIdNamespace);
        // public override ExpandedNodeId BinaryEncodingId => ExpandedNodeId.Parse(BinaryEncodingIdNamespace);

        protected VectorThree() : base()
        {
        }

        //public override void Encode(IEncoder encoder)
        //{
        //    encoder.Write(X, "X");
        //    encoder.Write(Y, "Y");
        //    encoder.Write(Z, "Z");
        //}

        //public override void Decode(IDecoder decoder)
        //{
        //    decoder.Read(ref X, "X");
        //    decoder.Read(ref Y, "Y");
        //    decoder.Read(ref Z, "Z");
        //}

        public override string ToString() => $"{{ X={this.X}; Y={this.Y}; Z={this.Z}; }}";
    }
}
