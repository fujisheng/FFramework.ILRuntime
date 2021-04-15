namespace Framework.ILR.Service.UI
{
    public interface ISubView : IView
    {
        IView parentView { get; set; }
    }
}
