using UnityEngine;

namespace Framework.ILR.Service.Script.Editor
{
    public class FrameworkILRuntimeEditorSetting : ScriptableObject
    {
        public string CodeSourcesPath = "Assets/Sources/Code";
        public string DllName = "Game.Hotfix.dll";
        public string PdbName = "Game.Hotfix.pdb";
    }
}