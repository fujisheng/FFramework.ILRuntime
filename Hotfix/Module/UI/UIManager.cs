using System.Collections.Generic;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    public class UIManager : ModuleBase, IUIManager
    {
        Queue<ICommand> commandQueue = new Queue<ICommand>();
        bool currentComplete = true;
        readonly string DefaultView = "StartView";

        public override void OnInit()
        {
            base.OnInit();
            Open(DefaultView);
        }

        public override async void OnUpdate()
        {
            //确保有序执行
            if (commandQueue.Count == 0 || currentComplete == false)
            {
                return;
            }

            currentComplete = false;
            await commandQueue.Dequeue().Execute();
            currentComplete = true;
        }

        public void Open(string viewName, object args = null)
        {
            //这儿之所以不等待是因为在openViewCommand中会判断 但是这儿之所以添加 这一句是为了在同时点击了多个ui的时候 可以异步加载其他的
            UICache.GetOrCreateViewGameObject(viewName);
            OpenViewCommand command = new OpenViewCommand(viewName, args);
            commandQueue.Enqueue(command);
        }

        public void Close(string viewName, object args = null)
        {
            var bindInfo = UICache.GetBindInfo(viewName);
            if(bindInfo.layer != Layer.TOP)
            {
                Debug.LogWarning($"{viewName}是需要入栈的view 请使用Back方法！！！");
                return;
            }
            CloseViewCommand command = new CloseViewCommand(viewName, args);
            commandQueue.Enqueue(command);
        }

        public void Back(int backCount = 1, object args = null)
        {
            
        }
    }
}
