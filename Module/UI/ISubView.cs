namespace Framework.ILR.Module.UI
{
    public interface ISubView : IView
    {
        IView parentView { get; set; }
    }
}
