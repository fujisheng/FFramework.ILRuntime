using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Framework.Module.Script.Editor
{
    [InitializeOnLoad]
    public class ScriptsEditor : UnityEditor.Editor
    {
        private const string ScriptAssembliesDir = "Library/ScriptAssemblies";
        private const string CodeDir = "Assets/Sources/Code/";
        private const string FrameworkHotfixDll = "Framework.IL.Hotfix.dll";
        private const string FrameworkHotfixPdb = "Framework.IL.Hotfix.pdb";
        private const string GameHotfixDll = "Game.Hotfix.dll";
        private const string GameHotfixPdb = "Game.Hotfix.pdb";

        static ScriptsEditor()
        {
            CompilationPipeline.assemblyCompilationFinished += AssemblyCompilationFinishedCallback;
        }

        //[MenuItem("Tools/Script/CopyDLL")]
        //static void CopyDll()
        //{
        //    // Copy最新的pdb文件
        //    string[] dirs =
        //    {
        //        "./Temp/UnityVS_bin/Debug",
        //        "./Temp/UnityVS_bin/Release",
        //        "./Temp/bin/Debug",
        //        "./Temp/bin/Release"
        //    };

        //    DateTime dateTime = DateTime.MinValue;
        //    string newestDir = "";
        //    foreach (string dir in dirs)
        //    {
        //        string gameDllPath = Path.Combine(dir, GameHotfixDll);
        //        if (!File.Exists(gameDllPath))
        //        {
        //            continue;
        //        }
        //        FileInfo fi = new FileInfo(gameDllPath);
        //        DateTime lastWriteTimeUtc = fi.LastWriteTimeUtc;
        //        if (lastWriteTimeUtc > dateTime)
        //        {
        //            newestDir = dir;
        //            dateTime = lastWriteTimeUtc;
        //        }
        //    }

        //    if (newestDir != "")
        //    {
        //        File.Copy(Path.Combine(newestDir, GameHotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
        //        File.Copy(Path.Combine(newestDir, GameHotfixPdb), Path.Combine(CodeDir, "Hotfix.pdb.bytes"), true);
        //        Debug.Log($"ilrt 复制Hotfix.dll, Hotfix.pdb到Sources/DLL完成");
        //    }
        //    //AssetDatabase.Refresh();
        //}

        static void AssemblyCompilationFinishedCallback(string file, CompilerMessage[] messages)
        {
            CopyToSources(file, FrameworkHotfixDll, FrameworkHotfixPdb);
            CopyToSources(file, GameHotfixDll, GameHotfixPdb);
        }

        static void CopyToSources(string file, string dllName, string pdbName)
        {
            if (file.EndsWith(dllName))
            {
                string pdbPath = file.Replace(dllName, pdbName);
                File.Copy(file, Path.Combine(CodeDir, $"{dllName}.bytes"), true);
                File.Copy(pdbPath, Path.Combine(CodeDir, $"{pdbName}.bytes"), true);
                AssetDatabase.Refresh();
            }
        }
    }
}