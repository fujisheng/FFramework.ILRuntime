using Cysharp.Threading.Tasks;
using Framework.ILR.Utility;
using Framework.Service.Resource;
using System;
using Object = UnityEngine.Object;

namespace Framework.ILR.Service.UI
{
    using ViewConfig = System.ValueTuple<string, int, int>;

    public class Context<TView, TViewModel> : IContext where TView : IView where TViewModel : IViewModel
    {
        public TViewModel ViewModel { get; private set; }
        public TView View { get; private set; }
        public IResourceLoader ResourceLoader { get; private set; }

        internal (string assetName, int layer, int flag) Config;

        public Context(TView view, TViewModel viewModel, IResourceLoader loader, ViewConfig config)
        {
            UnityEngine.Debug.Log("Context<>Ctor");
            ViewModel = viewModel;
            View = view;
            ResourceLoader = loader;
            Config = config;
            view.Initialize();
        }

        internal void Release()
        {
            ResourceLoader.Release();
        }

        public async UniTask<IView> CreateView()
        {
            var assetName = Config.assetName ?? View.ViewName;
            var viewObj = await ResourceLoader.InstantiateAsync(assetName);
            Object.DontDestroyOnLoad(viewObj);
            View.OnCreate(viewObj, this);
            return View;
        }

        internal async UniTask ShowView(object param)
        {
            View.gameObject.SetActive(true);
            await View.Opening();
            View.OnOpen(param);
            var methodInfo = ViewModel.GetType().GetMethod(Framework.Utility.String.GetOrCombine("OnOpen_", View.ViewName));
            if (methodInfo != null)
            {
                methodInfo.Invoke(ViewModel, new object[] { param });
            }
        }

        /// <summary>
        /// 隐藏View
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async UniTask HideView(object param)
        {
            View.gameObject.SetActive(false);
            await View.Closeing();
            View.OnClose(param);
        }

        /// <summary>
        /// 获取某个特定的属性 当缓存中有的时候就直接拿出来  没有的话就通过反射获取
        /// </summary>
        /// <param name="propertyName">属性的名字</param>
        /// <returns></returns>
        protected virtual IBindableProperty GetBindableProperty(string propertyName)
        {
            if (ViewModel == null)
            {
                throw new Exception($"can not get property {propertyName}, please bind viewModel first");
            }

            return BindablePropertyUtility.GetBindableProperty(ViewModel, propertyName);
        }

        /// <summary>
        /// 通过指定类型和名字获取对应的属性
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="propertyName">属性的名字</param>
        /// <returns></returns>
        protected virtual BindableProperty<T> GetBindableProperty<T>(string propertyName)
        {
            var property = GetBindableProperty(propertyName);
            if (!(property is BindableProperty<T>))
            {
                throw new Exception($"{propertyName} is {property.GetType().FullName} not {typeof(BindableProperty<T>)}");
            }
            return (BindableProperty<T>)property;
        }

        /// <summary>
        /// 手动绑定一个方法 需要手动移除绑定
        /// </summary>
        /// <typeparam name="T">这个属性的值的类型</typeparam>
        /// <param name="propertyName">对应ViewModel中的BindableProperty的名字</param>
        /// <param name="listener">对应的OnChanged方法</param>
        public void Binding<T>(string propertyName, Action<T, T> listener)
        {
            var property = GetBindableProperty<T>(propertyName);
            property.AddListener(listener);
        }

        /// <summary>
        /// 通过OnValueChanged这个特性来绑定对应的Property到某个方法
        /// </summary>
        /// <param name="target">要绑定的目标</param>
        public virtual void BindingWithAttribute()
        {
            var methodInfos = View.GetType().GetMethods(TypeUtility.allFlags);
            if (methodInfos == null)
            {
                return;
            }

            for (int i = 0; i < methodInfos.Length; i++)
            {
                var methodInfo = methodInfos[i];
                var bindInfo = methodInfo.GetHotfixCustomAttribute<OnValueChangedAttribute>(true);

                if (bindInfo == null)
                {
                    continue;
                }
                var property = GetBindableProperty(bindInfo.propertyName);
                var addMethodInfo = property.GetType().GetMethod("AddListener", TypeUtility.allFlags, null, BindablePropertyUtility.AddWithMethodInfoParamType, null);
                if (addMethodInfo == null)
                {
                    continue;
                }

                addMethodInfo.Invoke(property, new object[] { View, methodInfo });
            }
        }

        /// <summary>
        /// 手动解绑一个方法
        /// </summary>
        /// <typeparam name="T">这个属性的值的类型</typeparam>
        /// <param name="propertyName">对应ViewModel中的BindableProperty的名字</param>
        /// <param name="listener">对应的OnChanged方法</param>
        public void Unbind<T>(string propertyName, Action<T, T> listener)
        {
            var property = GetBindableProperty<T>(propertyName);
            property.RemoveListener(listener);
        }

        /// <summary>
        /// 给对应名字的BindableProperty赋值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="propertyName">对应ViewModel中的BindableProperty的名字</param>
        /// <param name="value">值</param>
        /// <param name="force">是否强制触发Listener而无论是否和上次的值一样</param>
        public void SetProperty<T>(string propertyName, T value, bool force = false)
        {
            var property = GetBindableProperty<T>(propertyName);
            property.SetValue(value, force);
        }

        /// <summary>
        /// 获取对应名字的BindableProperty的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="propertyName">对应ViewModel中的BindableProperty的名字</param>
        /// <returns>对应的值</returns>
        public T GetProperty<T>(string propertyName)
        {
            var property = GetBindableProperty<T>(propertyName);
            return property.Value;
        }
    }
}
