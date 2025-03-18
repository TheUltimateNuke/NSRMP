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
        using (var asmResolver = new DefaultAssemblyResolver())
        {
            asmResolver.AddSearchDirectory(Paths.ManagedPath);
            asmResolver.AddSearchDirectory(Path.Combine(Paths.PluginPath, "NSRMP", "Dependencies"));
            asmResolver.AddSearchDirectory(Paths.BepInExAssemblyDirectory);

            var weaver = new Weaver(new BepInExLogger());
            Console.WriteLine(weaver.Weave(assembly, asmResolver, out var modified)
                ? "Weaver complete!"
                : "ERROR: Weaver failed!");

            if (modified)
            {
                assembly.Write(new WriterParameters() { WriteSymbols = true });
            }
        }
    }
}