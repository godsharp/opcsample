using GodSharp.Extensions.Opc.Ua.Utilities;
using GodSharp.Opc.Ua;

using Mapster;

using Opc.Ua;

using System;
using System.IO;

namespace GodSharpOpcUaClientSample
{
    public abstract class OpcUaTestRunner
    {
        protected readonly OpcUaClient Client;
        protected readonly Random Random;

        protected OpcUaTestRunner(OpcUaClient client)
        {
            Client = client;
            Random = new Random((int)System.DateTime.Now.Ticks);
        }

        public abstract void Run();

        protected T[] GetRandomArray<T>(Func<Random, T> factory, int length = 8)
        {
            T[] array = new T[length];
            for (int i = 0; i < length; i++) array[i] = factory(Random);

            return array;
        }

        protected string GetByteString(int length = 8)
        {
            string str = null;
            for (int i = 0; i < length; i++) str += Random.Next(byte.MinValue, byte.MaxValue + 1).ToString("x2");

            return str;
        }

        protected void Run<T>(T value, string node, Func<T, T, bool> compare, Func<T, string> output=null)
        {
            bool ret = Client.Write(node, value);
            Console.WriteLine($"Write> {node}={(output == null ? value.ToString() : output(value))}:{ret}");
            if (!ret) return;
            var read = Client.Read<T>(node);
            Console.WriteLine($"Read> {node}={(output == null ? read.ToString() : output(read))}:{compare(read, value)}");
            Console.WriteLine(new string('-', 48));
        }

        protected void RunStruct<T>(T value, string node, Func<T, string> output, Func<T, T, bool> compare) where T : struct
        {
            ExtensionObject v = new ExtensionObject(node, StructConverter.GetBytes(value));
            bool res = Client.Write(node, v);
            Console.WriteLine($"Write> {node}={{{output(value)}}}:{res}");

            var raw = Client.Read(node);
            var val = StructConverter.GetStruct<T>((raw.Value as ExtensionObject).Body as byte[]);
            Console.WriteLine($"Read> {node}={{{output(val)}}}:{compare(value, val)}");
            Console.WriteLine(new string('-', 48));
        }

        protected void RunEncodeableObject<T>(T value, string node, Func<T, string> output, Func<T, T, bool> compare) where T : EncodeableObject
        {
            bool res = Client.Write(node, value);
            Console.WriteLine($"Write> {node}={{{output(value)}}}:{res}");

            var raw = Client.Read(node);
            var val = GetValue<T>(raw, default);
            Console.WriteLine($"Read> {node}={{{output(val)}}}:{compare(value, val)}");
            Console.WriteLine(new string('-', 48));
        }

        protected T GetValue<T>(DataValue value, T defaultValue)
        {
            if (StatusCode.IsNotGood(value.StatusCode))
            {
                return defaultValue;
            }

            if (typeof(T).IsInstanceOfType(value.Value))
            {
                return (T)value.Value;
            }


            if (value.Value is ExtensionObject extension)
            {
                return extension.Body.Adapt<T>();
            }

            return defaultValue;
        }

        protected void Bool(string node) => Run(Random.Next(0, 10) % 2 == 0, node, (x1, x2) => x1 == x2);

        protected void Byte(string node) => Run((byte)Random.Next(byte.MinValue, byte.MaxValue), node, (x1, x2) => x1 == x2);
        protected void Short(string node) => Run((short)Random.Next(short.MinValue, short.MaxValue), node, (x1, x2) => x1 == x2);
        protected void Int(string node) => Run(Random.Next(int.MinValue, int.MaxValue), node, (x1, x2) => x1 == x2);
        protected void Long(string node) => Run(long.MaxValue - (Random.Next(0, int.MaxValue) - uint.MaxValue), node, (x1, x2) => x1 == x2);

        protected void SByte(string node) => Run((sbyte)Random.Next(sbyte.MinValue, sbyte.MaxValue), node, (x1, x2) => x1 == x2);
        protected void UShort(string node) => Run((ushort)Random.Next(ushort.MinValue, ushort.MaxValue), node, (x1, x2) => x1 == x2);
        protected void UInt(string node) => Run((uint)Random.Next(0, int.MaxValue), node, (x1, x2) => x1 == x2);
        protected void ULong(string node) => Run(ulong.MaxValue - (ulong)(Random.Next(0, int.MaxValue) - uint.MaxValue), node, (x1, x2) => x1 == x2);

        protected void FloatMax(string node) => Run((float)(Random.NextDouble() * float.MaxValue), node, (x1, x2) => x1 == x2);
        protected void FloatMin(string node) => Run((float)(Random.NextDouble() * float.MinValue), node, (x1, x2) => x1 == x2);
        protected void Float(string node)
        {
            FloatMax(node);
            FloatMin(node);
        }

        protected void DoubleMax(string node) => Run((double)(Random.NextDouble() * double.MaxValue), node, (x1, x2) => x1 == x2);
        protected void DoubleMin(string node) => Run((double)(Random.NextDouble() * double.MinValue), node, (x1, x2) => x1 == x2);
        protected void Double(string node)
        {
            DoubleMax(node);
            DoubleMin(node);
        }
        protected void String(string node, string value = null) => Run(value ?? Path.GetRandomFileName(), node, (x1, x2) => x1 == x2);
        protected void DateTime(string node, DateTime? dt = null) => Run(dt ?? System.DateTime.Now, node, (x1, x2) => x1 == x2);
        protected void DateTimeUtc(string node) => DateTime(node, System.DateTime.UtcNow);
        protected void Guid(string node) => Run(new Uuid(System.Guid.NewGuid()), node, (x1, x2) => x1 == x2);
    }
}
