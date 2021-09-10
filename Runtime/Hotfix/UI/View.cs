using Cysharp.Threading.Tasks;
using Framework.ILR.Utility;
using Framework.Service.Resource;
using UnityEngine;

namespace Framework.ILR.Service.UI
{
    public abstract partial class View : IView
    {
        string _viewName = null;
        public string ViewName{ get { if (_viewName == null) _viewName = GetType().Name; return _viewName; }}
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }
        protected IContext Context { get; private set; }
        protected IResourceLoader Loader { get; private set; }

        public View() { }
        public virtual void Initialize(){}

        public void OnCreate(GameObject gameObject, IContext context)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;
            Context = context;
            Loader = context.ResourceLoader;
        }

        public virtual async UniTask Opening()
        {
            ViewUtility.AddAllButtonEvent(this);
            await UniTask.Delay(0);
        }

        public virtual void OnOpen(object param){}

        public virtual void OnFrontground() { }

        public virtual void OnBackground() { }

        public virtual async UniTask Closeing()
        {
            ViewUtility.RemoveAllButtonEvent(this);
            await UniTask.Delay(0);
        }

        public virtual void OnClose(object param) { }

        public virtual void OnDestroy()
        {
            Loader.Release();
        }
    }
}
