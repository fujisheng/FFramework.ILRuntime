using Cysharp.Threading.Tasks;

namespace Framework.ILR.Service.UI
{
    interface ICommand
    {
        UniTask Execute();
    }
}
