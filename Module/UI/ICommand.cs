using Cysharp.Threading.Tasks;

namespace Framework.IL.Hotfix.Module.UI
{
    interface ICommand
    {
        UniTask Execute();
    }
}
