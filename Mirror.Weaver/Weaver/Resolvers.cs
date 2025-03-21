// all the resolve functions for the weaver
// NOTE: these functions should be made extensions, but right now they still
//       make heavy use of Weaver.fail and we'd have to check each one's return
//       value for null otherwise.
//       (original FieldType.Resolve returns null if not found too, so
//        exceptions would be a bit inconsistent here)
using Mono.Cecil;

namespace Mirror.Weaver
{
    public static class Resolvers
    {
        public static MethodReference ResolveMethod(TypeReference tr, AssemblyDefinition assembly, Logger Log, string name, ref bool WeavingFailed)
        {
            if (tr == null)
            {
                Log.Error($"Cannot resolve method {name} without a class");
                WeavingFailed = true;
                return null;
            }
            MethodReference method = ResolveMethod(tr, assembly, Log, m => m.Name == name, ref WeavingFailed);
            if (method == null)
            {
                Log.Error($"Method not found with name {name} in type {tr.Name}", tr);
                WeavingFailed = true;
            }
            return method;
        }

        public static MethodReference ResolveMethod(TypeReference t, AssemblyDefinition assembly, Logger Log, System.Func<MethodDefinition, bool> predicate, ref bool WeavingFailed)
        {
            Console.WriteLine($"Resolving method in type \"{t.FullName}\". . .");
            foreach (MethodDefinition methodRef in t.Resolve().Methods)
            {
                if (predicate(methodRef))
                {
                    return assembly.MainModule.ImportReference(methodRef);
                }
            }

            Log.Error($"Method not found in type {t.Name}", t);
            WeavingFailed = true;
            return null;
        }

        public static FieldReference ResolveField(TypeReference tr, AssemblyDefinition assembly, Logger Log, string name, ref bool WeavingFailed)
        {
            if (tr == null)
            {
                Log.Error($"Cannot resolve Field {name} without a class");
                WeavingFailed = true;
                return null;
            }
            FieldReference field = ResolveField(tr, assembly, Log, m => m.Name == name, ref WeavingFailed);
            if (field == null)
            {
                Log.Error($"Field not found with name {name} in type {tr.Name}", tr);
                WeavingFailed = true;
            }
            return field;
        }

        public static FieldReference ResolveField(TypeReference t, AssemblyDefinition assembly, Logger Log, System.Func<FieldDefinition, bool> predicate, ref bool WeavingFailed)
        {
            foreach (FieldDefinition fieldRef in t.Resolve().Fields)
            {
                if (predicate(fieldRef))
                {
                    return assembly.MainModule.ImportReference(fieldRef);
                }
            }

            Log.Error($"Field not found in type {t.Name}", t);
            WeavingFailed = true;
            return null;
        }

        public static MethodReference TryResolveMethodInParents(TypeReference tr, AssemblyDefinition assembly, string name)
        {
            if (tr == null)
            {
                return null;
            }
            foreach (MethodDefinition methodDef in tr.Resolve().Methods)
            {
                if (methodDef.Name == name)
                {
                    MethodReference methodRef = methodDef;
                    if (tr.IsGenericInstance)
                    {
                        methodRef = methodRef.MakeHostInstanceGeneric(tr.Module, (GenericInstanceType)tr);
                    }
                    return assembly.MainModule.ImportReference(methodRef);
                }
            }

            // Could not find the method in this class,  try the parent
            return TryResolveMethodInParents(tr.Resolve().BaseType.ApplyGenericParameters(tr), assembly, name);
        }

        public static MethodDefinition ResolveDefaultPublicCtor(TypeReference variable)
        {
            foreach (MethodDefinition methodRef in variable.Resolve().Methods)
            {
                if (methodRef.Name == ".ctor" &&
                    methodRef.Resolve().IsPublic &&
                    methodRef.Parameters.Count == 0)
                {
                    return methodRef;
                }
            }
            return null;
        }

        public static MethodReference ResolveProperty(TypeReference tr, AssemblyDefinition assembly, string name)
        {
            foreach (PropertyDefinition pd in tr.Resolve().Properties)
            {
                if (pd.Name == name)
                {
                    return assembly.MainModule.ImportReference(pd.GetMethod);
                }
            }
            return null;
        }
    }
}
