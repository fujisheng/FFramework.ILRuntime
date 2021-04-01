using Framework.Utility;

namespace Framework.ILR.Module.UI
{
    public class SubView : View, ISubView
    {
        public IView parentView { get; set; }

        protected void SendMessageToRoot(string message, object args)
        {
            string parentTypeName = parentView.GetType().FullName;
            string functionName = StringUtility.GetOrAttach("OnSubMessage_", message);
            //MethodInvoker.Invoke(parentTypeName, functionName, parentView, args);
        }
    }
}
