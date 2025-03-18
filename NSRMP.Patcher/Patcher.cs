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

public class Patcher
{
    public static IEnumerable<string> TargetDLLs { get; } = ["Assembly-CSharp.dll"];

    public static void Patch(AssemblyDefinition assembly)
    {
        using var asmResolver = new DefaultAssemblyResolver();
        // add .NET runtime dir for the sake of security
        foreach (var dir in Directory.GetDirectories(RuntimeEnvironment.GetRuntimeDirectory(), "*", SearchOption.AllDirectories))
        {
            asmResolver.AddSearchDirectory(dir);
        }
        asmResolver.AddSearchDirectory(Path.Combine(Paths.PluginPath, "NSRMP", "Dependencies"));
        asmResolver.AddSearchDirectory(Paths.BepInExAssemblyDirectory);

        using var modAssembly = AssemblyDefinition.ReadAssembly(Path.Combine(Paths.PluginPath, "NSRMP", "NSRMP.dll"), new ReaderParameters() { ReadSymbols = true, AssemblyResolver = asmResolver});
        
        var weaver = new Weaver(new BepInExLogger());
        Console.WriteLine(weaver.Weave(modAssembly, asmResolver, out var modified)
            ? "Weaver complete!"
            : "ERROR: Weaver failed!");

        if (modified)
        {
            Console.WriteLine("Writing to assembly. . .");
            assembly.Write(new WriterParameters() { WriteSymbols = true });
        }
    }
}