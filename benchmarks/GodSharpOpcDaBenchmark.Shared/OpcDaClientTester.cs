using GodSharp.Opc.Da;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GodSharpOpcDaBenchmark;

public class OpcDaClientTester
{
    private const int Length = 1000;
    private const int Count = 100;
    private readonly IOpcDaClient _client;
    private ConcurrentDictionary<int, string> _tags;
    private ConcurrentDictionary<string, object?> _values;

    public OpcDaClientTester(IOpcDaClient client)
    {
        _tags = new();
        _values = new();
        for (int i = 1; i <= Length; i++)
        {
            _tags.TryAdd(i, $"Test.Simulator.Booleans.B{i:0000}");
            _values.TryAdd($"Test.Simulator.Booleans.B{i:0000}", null);
            _tags.TryAdd(1000 + i, $"Test.Simulator.Numbers.N{i:0000}");
            _values.TryAdd($"Test.Simulator.Numbers.N{i:0000}", null);
            _tags.TryAdd(2000 + i, $"Test.Simulator.Characters.C{i:0000}");
            _values.TryAdd($"Test.Simulator.Characters.C{i:0000}", null);
        }
        _client = client;
        var group = _client.Add(new Group() { Name = "t", ClientHandle = 1, UpdateRate = 10 });
        var array = _tags.Select(x => new Tag(x.Value, x.Key)).ToArray();
        group.Add(array);
    }

    public void Run()
    {
        Console.WriteLine("Running...");
        Read();
        Read10();
        Read20();
        Read50();
        Read100();
        Read500();
    }

    public void Read()
    {
        Watch(() =>
        {
            Random random = new((int)DateTime.Now.Ticks);
            for (int i = 0; i < Count; i++)
            {
                var val = _client.Current.Read(_tags[random.Next(1, 3000)]);
            }
        });
    }

    public void Read10()
    {
        Watch(() =>
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < Count; i++)
            {
                var val = _client.Current.Reads(RandomTagArray(random, 10));
            }
        });
    }

    public void Read20()
    {
        Watch(() =>
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < Count; i++)
            {
                var val = _client.Current.Reads(RandomTagArray(random, 20));
            }
        });
    }

    public void Read50()
    {
        Watch(() =>
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < Count; i++)
            {
                var val = _client.Current.Reads(RandomTagArray(random, 50));
            }
        });
    }

    public void Read100()
    {
        Watch(() =>
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < Count; i++)
            {
                var val = _client.Current.Reads(RandomTagArray(random, 100));
            }
        });
    }

    public void Read500()
    {
        Watch(() =>
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < Count; i++)
            {
                var val = _client.Current.Reads(RandomTagArray(random, 500));
            }
        });
    }

    private string[] RandomTagArray(Random random, int size)
    {
        var keys = RandomArray(random, size);

        var array = new string[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = _tags[keys[i]];
        }
        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int[] RandomArray(Random random, int size)
    {
        var array = new int[size];
        for (int i = 0; i < size; i++)
        {
            do
            {
                var v = random.Next(1, 3000);
                if (!array.Contains(v))
                {
                    array[i] = v;
                    break;
                }
            } while (true);
        }

        return array;
    }

    private void Watch(Action fun, [CallerMemberName] string? method = null)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        fun();
        stopwatch.Stop();
        var ns = stopwatch.Elapsed.Ticks * 100;
        Console.WriteLine($"{method}\t{ns}ns\t{stopwatch.Elapsed.TotalMilliseconds}ms\t{ns / Count}ns\t{ns / Count / 1000}us\t{ns / Count / 1000 / 1000}ms");
    }
}