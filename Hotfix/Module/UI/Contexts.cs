using Framework.Module.Resource;
using Framework.Module.Script;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    public static class Contexts
    {
        static Dictionary<Type, IViewModel> viewModelCache = new Dictionary<Type, IViewModel>();
        static Dictionary<string, Type> viewModelTypeCache = new Dictionary<string, Type>();
        static Dictionary<Type, (Type viewModelType, string assetName, int layer, int flag)> bindInfoCache = new Dictionary<Type, (Type viewModelType, string assetName, int layer, int flag)>();

        static bool Is<T>(Type type)
        {
            try
            {
                return typeof(T).IsAssignableFrom(type);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 初始化 直接初始化所有继承自PerloadViewModel的ViewModel
        /// </summary>
        public static void Init(IScriptManager scriptManager)
        {
            foreach(var typeName in scriptManager.Types)
            {
                var type = Type.GetType(typeName);
                if(type == null)
                {
                    continue;
                }

                if (Is<IViewModel>(type) && Is<IPerloadViewModel>(type) && !type.IsAbstract && type.IsClass)
                {
                    var viewModel = Activator.CreateInstance(type) as IViewModel;
                    Debug.Log($"<color=blue>{type.FullName} is perloaded</color>");
                    viewModel.Init();
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
            if (!Is<IViewModel>(viewModelType))
            {
                throw new Exception($"get ViewModel failure, {viewModelType.FullName} is not IViewModel");
            }

            bool get = viewModelCache.TryGetValue(viewModelType, out IViewModel viewModel);
            if (!get)
            {
                viewModel = Activator.CreateInstance(viewModelType) as IViewModel;
                viewModel.Init();
                viewModelCache.Add(viewModelType, viewModel);
            }
            return viewModel;
        }

        /// <summary>
        /// 获取ViewModel
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        public static IViewModel GetViewModel<TViewModel>() where TViewModel : IViewModel
        {
            return GetViewModel(typeof(TViewModel));
        }

        static Type GetViewModelType(string viewModelName)
        {
            bool get = viewModelTypeCache.TryGetValue(viewModelName, out Type type);
            if (get)
            {
                return type;
            }
            type = Type.GetType(viewModelName);
            if(type == null)
            {
                return null;
            }
            viewModelTypeCache.Add(viewModelName, type);
            return type;
        }

        
        /// <summary>
        /// 获取Bind信息
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns></returns>
        public static (Type viewModelType, string assetName, int layer, int flag) GetBindInfo(Type viewType)
        {
            if (!Is<IView>(viewType))
            {
                throw new Exception($"get bind info failure, {viewType.FullName} is not {typeof(IView).FullName}");
            }

            var get = bindInfoCache.TryGetValue(viewType, out (Type viewModelType, string assetName, int layer, int flag) bindInfo);
            if (get)
            {
                return bindInfo;
            }
            var attributes = viewType.GetCustomAttributes(true);
            for (int j = 0; j < attributes.Length; j++)
            {
                var attribute = attributes[j];
                if (!(attribute is Bind))
                {
                    continue;
                }

                var bind = attribute as Bind;
                //这儿之所以这样是因为在ILRuntime中Attribute只支持基本类型 其它类型会报错
                var viewModelType = GetViewModelType(bind.ViewModelType.ToString());
                if (!Is<IViewModel>(viewModelType))
                {
                    throw new Exception($"get bind info failure, {viewType.FullName} [{typeof(Bind).FullName}].ViewModelType [{bind.ViewModelType}] is not IViewModel");
                }
                bindInfo = (viewModelType, bind.AssetName, bind.Layer, bind.Flag);
                bindInfoCache.Add(viewType, bindInfo);
                return bindInfo;
            }
            throw new Exception($"get bind info failure, {viewType.FullName} dont have attribute [{typeof(Bind).FullName}]");
        }

        /// <summary>
        /// 获取Bind信息
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public static (Type viewModelType, string assetName, int layer, int flag) GetBindInfo<TView>() where TView : IView
        {
            return GetBindInfo(typeof(TView));
        }

        /// <summary>
        /// 创建Context
        /// </summary>
        /// <param name="viewModelType">ViewModel的类型</param>
        /// <param name="viewType">View的类型</param>
        /// <returns>context</returns>
        public static Context Create(Type viewModelType, Type viewType)
        {
            var viewModel = GetViewModel(viewModelType);
            if (!Is<IView>(viewType))
            {
                throw new Exception($"create context failure, {viewType.FullName} is not {typeof(IView).FullName}");
            }
            var view = Activator.CreateInstance(viewType) as IView;
            var context = new Context(viewModel, view, new ResourceLoader(), GetBindInfo(viewType));
            return context;
        }

        /// <summary>
        /// 创建Context
        /// </summary>
        /// <typeparam name="TViewModel">viewModel Type</typeparam>
        /// <typeparam name="TView">view Type</typeparam>
        /// <returns>context</returns>
        public static Context Create<TViewModel, TView>() where TViewModel : IViewModel where TView : IView
        {
            return Create(typeof(TViewModel), typeof(TView));
        }

        /// <summary>
        /// 创建Context
        /// </summary>
        /// <param name="viewType">View的类型</param>
        /// <returns>context</returns>
        public static Context Create(Type viewType)
        {
            var bindInfo = GetBindInfo(viewType);
            return Create(bindInfo.viewModelType, viewType);
        }

        /// <summary>
        /// 创建Context
        /// </summary>
        /// <typeparam name="TView">View</typeparam>
        /// <returns>context</returns>
        public static Context Create<TView>() where TView : IView
        {
            return Create(typeof(TView));
        }
    }
}
