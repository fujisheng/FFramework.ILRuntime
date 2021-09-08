using Framework.Service.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewConfig = System.ValueTuple<string, int, int>;

namespace Framework.ILR.Service.UI
{
    public class CommonContext : Context<IView, IViewModel>
    {
        internal CommonContext(IView view, IViewModel viewModel, IResourceLoader loader, ViewConfig config) : base(view, viewModel, loader, config) { }
    }
}
