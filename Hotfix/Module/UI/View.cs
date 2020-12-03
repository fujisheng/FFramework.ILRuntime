using Cysharp.Threading.Tasks;
using Framework.Module.Resource;
using Hotfix.Utility;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    public abstract partial class View : IView
    {
        string _viewName = null;
        public string ViewName{ get { if (_viewName == null) _viewName = GetType().Name; return _viewName; }}
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }
        protected Context Context { get; private set; }
        protected IResourceLoader ResourceLoader { get; private set; }

        public virtual void Initialize(){}

        public void OnCreate(GameObject gameObject, Context context)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;
            Context = context;
            ResourceLoader = context.ResourceLoader;
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
            ResourceLoader.Release();
        }
    }
}
