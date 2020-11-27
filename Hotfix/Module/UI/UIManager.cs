using System.Collections.Generic;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    public class UIManager : ModuleBase, IUIManager
    {
        Queue<ICommand> commandQueue = new Queue<ICommand>();

        public override async void OnUpdate()
        {
            
        }

        public void Open(string viewName, object args = null)
        {
            
        }

        public void Close(string viewName, object args = null)
        {
            
        }

        public void Back(int backCount = 1, object args = null)
        {
            
        }
    }
}
