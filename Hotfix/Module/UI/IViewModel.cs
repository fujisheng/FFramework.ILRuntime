namespace Framework.IL.Hotfix.Module.UI
{
    public interface IViewModel
    {
        void Init();
        void BindProperty();
        void BindView(IView view);
        void UnbindView(IView view);
    }
}
