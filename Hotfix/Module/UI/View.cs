using Cysharp.Threading.Tasks;
using Framework.Module.Resource;
using Hotfix.Utility;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    public abstract partial class View : IView
    {
        string _viewName = null;
        public string viewName{ get { if (_viewName == null) _viewName = GetType().Name; return _viewName; }}
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }
        protected Context context { get; private set; }
        protected IResourceLoader resourceLoader { get; private set; }

        public virtual void Init(){}

        public void OnCreate(GameObject gameObject, Context context)
        {
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
            this.context = context;
            this.resourceLoader = context.resourceLoader;
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
            resourceLoader.Release();
        }
    }
}
