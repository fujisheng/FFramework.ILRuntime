using Cysharp.Threading.Tasks;
using Framework.Module.Resource;
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    public class Context
    {
        internal IViewModel viewModel { get; private set; }
        internal IView view { get; private set; }
        internal IResourceLoader resourceLoader { get; private set; }

        Dictionary<string, IBindableProperty> propertyCache;

        internal Context(IViewModel viewModel, IView view, IResourceLoader loader)
        {
            this.viewModel = viewModel;
            this.view = view;
            this.resourceLoader = loader;
            propertyCache = new Dictionary<string, IBindableProperty>();
            this.viewModel.Init();
            this.view.Init();
        }

        public void Release()
        {
            
        }

        public async UniTask<IView> CreateView()
        {
            var viewObj = await resourceLoader.InstantiateAsync(view.viewName);
            GameObject.DontDestroyOnLoad(viewObj);
            view = Activator.CreateInstance(view.GetType()) as IView;
            view.OnCreate(viewObj, this);
            return view;
        }

        //获取所有的BindableProperty
        List<(string propertyName, IBindableProperty property)> GetPropertys(bool withCached = true)
        {
            if(viewModel == null)
            {
                throw new Exception($"can not get propertys, please bind viewModel first");
            }

            var fieldInfos = viewModel.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic);
            if(fieldInfos == null)
            {
                return null;
            }

            List<(string propertyName, IBindableProperty property)> result = new List<(string propertyName, IBindableProperty property)>();

            foreach (var fieldInfo in fieldInfos)
            {
                bool get = propertyCache.TryGetValue(fieldInfo.Name, out IBindableProperty field);
                if (!get)
                {
                    if (fieldInfo == null || !fieldInfo.FieldType.IsAssignableFrom(typeof(IBindableProperty)))
                    {
                        continue;
                    }

                    field = fieldInfo.GetValue(viewModel) as IBindableProperty;
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

        //获取某个特定的属性 当缓存中有的时候就直接拿出来  没有的话就通过反射获取
        IBindableProperty GetProperty(string propertyName)
        {
            bool get = propertyCache.TryGetValue(propertyName, out IBindableProperty field);
            if (!get)
            {
                if (viewModel == null)
                {
                    throw new Exception($"can not get property {propertyName}, please bind viewModel first");
                }
                var fieldInfo = viewModel.GetType().GetField(propertyName);
                if (fieldInfo == null || !fieldInfo.FieldType.IsAssignableFrom(typeof(IBindableProperty)))
                {
                    throw new Exception($"can not get property {propertyName} with {viewModel.GetType().FullName}, please check property name is right");
                }

                field = fieldInfo.GetValue(viewModel) as IBindableProperty;
                if(field != null)
                {
                    propertyCache.Add(propertyName, field);
                }
            }
            return field;
        }

        //通过指定类型和名字获取对应的属性
        BindableProperty<T> GetProperty<T>(string propertyName)
        {
            var property = GetProperty(propertyName);
            if(!(property is BindableProperty<T>))
            {
                throw new Exception($"{propertyName} is {property.GetType().FullName} not {typeof(BindableProperty<T>)}");
            }
            return (BindableProperty<T>)GetProperty(propertyName);
        }

        /// <summary>
        /// 一键绑定所有的Property
        /// </summary>
        /// <param name="bindView">是否绑定View</param>
        /// <param name="bindAlreadyBinded">是否绑定已经绑定的 请注意如果已经绑定过的再绑定到同一个listener的话 每次改变这个listener将会执行多次</param>
        public void BindPropertys(object target, bool withAlreadyBinded = false)
        {
            var propertys = GetPropertys(withAlreadyBinded);
            foreach(var property in propertys)
            {
                var addMethodInfo = property.property.GetType().GetMethod("AddListener", BindingFlags.Public | BindingFlags.NonPublic);
                string listenerMethodName = StringUtility.GetOrAttach("OnChanged_", property.propertyName);
                var listenerMethodInfo = target.GetType().GetMethod(listenerMethodName);
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
        /// <param name="bindView">是否是绑定view</param>
        public void BindWithAttribute(object target)
        {
            var methodInfos = target.GetType().GetMethods();
            if(methodInfos == null)
            {
                return;
            }

            foreach(var methodInfo in methodInfos)
            {
                var bindInfo = methodInfo.GetCustomAttribute<BindProperty>();
                if(bindInfo == null)
                {
                    continue;
                }
                var property = GetProperty(bindInfo.propertyName);
                var addMethodInfo = property.GetType().GetMethod("AddListener", BindingFlags.Public | BindingFlags.NonPublic);
                string listenerMethodName = StringUtility.GetOrAttach("OnChanged_", bindInfo.propertyName);
                var listenerMethodInfo = target.GetType().GetMethod(listenerMethodName);
                if (addMethodInfo == null || listenerMethodInfo == null)
                {
                    continue;
                }

                addMethodInfo.Invoke(property, new object[] { target, listenerMethodInfo });
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