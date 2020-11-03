using Framework.Module.Resource;
using System.Threading.Tasks;

namespace Framework.IL.Hotfix.Module.UI
{
    public interface IView : IBehaviour
    {
        string viewName { get; }

        //View的生命周期
        void Init();
        void SetResourcesLoader(IResourceLoader resourceLoader);
        void BindViewModel(IViewModel viewModel);
        Task Opening();
        void OnOpen(object param);
        void OnFrontground();
        void OnBackground();
        Task Closeing();
        void OnClose(object param);
        void UnbindViewModel(IViewModel viewModel);
    }
}
