using Framework.Utility;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Framework.Module.Script.Editor
{
    [InitializeOnLoad]
    public class ScriptsEditor : UnityEditor.Editor
    {
        static ScriptsEditor()
        {
            CompilationPipeline.assemblyCompilationFinished += AssemblyCompilationFinishedCallback;
        }

        [MenuItem("Tools/Framewrok.ILRuntime/CreateScriptEditorSetting")]
        public static void CreateSetting()
        {
            if (!Directory.Exists("Assets/Editor Default Resources/"))
            {
                Directory.CreateDirectory("Assets/Editor Default Resources/");
            }

            var setting = CreateInstance<FrameworkILRuntimeEditorSetting>();
            AssetDatabase.CreateAsset(setting, "Assets/Editor Default Resources/FrameworkILRuntimeEditorSetting.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Framewrok.ILRuntime/GenerateCode")]
        public static void GenerateCodeSources()
        {
            var setting = EditorGUIUtility.Load("FrameworkILRuntimeEditorSetting.asset") as FrameworkILRuntimeEditorSetting;
            if (setting == null)
            {
                Debug.LogWarning("FrameworkILRuntimeEditorSetting is Empty, please create with menu [Tools/Framewrok.ILRuntime/CreateScriptEditorSetting]");
                return;
            }

            AssemblyCompilationFinishedCallback($"./Library/ScriptAssemblies/{setting.DllName}", null);
            AssemblyCompilationFinishedCallback($"./Library/ScriptAssemblies/{setting.PdbName}", null);
        }

        static void AssemblyCompilationFinishedCallback(string file, CompilerMessage[] messages)
        {
            var setting = EditorGUIUtility.Load("FrameworkILRuntimeEditorSetting.asset") as FrameworkILRuntimeEditorSetting;
            if(setting == null)
            {
                Debug.LogWarning("FrameworkILRuntimeEditorSetting is Empty, please create with menu [Tools/Framewrok.ILRuntime/CreateScriptEditorSetting]");
                return;
            }
            CopyToSources(file, setting.DllName, setting.PdbName, setting.CodeSourcesPath);
        }

        //将原始的dll拷贝到对应路径并且加密
        static void CopyToSources(string file, string dllName, string pdbName, string savePath)
        {
            if (file.EndsWith(dllName))
            {
                CopyAndEncryption(file, dllName, savePath);
                string pdbPath = file.Replace(dllName, pdbName);
                CopyAndEncryption(pdbPath, pdbName, savePath);
                AssetDatabase.Refresh();
            }
        }

        static void CopyAndEncryption(string file, string fileName, string savePath)
        {
            if (file.EndsWith(fileName))
            {
                FileStream fsread = File.Open(file, FileMode.Open);
                byte[] buffer = new byte[fsread.Length];
                fsread.Read(buffer, 0, buffer.Length);
                fsread.Close();
                var filePath = Path.Combine(savePath, $"{fileName}.bytes");

                if (!File.Exists(filePath))
                {
                    FileStream fscreate = File.Create(filePath);
                    fscreate.Close();
                }
                FileStream fsW = new FileStream(filePath, FileMode.Create);
                byte[] enctryptBytes = EncryptionUtility.AESEncrypt(buffer);
                fsW.Write(enctryptBytes, 0, enctryptBytes.Length);
                fsW.Flush();
                fsW.Close();
            }
        }
    }
}