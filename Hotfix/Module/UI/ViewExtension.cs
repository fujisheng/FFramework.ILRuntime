using Framework.Utility;
using System.Collections.Generic;

namespace Framework.IL.Hotfix.Module.UI
{
    public abstract partial class View
    {
        //SubView
        protected Dictionary<string, List<ISubView>> SubViewDic = new Dictionary<string, List<ISubView>>();
        //本地化text和语言包id映射表
        protected virtual Dictionary<string, int> LocalizationMap { get; }

        /// <summary>
        /// 向绑定的viewModel发送一个命令 对应OnCommand_xxx
        /// </summary>
        /// <param name="command">命令名字</param>
        /// <param name="args">参数</param>
        protected void SendCommand(string command, object args = null)
        {
            if (viewModel == null)
            {
                return;
            }
            string viewModelTypeName = viewModel.GetType().FullName;
            string functionName = StringUtility.GetOrAttach("OnCommand_", command);
            //MethodInvoker.Invoke(viewModelTypeName, functionName, viewModel, args);
        }

        /// <summary>
        /// 本地化 如果需要本地化  需要先重写LocalizationMap
        /// </summary>
        async void Localization()
        {
            //Dictionary<string, int> localizationMap = this.LocalizationMap;

            //if(localizationMap == null)
            //{
            //    return;
            //}

            //LanguageManager languageManager = AutoLoader.GetInstance<LanguageManager>();

            //foreach (var key in localizationMap.Keys)
            //{
            //    if (!ComponentDic.ContainsKey(key))
            //    {
            //        continue;
            //    }

            //    List<Component> components = ComponentDic[key];

            //    for(int i = 0; i < components.Count; i++)
            //    {
            //        Component component = components[i];
            //        if(component is Text)
            //        {
            //            (component as Text).text = await languageManager.GetString(localizationMap[key]);
            //        }
            //    }
            //}
        }
    }
}
