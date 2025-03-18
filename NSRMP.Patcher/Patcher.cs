using System.Runtime.InteropServices;
using BepInEx;
using Mirror.Weaver;
using Mono.Cecil;

namespace NSRMP.Patcher;

public class BepInExLogger : Logger
{
    public void Warning(string message)
    {
        Console.WriteLine(message);
    }

    public void Warning(string message, MemberReference mr)
    {
        Console.WriteLine(message, mr);
    }

    public void Error(string message)
    {
        Console.WriteLine($"Error: {message}");
    }

    public void Error(string message, MemberReference mr)
    {
        Console.WriteLine($"Error: {message} | {mr}");
    }
}

public static class Patcher
{
    public static IEnumerable<string> TargetDLLs { get; } = ["Assembly-CSharp.dll"];

    private static void WeaveAssembly(string assemblyPath)
    {
        using var asmResolver = new DefaultAssemblyResolver();

        foreach (var dir in Directory.GetDirectories(RuntimeEnvironment.GetRuntimeDirectory(), "*",
                     SearchOption.AllDirectories))
        {
            asmResolver.AddSearchDirectory(dir);
        }

        asmResolver.AddSearchDirectory(Path.Combine(Paths.PluginPath, "NSRMP", "Dependencies"));
        asmResolver.AddSearchDirectory(Path.Combine(Paths.PluginPath, "MMHOOK"));
        asmResolver.AddSearchDirectory(Paths.ManagedPath);
        asmResolver.AddSearchDirectory(Paths.BepInExAssemblyDirectory);

        using var modAssembly = AssemblyDefinition.ReadAssembly(assemblyPath,
            new ReaderParameters { AssemblyResolver = asmResolver });

        var weaver = new Weaver(new BepInExLogger());
        Console.WriteLine(weaver.Weave(modAssembly, asmResolver, out var modified)
            ? "Weaver complete!"
            : "ERROR: Weaver failed!");

        if (modified)
        {
            Console.WriteLine("Writing to assembly. . .");
            modAssembly.Write(new WriterParameters { WriteSymbols = true });
        }
    }
    
    public static void Finish()
    {
        WeaveAssembly(Path.Combine(Paths.ManagedPath, "Assembly-CSharp.dll"));
        WeaveAssembly(Path.Combine(Paths.PluginPath, "NSRMP", "NSRMP.dll"));
    }
    
    public static void Patch(AssemblyDefinition assembly) {}
}