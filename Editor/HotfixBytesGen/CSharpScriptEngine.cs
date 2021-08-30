using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class CSharpScriptEngine
{
    public static Assembly GetAssembly(string[] fileStrings)
    {
        var syntaxTrees = new Microsoft.CodeAnalysis.SyntaxTree[fileStrings.Length];
        for (int i = 0; i < fileStrings.Length; i++)
        {
            var tree = CSharpSyntaxTree.ParseText(fileStrings[i]);
            syntaxTrees[i] = tree;
            
            var root = tree.GetRoot();
            foreach(var child in root.ChildNodes())
            {
                //UnityEngine.Debug.Log(child.ToString());
                foreach(var ch in child.ChildNodes())
                {
                    UnityEngine.Debug.Log(ch.ToString());
                }
            }
        }

        string assemblyName = Path.GetRandomFileName();
        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var references = new List<MetadataReference>();
        foreach (var assembly in currentAssemblies)
        {
            if (string.IsNullOrEmpty(assembly.Location))
            {
                continue;
            }
            references.Add(MetadataReference.CreateFromFile(assembly.Location));
        }

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: syntaxTrees,
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using (var ms = new MemoryStream())
        {
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                {
                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                }
                return null;
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                File.WriteAllBytes("C:\\Users\\Administrator\\Desktop\\test.dll", ms.ToArray());
                return assembly;
            }
        }
    }
}