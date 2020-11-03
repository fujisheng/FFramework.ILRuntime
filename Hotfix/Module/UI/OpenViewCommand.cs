using Framework.Module.Resource;
using Framework.Utility;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    class OpenViewCommand : ICommand
    {
        string viewName;
        Bind info;
        object param;

        IViewModel viewModel;
        IView view;

        public OpenViewCommand(string viewName, object param)
        {
            this.viewName = viewName;
            this.param = param;

            info = null;
            viewModel = null;
            view = null;
        }

        public async Task Execute()
        {
            //判断是否已经打开 已经打开的话直接置顶
            info = UICache.GetBindInfo(viewName);
            view = UICache.GetRegistedView(viewName);
            if (view != null)
            {
                UICache.FrontgroundView(viewName);
                SetLayer();
                return;
            }

            //没有打开就创建新的
            view = UICache.GetOrCreateViewInstance(viewName);
            viewModel = UICache.GetOrCreateViewModel(info.viewModelName);
            UICache.RegistView(viewName, view);
            view.SetResourcesLoader(ResourceLoader.Ctor());
            view.Init();
            GameObject gameObject = await UICache.GetOrCreateViewGameObject(viewName);
            view.OnCreated(gameObject);
            SetLayer();
            gameObject.SetActive(true);
            viewModel.BindView(view);
            view.BindViewModel(viewModel);
            await view.Opening();
            view.OnOpen(param);
            string viewModelTypeName = viewModel.GetType().FullName;
            string functionName = StringUtility.GetOrAttach("OnOpen_", viewName);
            //MethodInvoker.Invoke(viewModelTypeName, functionName, viewModel, param);
            UICache.FrontgroundView(viewName);
        }

        void SetLayer()
        {
            Canvas canvas = view.gameObject.GetComponent<Canvas>();
            if(info.layer == Layer.NORMAL || info.layer == Layer.POPUP)
            {
                ViewRecording topView = UICache.GetTopView();
                int layer = topView.IsEmpty() ? 0 : topView.layer + 1;
                bool isFull = info.layer == Layer.NORMAL;
                canvas.sortingOrder = layer;
                UICache.PushView(viewName, layer, isFull, param);
            }
            else if(info.layer == Layer.TOP)
            {
                ViewRecording topView = UICache.GetTopFreeView();
                int layer = topView.IsEmpty() ? Layer.TOP : topView.layer + 1;
                canvas.sortingOrder = layer;
                UICache.PushFreeView(viewName, layer, param);
            }
            else
            {
                canvas.sortingOrder = info.layer;
            }
        }
    }
}
