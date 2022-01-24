using FInject;
using Framework.ILR.Utility;
using Framework.Service.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;
using ViewConfig = System.ValueTuple<string, int, int>;

namespace Framework.ILR.Service.UI
{
    public static class Contexts
    {
        static readonly Dictionary<Type, IViewModel> viewModelCache = new Dictionary<Type, IViewModel>();
        static readonly Dictionary<Type, Type> bindInfoCache = new Dictionary<Type, Type>();
        static readonly Dictionary<Type, ViewConfig> viewConfigCache = new Dictionary<Type, ViewConfig>();

        /// <summary>
        /// 初始化 直接初始化所有继承自PerloadViewModel的ViewModel
        /// </summary>
        public static void Initialize(string[] types)
        {
            foreach(var typeName in types)
            {
                var type = Type.GetType(typeName);
                if(type == null)
                {
                    continue;
                }
                if (type.Is<IViewModel>() && type.Is<IPerloadViewModel>() && !type.IsAbstract && type.IsClass)
                {
                    var viewModel = Activator.CreateInstance(type) as IViewModel;
                    Debug.Log($"<color=blue>{type.FullName} is perloaded</color>");
                    viewModel.Initialize();
                    viewModelCache.Add(type, viewModel);
                }
            }
        }

        /// <summary>
        /// 获取ViewModel
        /// </summary>
        /// <param name="viewModelType">viewModelType</param>
        /// <returns></returns>
        public static IViewModel GetViewModel(Type viewModelType)
        {
            UnityEngine.Debug.Log(viewModelType);
            if (!viewModelType.Is<IViewModel>())
            {
                throw new Exception($"get ViewModel failure, {viewModelType.FullName} is not IViewModel");
            }

            if(!viewModelCache.TryGetValue(viewModelType, out IViewModel viewModel))
            {
                viewModel = Activator.CreateInstance(viewModelType) as IViewModel;
                viewModel.Initialize();
                viewModelCache.Add(viewModelType, viewModel);
            }

            return viewModel;
        }

        /// <summary>
        /// 获取ViewModel
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        public static TViewModel GetViewModel<TViewModel>() where TViewModel : IViewModel
        {
            return (TViewModel)GetViewModel(typeof(TViewModel));
        }

        /// <summary>
        /// 获取Bind信息
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns></returns>
        static Type GetBindingViewModelType(Type viewType)
        {
            if (!viewType.Is<IView>())
            {
                throw new Exception($"get bind info failure, {viewType.FullName} is not {typeof(IView).FullName}");
            }

            if(bindInfoCache.TryGetValue(viewType, out Type viewModelType))
            {
                return viewModelType;
            }

            var bind = viewType.GetHotfixCustomAttribute<BindingAttribute>(true);
            if(bind == null)
            {
                throw new Exception($"get bind info failure, {viewType.FullName} dont have attribute [{typeof(BindingAttribute).FullName}]");
            }

            //这儿之所以这样是因为在ILRuntime中Attribute只支持基本类型 其它类型会报错
            viewModelType = TypeUtility.GetType(bind.viewModelType.ToString());
            UnityEngine.Debug.Log(bind.viewModelType);
            if (!bind.viewModelType.Is<IViewModel>())// viewModelType.Is<IViewModel>())
            {
                throw new Exception($"get bind info failure, {viewType.FullName} [{typeof(BindingAttribute).FullName}].ViewModelType [{bind.viewModelType}] is not IViewModel");
            }
            bindInfoCache.Add(viewType, bind.viewModelType);// viewModelType);
            return bind.viewModelType;// viewModelType;
        }

        /// <summary>
        /// 获取View的配置信息
        /// </summary>
        /// <param name="viewType">view类型</param>
        /// <returns></returns>
        static (string assetName, int layer, int flag) GetViewConfig(Type viewType)
        {
            if (!viewType.Is<IView>())
            {
                throw new Exception($"get view config failure, {viewType.FullName} is not {typeof(IView).FullName}");
            }

            if(viewConfigCache.TryGetValue(viewType, out var config))
            {
                return config;
            }

            var configInfo = viewType.GetHotfixCustomAttribute<ConfigAttribute>(true);
            config = configInfo == null ? default : configInfo.ToTuple();
            viewConfigCache.Add(viewType, config);
            return config;
        }

        /// <summary>
        /// 创建Context
        /// </summary>
        /// <param name="viewModelType">ViewModel的类型</param>
        /// <param name="viewType">View的类型</param>
        /// <returns>context</returns>
        public static IContext Create(Type viewModelType, Type viewType)
        {
            var viewModel = GetViewModel(viewModelType);
            if (!viewType.Is<IView>())
            {
                throw new Exception($"create context failure, {viewType.FullName} is not {typeof(IView).FullName}");
            }
            var view = Activator.CreateInstance(viewType) as IView;
            var resourceLoader = Bootstrapper.CreateInstance<ResourceLoader>();
            var viewConfig = GetViewConfig(viewType);
            var contextType = TypeUtility.GetType($"Game.Hotfix.{viewType.Name}_{viewModelType.Name}_Context");
            if(contextType != null)
            {
                return (IContext)Activator.CreateInstance(contextType, new object[] { view, viewModel, resourceLoader, viewConfig });
            }
            return new Context<IView, IViewModel>(view, viewModel, resourceLoader, viewConfig);
        }

        /// <summary>
        /// 创建Context
        /// </summary>
        /// <typeparam name="TViewModel">viewModel Type</typeparam>
        /// <typeparam name="TView">view Type</typeparam>
        /// <returns>context</returns>
        public static IContext Create<TViewModel, TView>() where TViewModel : IViewModel where TView : IView
        {
            return Create(typeof(TViewModel), typeof(TView));
        }

        /// <summary>
        /// 创建Context
        /// </summary>
        /// <param name="viewType">View的类型</param>
        /// <returns>context</returns>
        public static IContext Create(Type viewType)
        {
            var viewModelType = GetBindingViewModelType(viewType);
            return Create(viewModelType, viewType);
        }

        /// <summary>
        /// 创建Context
        /// </summary>
        /// <typeparam name="TView">View</typeparam>
        /// <returns>context</returns>
        public static IContext Create<TView>() where TView : IView
        {
            return Create(typeof(TView));
        }
    }
}
