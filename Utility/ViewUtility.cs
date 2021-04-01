using Framework.ILR.Module.UI;
using Framework.Utility;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Framework.ILR.Utility
{
    public static class ViewUtility
    {
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
