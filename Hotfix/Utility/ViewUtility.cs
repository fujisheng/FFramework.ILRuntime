using Framework.IL.Hotfix.Module.UI;
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix.Utility
{
    public static class ViewUtility
    {
        public static IViewModel CreateViewModel(string typeName)
        {
            string realTypeName = StringUtility.GetOrAttach("Hotfix.Logic.", typeName);
            try
            {
                Type viewModelType = Type.GetType(realTypeName);
                Debug.Log(viewModelType);
                object instance = Activator.CreateInstance(viewModelType);
                return instance as IViewModel;
            }
            catch
            {
                Debug.LogError($"没有找到这个viewModel {realTypeName}");
                return null;
            }
        }

        public static Bind GetViewInfo(string typeName)
        {
            string realTypeName = StringUtility.GetOrAttach("Hotfix.Logic.", typeName);
            try
            {
                Type viewType =  Type.GetType(realTypeName);
                MemberInfo memberInfo = viewType;
                object[] attributes = memberInfo.GetCustomAttributes(true);
                for (int i = 0; i < attributes.Length; i++)
                {
                    object attribute = attributes[i];
                    if (attribute is Bind)
                    {
                        return attribute as Bind;
                    }
                }
                return null;
            }
            catch
            {
                Debug.LogError($"没有找到这个viewType {realTypeName}");
                return null;
            }
        }

        public static IView CreatView(string typeName)
        {
            string realTypeName = StringUtility.GetOrAttach("Hotfix.Logic.", typeName);
            try
            {
                Type viewType = Type.GetType(realTypeName);
                object instance = Activator.CreateInstance(viewType);
                return instance as IView;
            }
            catch
            {
                Debug.LogError($"没有找到这个viewType {realTypeName}");
                return null;
            }
        }

        public static void AddAllButtonEvent(object view)
        {
            try
            {
                Type viewType = view.GetType();
                string viewTypeName = viewType.FullName;
                object buttons = viewType.GetProperty("Buttons").GetValue(view, null);
                List<Button> allButton = buttons as List<Button>;
                foreach(var button in allButton)
                {
                    string buttonName = button.name.Replace("@", "").Replace("(Clone)", "");
                    string functionName = StringUtility.GetOrAttach("OnClick_", buttonName);

                    button.onClick.AddListener(() =>
                    {
                        //IScriptManager scriptManager = Framework.Module.ModuleManager.GetModule<IScriptManager>();
                        //scriptManager.InvokeMethod(viewTypeName, functionName, view);
                        //Debug.Log("onClicksssssssssssssssssssssssssssss");
                        //MethodInvoker invoker = MethodInvokerCache.Get(viewTypeName, functionName, view, 0);
                        //invoker?.Invoke();
                    });
                }
            }
            catch
            {

            }
        }

        public static void RemoveAllButtonEvent(IView view)
        {
            try
            {
                Type viewType = view.GetType();
                object buttons = viewType.GetProperty("Buttons").GetValue(view, null);
                List<Button> allButton = buttons as List<Button>;
                foreach (var button in allButton)
                {
                    button.onClick.RemoveAllListeners();
                }
            }
            catch
            {

            }
        }
    }
}
