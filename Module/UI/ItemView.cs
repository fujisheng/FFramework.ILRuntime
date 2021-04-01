namespace Framework.ILR.Module.UI
{
    public class ItemView : SubView, IItemView
    {
        protected object data;

        public virtual void OnChangedData(object data)
        {
            this.data = data;
        }
    }
}