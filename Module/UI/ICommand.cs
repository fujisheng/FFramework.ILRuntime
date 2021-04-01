using Cysharp.Threading.Tasks;

namespace Framework.ILR.Module.UI
{
    interface ICommand
    {
        UniTask Execute();
    }
}
