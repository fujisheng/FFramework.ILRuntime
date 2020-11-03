using Hotfix.Utility;
using Framework.Module.Resource;
using Framework.Utility;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    public abstract partial class View : IView
    {
        string _viewName = null;
        public string viewName{ get { if (_viewName == null) _viewName = GetType().Name; return _viewName; }}
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }
        protected IViewModel viewModel { get; private set; }
        protected IResourceLoader resourceLoader { get; private set; }

        public virtual void Init(){}

        public void SetResourcesLoader(IResourceLoader resourceLoader)
        {
            this.resourceLoader = resourceLoader;
        }

        public void OnCreated(GameObject gameObject)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;
        }

        /// <summary>
        /// 绑定viewModel
        /// </summary>
        /// <param name="viewModel"></param>
        public void BindViewModel(IViewModel viewModel)
        {
            if (this.viewModel != null)
            {
                return;
            }
            this.viewModel = viewModel;
        }

        public virtual async Task Opening()
        {
            string viewModelTypeName = viewModel.GetType().FullName;
            string typeName = GetType().FullName;
            //PropertyBinder.Bind(viewModelTypeName,viewModel, typeName, this)
            ViewUtility.AddAllButtonEvent(this);
            await Task.Delay(0);
        }

        public virtual void OnOpen(object param){}

        public virtual void OnFrontground() { }

        public virtual void OnBackground() { }

        public virtual async Task Closeing()
        {
            ViewUtility.RemoveAllButtonEvent(this);
            await Task.Delay(0);
        }

        public virtual void OnClose(object param) { }

        public void UnbindViewModel(IViewModel viewModel)
        {
            this.viewModel = null;
        }

        public virtual void OnDestroy()
        {
            resourceLoader.Release();
        }
    }
}
