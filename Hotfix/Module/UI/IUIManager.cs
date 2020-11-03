namespace Framework.IL.Hotfix.Module.UI
{
    public interface IUIManager
    {
        void Open(string viewName, object args = null);
        void Close(string viewName, object args = null);
        void Back(int backCount = 1, object args = null);
    }
}