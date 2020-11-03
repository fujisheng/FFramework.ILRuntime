using ILRuntime.Runtime.Enviorment;

namespace Framework.IL.Module.Script
{
    public interface IDelegateConvertor
    {
        void Convert(AppDomain domain);
    }
}