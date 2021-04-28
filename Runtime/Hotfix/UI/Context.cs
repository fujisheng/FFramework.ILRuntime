using Cysharp.Threading.Tasks;
using Framework.ILR.Utility;
using Framework.Service.Resource;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.ILR.Service.UI
{
    public class Context
    {
        internal IViewModel ViewModel { get; private set; }
        internal IView View { get; private set; }
        internal IResourceLoader ResourceLoader { get; private set; }

        Dictionary<string, IBindableProperty> propertyCache;
        (Type viewModelType, string assetName, int layer, int flag) bindInfo;
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

        internal Context(IViewModel viewModel, IView view, IResourceLoader loader, (Type viewModelType, string assetName, int layer, int flag) bindInfo)
        {
            this.ViewModel = viewModel;
            this.View = view;
            ResourceLoader = loader;
            this.bindInfo = bindInfo;
            propertyCache = new Dictionary<string, IBindableProperty>();
            view.Initialize();
        }

        /// <summary>
        /// 释放这个Context
        /// </summary>
        public void Release()
        {
            Debug.Log("Releasesssssssssssssssssssssssssssssssssssssssssssssss");
        }

        /// <summary>
        /// 创建View
        /// </summary>
        /// <returns></returns>
        public async UniTask<IView> CreateView()
        {
            var assetName = bindInfo.assetName ?? View.ViewName;
            var viewObj = await ResourceLoader.InstantiateAsync(assetName);
            Object.DontDestroyOnLoad(viewObj);
            View.OnCreate(viewObj, this);
            return View;
        }

        /// <summary>
        /// 展示View
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public async UniTask ShowView(object param)
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
        /// 获取所有的BindableProperty
        /// </summary>
        /// <param name="withCached">是否缓存</param>
        /// <returns></returns>
        List<(string propertyName, IBindableProperty property)> GetPropertys(bool withCached = true)
        {
            if(ViewModel == null)
            {
                throw new Exception($"can not get propertys, please bind viewModel first");
            }

            var fieldInfos = ViewModel.GetType().GetFields(flags);
            if(fieldInfos == null)
            {
                return null;
            }

            List<(string propertyName, IBindableProperty property)> result = new List<(string propertyName, IBindableProperty property)>();

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                var fieldInfo = fieldInfos[i];
                bool get = propertyCache.TryGetValue(fieldInfo.Name, out IBindableProperty field);
                if (!get)
                {
                    if (fieldInfo == null || !typeof(IBindableProperty).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        continue;
                    }

                    field = fieldInfo.GetValue(ViewModel) as IBindableProperty;
                    propertyCache.Add(fieldInfo.Name, field);
                    result.Add((fieldInfo.Name, field));
                }
                else if(withCached)
                {
                    result.Add((fieldInfo.Name, field));
                }
            }
            return result;
        }

        /// <summary>
        /// 获取某个特定的属性 当缓存中有的时候就直接拿出来  没有的话就通过反射获取
        /// </summary>
        /// <param name="propertyName">属性的名字</param>
        /// <returns></returns>
        IBindableProperty GetProperty(string propertyName)
        {
            bool get = propertyCache.TryGetValue(propertyName, out IBindableProperty field);
            if (!get)
            {
                if (ViewModel == null)
                {
                    throw new Exception($"can not get property {propertyName}, please bind viewModel first");
                }

                var fieldInfo = ViewModel.GetType().GetField(propertyName, flags);
                if (fieldInfo == null || ! typeof(IBindableProperty).IsAssignableFrom(fieldInfo.FieldType))
                {
                    throw new Exception($"can not get property {propertyName} with {ViewModel.GetType().FullName}, please check property name is right");
                }

                field = fieldInfo.GetValue(ViewModel) as IBindableProperty;
                if(field != null)
                {
                    propertyCache.Add(propertyName, field);
                }
            }
            return field;
        }

        /// <summary>
        /// 通过指定类型和名字获取对应的属性
        /// </summary>
        /// <typeparam name="T">属性值的类型</typeparam>
        /// <param name="propertyName">属性的名字</param>
        /// <returns></returns>
        BindableProperty<T> GetProperty<T>(string propertyName)
        {
            var property = GetProperty(propertyName);
            if(!(property is BindableProperty<T>))
            {
                throw new Exception($"{propertyName} is {property.GetType().FullName} not {typeof(BindableProperty<T>)}");
            }
            return (BindableProperty<T>)property;
        }

        /// <summary>
        /// 一键绑定所有的Property
        /// </summary>
        /// <param name="bindView">是否绑定View</param>
        /// <param name="bindAlreadyBinded">是否绑定已经绑定的 请注意如果已经绑定过的再绑定到同一个listener的话 每次改变这个listener将会执行多次</param>
        public void BindPropertys(object target, bool withAlreadyBinded = false)
        {
            var propertys = GetPropertys(withAlreadyBinded);
            for(int i = 0; i < propertys.Count; i++)
            {
                var property = propertys[i];
                var addMethodInfo = property.property.GetType().GetMethod("AddListener", flags);
                string listenerMethodName = Framework.Utility.String.GetOrCombine("OnChanged_", property.propertyName);
                var listenerMethodInfo = target.GetType().GetMethod(listenerMethodName, flags);
                if(addMethodInfo == null || listenerMethodInfo == null)
                {
                    continue;
                }

                addMethodInfo.Invoke(property.property, new object[] {target, listenerMethodInfo});
            }
        }

        /// <summary>
        /// 手动绑定一个方法 需要手动移除绑定
        /// </summary>
        /// <typeparam name="T">这个属性的值的类型</typeparam>
        /// <param name="propertyName">对应ViewModel中的BindableProperty的名字</param>
        /// <param name="listener">对应的OnChanged方法</param>
        public void Bind<T>(string propertyName, Action<T, T> listener)
        {
            var property = GetProperty<T>(propertyName);
            property.AddListener(listener);
        }

        /// <summary>
        /// 通过BindProperty这个特性来绑定对应的Property到某个方法
        /// </summary>
        /// <param name="target">要绑定的目标</param>
        public void BindWithAttribute(object target)
        {
            var methodInfos = target.GetType().GetMethods(flags);
            if(methodInfos == null)
            {
                return;
            }

            for(int i = 0; i < methodInfos.Length; i++)
            {
                var methodInfo = methodInfos[i];
                var bindInfo = TypeUtility.GetCustomAttribute<BindProperty>(methodInfo, true);

                if(bindInfo == null)
                {
                    continue;
                }
                var property = GetProperty(bindInfo.PropertyName);
                var addMethodInfo = property.GetType().GetMethod("AddListener", flags, null, new Type[] {typeof(object), typeof(MethodInfo)}, null);
                if (addMethodInfo == null)
                {
                    continue;
                }

                addMethodInfo.Invoke(property, new object[] { target, methodInfo });
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
            var property = GetProperty<T>(propertyName);
            property.RemoveListener(listener);
        }

        /// <summary>
        /// 给对应名字的BindableProperty赋值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="propertyName">对应ViewModel中的BindableProperty的名字</param>
        /// <param name="value">值</param>
        /// <param name="force">是否强制触发Listener而无论是否和上次的值一样</param>
        public void SetValue<T>(string propertyName, T value, bool force = false)
        {
            var property = GetProperty<T>(propertyName);
            property.SetValue(value, force);
        }

        /// <summary>
        /// 获取对应名字的BindableProperty的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="propertyName">对应ViewModel中的BindableProperty的名字</param>
        /// <returns>对应的值</returns>
        public T GetValue<T>(string propertyName)
        {
            var property = GetProperty<T>(propertyName);
            return property.Value;
        }
    }
}