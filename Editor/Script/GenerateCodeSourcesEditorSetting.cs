using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.ILR.Editor
{
    public class GenerateCodeSourcesEditorSetting : ScriptableObject
    {
        public string ScriptingDefineSymbols = string.Empty;
        public OptimizationLevel optimizationLevel = OptimizationLevel.Debug;
        public BuildTargetGroup targetGroup = BuildTargetGroup.Android;
        public List<string> HotfixPath = new List<string>()
        {
            "./FUPMPackages/com.fujisheng.fframework.ilruntime/Runtime/Hotfix/",
            "./Assets/Scripts/Hotfix/",
        };
        public string CodeSourcesPath = "Assets/Sources/Code";
        public string DllName = "Game.Hotfix.dll";
        public string PdbName = "Game.Hotfix.pdb";

        public string[] GetSymbols()
        {
            if (string.IsNullOrEmpty(ScriptingDefineSymbols))
            {
                return null;
            }
            var result = new List<string>();
            var unitySymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            result.AddRange(SplitSymbols(unitySymbols));
            result.AddRange(SplitSymbols(ScriptingDefineSymbols));
            return result.ToArray();
        }

        string[] SplitSymbols(string symbolsString)
        {
            var result = new List<string>();
            var symbols = symbolsString.Split(';');

            foreach (var symbol in symbols)
            {
                if (string.IsNullOrEmpty(symbol))
                {
                    continue;
                }
                result.Add(symbol.Trim());
            }
            return result.ToArray();
        }
    }
}