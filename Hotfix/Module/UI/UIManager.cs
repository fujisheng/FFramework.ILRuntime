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

        public async void Open<TView>(object args = null) where TView : IView
        {
            var context = Contexts.Create<TView>();
            var view = await context.CreateView();
            context.BindWithAttribute(view);
            context.SetValue("title", "sssssssssssssssss");
            context.SetValue("age", 10000, true);
        }

        public async void Open<TView, TViewModel>(object args = null) where TView : IView where TViewModel : IViewModel
        {
            var context = Contexts.Create<TViewModel, TView>();
            var view = await context.CreateView();
            context.BindWithAttribute(view);
            context.SetValue("title", "我是通过PublicViewModel 设置的");
            context.SetValue("age", 1000);
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
