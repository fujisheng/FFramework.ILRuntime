using System.Collections.Generic;
using UnityEngine;

namespace Framework.ILR.Editor
{
    public class GenerateCodeSourcesEditorSetting : ScriptableObject
    {
        public string ScriptingDefineSymbols = string.Empty;
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
            var symbols = ScriptingDefineSymbols.Split(';');
            var result = new List<string>();
            foreach(var symbol in symbols)
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