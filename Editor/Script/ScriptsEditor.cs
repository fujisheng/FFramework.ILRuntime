using Framework.Utility;
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
        private const string CodeDir = "Assets/Sources/Code/";
        private const string GameHotfixDll = "Game.Hotfix.dll";
        private const string GameHotfixPdb = "Game.Hotfix.pdb";

        static ScriptsEditor()
        {
            CompilationPipeline.assemblyCompilationFinished += AssemblyCompilationFinishedCallback;
        }

        static void AssemblyCompilationFinishedCallback(string file, CompilerMessage[] messages)
        {
            CopyToSources(file, GameHotfixDll, GameHotfixPdb);
        }

        //将原始的dll拷贝到对应路径并且加密
        static void CopyToSources(string file, string dllName, string pdbName)
        {
            if (file.EndsWith(dllName))
            {
                CopyAndEncryption(file, dllName);
                string pdbPath = file.Replace(dllName, pdbName);
                CopyAndEncryption(pdbPath, pdbName);
                AssetDatabase.Refresh();
            }
        }

        static void CopyAndEncryption(string file, string fileName)
        {
            if (file.EndsWith(fileName))
            {
                FileStream fsread = File.Open(file, FileMode.Open);
                byte[] buffer = new byte[fsread.Length];
                fsread.Read(buffer, 0, buffer.Length);
                fsread.Close();
                FileStream fsW = new FileStream(Path.Combine(CodeDir, $"{fileName}.bytes"), FileMode.Create);
                byte[] enctryptBytes = EncryptionUtility.AESEncrypt(buffer);
                fsW.Write(enctryptBytes, 0, enctryptBytes.Length);
                fsW.Flush();
                fsW.Close();
            }
        }
    }
}