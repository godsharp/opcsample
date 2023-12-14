using System.Reflection;

namespace GodSharp.Da.Test;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("操作系统版本： " + Environment.OSVersion);
        Console.WriteLine("处理器数目： " + Environment.ProcessorCount);
        Console.WriteLine("CLR版本： " + Environment.Version);
        Console.WriteLine("是否64位系统： " + Environment.Is64BitOperatingSystem);
        Console.WriteLine("是否64位进程： " + Environment.Is64BitProcess);
        var assemblis = Assembly.GetExecutingAssembly()!.GetReferencedAssemblies();

        foreach (var item in assemblis)
        {
            Console.WriteLine($"{item.FullName}");
            if (item.Name == "GodSharp.Opc.Da.OpcAutomation.Graybox")
            {
                Console.WriteLine(item.ProcessorArchitecture);
            }
        }

        Console.WriteLine("Press any key continue...");
        Console.ReadLine();
        new AggsoftSimulator().Run();
    }
}