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
            var result = new List<string>();
            var unitySymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            result.AddRange(SplitSymbols(unitySymbols));
            if (string.IsNullOrEmpty(ScriptingDefineSymbols))
            {
                return result.ToArray();
            }
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

    [CustomEditor(typeof(GenerateCodeSourcesEditorSetting))]
    public class GenerateCodeSourceEditorSettingInspactor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Apply", GUILayout.MaxWidth(60f), GUILayout.MaxHeight(25f)))
            {
                GenerateCodeSourcesEditor.GenerateCodeSources();
            }
            base.OnInspectorGUI();
        }
    }
}