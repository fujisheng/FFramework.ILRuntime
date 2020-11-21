﻿using Framework.Module.Resource;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Framework.IL.Hotfix.Module.UI
{
    public static class Contexts
    {
        static List<Context> contextCache = new List<Context>();
        static Dictionary<Type, IViewModel> viewModelCache = new Dictionary<Type, IViewModel>();

        /// <summary>
        /// 初始化 直接初始化所有的ViewModel
        /// </summary>
        public static void Init()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsAssignableFrom(typeof(IViewModel)) && !type.IsAbstract && type.IsClass)
                {
                    var viewModel = Activator.CreateInstance(type) as IViewModel;
                    viewModel.Init();
                    viewModelCache.Add(type, viewModel);
                }
            }
        }

        public static Context GetOrCreate<TViewModel, TView>() where TViewModel : IViewModel where TView : IView
        {
            var viewModelType = typeof(TViewModel);
            var viewType = typeof(TView);

            foreach (var context in contextCache)
            {
                if (context.viewModel.GetType() == viewModelType && context.view.GetType() == viewType)
                {
                    return context;
                }
            }

            bool get = viewModelCache.TryGetValue(viewModelType, out IViewModel viewModel);
            if (!get)
            {
                viewModel = Activator.CreateInstance(viewModelType) as IViewModel;
            }

            var newContext = new Context((TViewModel)viewModel, ResourceLoader.Ctor());
            return newContext;
        }
    }
}
