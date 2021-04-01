using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Framework.ILR.Module.Script.Adaptor
{   
    public class ViewModelAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mInitialize_0 = new CrossBindingMethodInfo("Initialize");
        static CrossBindingMethodInfo mRelease_1 = new CrossBindingMethodInfo("Release");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Framework.ILR.Module.UI.ViewModel);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : Framework.ILR.Module.UI.ViewModel, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void Initialize()
            {
                if (mInitialize_0.CheckShouldInvokeBase(this.instance))
                    base.Initialize();
                else
                    mInitialize_0.Invoke(this.instance);
            }

            public override void Release()
            {
                if (mRelease_1.CheckShouldInvokeBase(this.instance))
                    base.Release();
                else
                    mRelease_1.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

