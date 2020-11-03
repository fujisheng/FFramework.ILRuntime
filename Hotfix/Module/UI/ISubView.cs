namespace Framework.IL.Hotfix.Module.UI
{
    public interface ISubView : IView
    {
        IView parentView { get; set; }
    }
}
