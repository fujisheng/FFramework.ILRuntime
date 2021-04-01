//using System;
//using ILRuntime.CLR.Method;
//using ILRuntime.Runtime.Enviorment;
//using ILRuntime.Runtime.Intepreter;

//namespace Framework.ILR.Module.Script.Adaptor
//{   
//    public class ViewAdapter : CrossBindingAdaptor
//    {
//        static CrossBindingFunctionInfo<System.String> mget_ViewName_0 = new CrossBindingFunctionInfo<System.String>("get_ViewName");
//        static CrossBindingFunctionInfo<UnityEngine.GameObject> mget_gameObject_1 = new CrossBindingFunctionInfo<UnityEngine.GameObject>("get_gameObject");
//        static CrossBindingFunctionInfo<UnityEngine.Transform> mget_transform_2 = new CrossBindingFunctionInfo<UnityEngine.Transform>("get_transform");
//        static CrossBindingMethodInfo mInitialize_3 = new CrossBindingMethodInfo("Initialize");
//        static CrossBindingMethodInfo<UnityEngine.GameObject, Framework.ILR.Module.UI.Context> mOnCreate_4 = new CrossBindingMethodInfo<UnityEngine.GameObject, Framework.ILR.Module.UI.Context>("OnCreate");
//        static CrossBindingFunctionInfo<Cysharp.Threading.Tasks.UniTask> mOpening_5 = new CrossBindingFunctionInfo<Cysharp.Threading.Tasks.UniTask>("Opening");
//        static CrossBindingMethodInfo<System.Object> mOnOpen_6 = new CrossBindingMethodInfo<System.Object>("OnOpen");
//        static CrossBindingMethodInfo mOnFrontground_7 = new CrossBindingMethodInfo("OnFrontground");
//        static CrossBindingMethodInfo mOnBackground_8 = new CrossBindingMethodInfo("OnBackground");
//        static CrossBindingFunctionInfo<Cysharp.Threading.Tasks.UniTask> mCloseing_9 = new CrossBindingFunctionInfo<Cysharp.Threading.Tasks.UniTask>("Closeing");
//        static CrossBindingMethodInfo<System.Object> mOnClose_10 = new CrossBindingMethodInfo<System.Object>("OnClose");
//        static CrossBindingMethodInfo mOnDestroy_11 = new CrossBindingMethodInfo("OnDestroy");
//        static CrossBindingFunctionInfo<System.Collections.Generic.Dictionary<System.String, System.Int32>> mget_LocalizationMap_12 = new CrossBindingFunctionInfo<System.Collections.Generic.Dictionary<System.String, System.Int32>>("get_LocalizationMap");
//        public override Type BaseCLRType
//        {
//            get
//            {
//                return typeof(Framework.ILR.Module.UI.View);
//            }
//        }

//        public override Type AdaptorType
//        {
//            get
//            {
//                return typeof(Adapter);
//            }
//        }

//        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
//        {
//            return new Adapter(appdomain, instance);
//        }

//        public class Adapter : Framework.ILR.Module.UI.View, CrossBindingAdaptorType
//        {
//            ILTypeInstance instance;
//            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

//            public Adapter()
//            {

//            }

//            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
//            {
//                this.appdomain = appdomain;
//                this.instance = instance;
//            }

//            public ILTypeInstance ILInstance { get { return instance; } }

//            public override void Initialize()
//            {
//                if (mInitialize_3.CheckShouldInvokeBase(this.instance))
//                    base.Initialize();
//                else
//                    mInitialize_3.Invoke(this.instance);
//            }

//            public  void OnCreate(UnityEngine.GameObject gameObject, Framework.ILR.Module.UI.Context context)
//            {
//                //if (mOnCreate_4.CheckShouldInvokeBase(this.instance))
//                //    base.OnCreate(gameObject, context);
//                //else
//                    mOnCreate_4.Invoke(this.instance, gameObject, context);
//            }

//            public override Cysharp.Threading.Tasks.UniTask Opening()
//            {
//                if (mOpening_5.CheckShouldInvokeBase(this.instance))
//                    return base.Opening();
//                else
//                    return mOpening_5.Invoke(this.instance);
//            }

//            public override void OnOpen(System.Object param)
//            {
//                if (mOnOpen_6.CheckShouldInvokeBase(this.instance))
//                    base.OnOpen(param);
//                else
//                    mOnOpen_6.Invoke(this.instance, param);
//            }

//            public override void OnFrontground()
//            {
//                if (mOnFrontground_7.CheckShouldInvokeBase(this.instance))
//                    base.OnFrontground();
//                else
//                    mOnFrontground_7.Invoke(this.instance);
//            }

//            public override void OnBackground()
//            {
//                if (mOnBackground_8.CheckShouldInvokeBase(this.instance))
//                    base.OnBackground();
//                else
//                    mOnBackground_8.Invoke(this.instance);
//            }

//            public override Cysharp.Threading.Tasks.UniTask Closeing()
//            {
//                if (mCloseing_9.CheckShouldInvokeBase(this.instance))
//                    return base.Closeing();
//                else
//                    return mCloseing_9.Invoke(this.instance);
//            }

//            public override void OnClose(System.Object param)
//            {
//                if (mOnClose_10.CheckShouldInvokeBase(this.instance))
//                    base.OnClose(param);
//                else
//                    mOnClose_10.Invoke(this.instance, param);
//            }

//            public override void OnDestroy()
//            {
//                if (mOnDestroy_11.CheckShouldInvokeBase(this.instance))
//                    base.OnDestroy();
//                else
//                    mOnDestroy_11.Invoke(this.instance);
//            }

//            public System.String ViewName
//            {
//            get
//            {
//                //if (mget_ViewName_0.CheckShouldInvokeBase(this.instance))
//                //    return base.ViewName;
//                //else
//                    return mget_ViewName_0.Invoke(this.instance);

//            }
//            }

//            public UnityEngine.GameObject gameObject
//            {
//            get
//            {
//                //if (mget_gameObject_1.CheckShouldInvokeBase(this.instance))
//                //    return base.gameObject;
//                //else
//                    return mget_gameObject_1.Invoke(this.instance);

//            }
//            }

//            public UnityEngine.Transform transform
//            {
//            get
//            {
//                //if (mget_transform_2.CheckShouldInvokeBase(this.instance))
//                //    return base.transform;
//                //else
//                    return mget_transform_2.Invoke(this.instance);

//            }
//            }

//            protected override System.Collections.Generic.Dictionary<System.String, System.Int32> LocalizationMap
//            {
//            get
//            {
//                if (mget_LocalizationMap_12.CheckShouldInvokeBase(this.instance))
//                    return base.LocalizationMap;
//                else
//                    return mget_LocalizationMap_12.Invoke(this.instance);

//            }
//            }

//            public override string ToString()
//            {
//                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
//                m = instance.Type.GetVirtualMethod(m);
//                if (m == null || m is ILMethod)
//                {
//                    return instance.ToString();
//                }
//                else
//                    return instance.Type.FullName;
//            }
//        }
//    }
//}

