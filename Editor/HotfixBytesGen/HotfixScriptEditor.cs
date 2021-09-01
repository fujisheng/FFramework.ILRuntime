using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Framework.ILR.Editor
{
    public class HotfixScriptEditor : UnityEditor.Editor
    {
        //TODO 基本功能已经跑通  后续可以添加自动分析引用，添加宏编译 自动编译等功能
        [MenuItem("Tools/Framework.ILRuntime/GenScriptBytes")]
        public static void GenScriptBytes()
        {
            List<string> scripts = new List<string>();
            var fframeworkFiles = Directory.GetFiles("./FUPMPackages/com.fujisheng.fframework.ilruntime/Runtime/Hotfix/", "*.cs", SearchOption.AllDirectories);
            var gameFiles = Directory.GetFiles("./Assets/Scripts/Hotfix/", "*.cs");

            UnityEngine.Debug.Log(fframeworkFiles.Length);
            var files = new List<string>();
            files.AddRange(fframeworkFiles);
            files.AddRange(gameFiles);
            foreach(var file in files)
            {
                var str = File.ReadAllText(file);
                UnityEngine.Debug.Log(file);
                scripts.Add(str);
            }
            var syntaxTrees = new Microsoft.CodeAnalysis.SyntaxTree[scripts.Count];
            for (int i = 0; i < scripts.Count; i++)
            {
                var tree = CSharpSyntaxTree.ParseText(scripts[i], new CSharpParseOptions().WithLanguageVersion(LanguageVersion.CSharp7_1));
                syntaxTrees[i] = tree;
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

                if (assembly.Location.EndsWith(".Hotfix.dll"))
                {
                    continue;
                }

                UnityEngine.Debug.Log(assembly.Location);
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var pdbStream = new MemoryStream())
            using (var dllStream = new MemoryStream())
            {
                EmitResult result = compilation.Emit(dllStream, pdbStream, null, null, null, new EmitOptions(false, DebugInformationFormat.PortablePdb));

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        UnityEngine.Debug.LogError($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }
                    return;
                }
                else
                {
                    dllStream.Seek(0, SeekOrigin.Begin);
                    pdbStream.Seek(0, SeekOrigin.Begin);
                    var dllBytes = dllStream.ToArray();
                    dllBytes = Utility.Encryption.AESEncrypt(dllBytes);
                    var pdbBytes = pdbStream.ToArray();
                    pdbBytes = Utility.Encryption.AESEncrypt(pdbBytes);
                    File.WriteAllBytes("./Assets/Sources/Code/Game.Hotfix.dll.bytes", dllBytes);
                    File.WriteAllBytes("./Assets/Sources/Code/Game.Hotfix.pdb.bytes", pdbBytes);
                    UnityEngine.Debug.Log("生成热更bytes成功");
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
