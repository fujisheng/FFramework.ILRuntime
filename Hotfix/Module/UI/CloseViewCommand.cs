using Cysharp.Threading.Tasks;
using Framework.Utility;

namespace Framework.IL.Hotfix.Module.UI
{
    class CloseViewCommand : ICommand
    {
        string viewName;
        BindInfo info;
        object param;

        IViewModel viewModel;
        IView view;

        public CloseViewCommand(string viewName, object param)
        {
            this.viewName = viewName;
            this.param = param;

            info = default;
            viewModel = null;
            view = null;
        }

        public async UniTask Execute()
        {
            view = UICache.GetRegistedView(viewName);
            if(view == null)
            {
                return;
            }

            UICache.UnregistView(viewName);
            info = UICache.GetBindInfo(viewName);
            viewModel = UICache.GetOrCreateViewModel(info.viewModelName);
            await view.Closeing();
            string viewModelTypeName = viewModel.GetType().FullName;
            string functionName = StringUtility.GetOrAttach("OnClose_", viewName);
            //MethodInvoker.Invoke(viewModelTypeName, functionName, viewModel, param);
            view.OnClose(param);

            //先直接删除
            view.OnDestroy();
            UICache.DestroyViewInstance(viewName);
            UICache.DestroyViewGameObject(viewName);
        }
    }
}
