using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;

namespace Framework.ILR.Editor
{
    [Serializable]
    class AssemblyDefinition
    {
        public List<string> references;
    }

    [InitializeOnLoad]
    public class GenerateCodeSourcesEditor : UnityEditor.Editor
    {
        static readonly string settingName = "GenerateCodeSourcesEditorSetting";
        static readonly string createMenuName = "Tools/Framewrok.ILRuntime/CreateScriptEditorSetting";
        static GenerateCodeSourcesEditor()
        {
            CompilationPipeline.assemblyCompilationFinished += AssemblyCompilationFinishedCallback;
        }

        static bool LoadSetting(out GenerateCodeSourcesEditorSetting setting)
        {
            setting = EditorGUIUtility.Load($"{settingName}.asset") as GenerateCodeSourcesEditorSetting;
            if (setting == null)
            {
                UnityEngine.Debug.LogWarning($"{settingName} is Empty, please create with menu [{createMenuName}]");
                return false;
            }
            return true;
        }

        static void AssemblyCompilationFinishedCallback(string file, CompilerMessage[] messages)
        {
            if (!file.EndsWith("Hotfix.dll"))
            {
                return;
            }

            if(!LoadSetting(out var setting))
            {
                return;
            }

            GenerateCodeSources();
        }

        [MenuItem("Tools/Framework.ILRuntime/CreateScriptEditorSetting")]
        public static void CreateSetting()
        {
            if (!Directory.Exists("Assets/Editor Default Resources/"))
            {
                Directory.CreateDirectory("Assets/Editor Default Resources/");
            }

            var setting = CreateInstance<GenerateCodeSourcesEditorSetting>();
            AssetDatabase.CreateAsset(setting, $"Assets/Editor Default Resources/{settingName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Framework.ILRuntime/GenerateCodeSources")]
        public static void GenerateCodeSources()
        {
            //读取设置
            if(!LoadSetting(out var setting))
            {
                return;
            }

            //读取脚本
            List<string> scripts = new List<string>();
            foreach(var fileDirectory in setting.HotfixPath)
            {
                var files = Directory.GetFiles(fileDirectory, "*.cs", SearchOption.AllDirectories);
                foreach(var file in files)
                {
                    var script = File.ReadAllText(file);
                    scripts.Add(script);
                }
            }

            //解析语法树
            var syntaxTrees = new Microsoft.CodeAnalysis.SyntaxTree[scripts.Count];
            var parseOptions = new CSharpParseOptions()
                .WithLanguageVersion(LanguageVersion.CSharp7_1)
                .WithPreprocessorSymbols(setting.GetSymbols());

            for (int i = 0; i < scripts.Count; i++)
            {
                var tree = CSharpSyntaxTree.ParseText(scripts[i], parseOptions);
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

                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }

            //编译
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(setting.optimizationLevel);
            var compilation = CSharpCompilation.Create(assemblyName)
                .AddSyntaxTrees(syntaxTrees)
                .AddReferences(references)
                .WithOptions(compilationOptions);

            using (var pdbStream = new MemoryStream())
            using (var dllStream = new MemoryStream())
            {
                var emitOptions = new EmitOptions(false, DebugInformationFormat.PortablePdb);
                var result = compilation.Emit(dllStream, pdbStream, null, null, null, emitOptions);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        UnityEngine.Debug.LogError($"<color=red>[HotfixError]</color> {diagnostic.Id}: {diagnostic.GetMessage()}");
                    }
                    return;
                }

                dllStream.Seek(0, SeekOrigin.Begin);
                pdbStream.Seek(0, SeekOrigin.Begin);
                var dllBytes = dllStream.ToArray();
                dllBytes = Utility.Encryption.AESEncrypt(dllBytes);
                var pdbBytes = pdbStream.ToArray();
                pdbBytes = Utility.Encryption.AESEncrypt(pdbBytes);
                File.WriteAllBytes($"{setting.CodeSourcesPath}/{setting.DllName}.bytes", dllBytes);
                File.WriteAllBytes($"{setting.CodeSourcesPath}/{setting.PdbName}.bytes", pdbBytes);
                UnityEngine.Debug.Log("GenerateCodeSources bytes success");
                AssetDatabase.Refresh();
            }
        }
    }
}
