using Framework.Utility;
using System.Collections.Generic;

namespace Framework.IL.Hotfix.Module.UI
{
    public abstract class ViewModel : IViewModel
    {
        Dictionary<string, IView> bindedView = new Dictionary<string, IView>();

        public virtual void Init() { }

        public virtual void BindProperty()
        {
            string typeName = GetType().FullName;
            //PropertyBinder.Bind(typeName, this, typeName, this);
        }

        public void BindView(IView view)
        {
            if (!bindedView.ContainsKey(view.viewName))
            {
                bindedView.Add(view.viewName, view);
            }
        }

        public void UnbindView(IView view)
        {
            if (bindedView.ContainsKey(view.viewName))
            {
                bindedView.Remove(view.viewName);
            }
        }

        /// <summary>
        /// 向绑定的view发送通知
        /// </summary>
        /// <param name="notification">通知的名字</param>
        /// <param name="args">参数</param>
        protected void SendNotification(string notification, object args = null)
        {
            foreach (var view in bindedView.Values)
            {
                string viewTypeName = view.GetType().FullName;
                string functionName = StringUtility.GetOrAttach("OnNotification_", notification);
                //MethodInvoker.Invoke(viewTypeName, functionName, view, args);
            }
        }
    }
}
