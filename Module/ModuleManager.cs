using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.ILR.Module
{
    public static class ModuleManager
    {
        static List<IModule> modules = new List<IModule>();
        static Queue<IModule> LoadQueue = new Queue<IModule>();
        static Queue<IModule> InitQueue = new Queue<IModule>();
        static IModule currentOperateModule;
        static bool initComplete = false;

        /// <summary>
        /// 添加Module  添加顺序决定了加载顺序
        /// </summary>
        /// <param name="moduleList"></param>
        public static void AddModules(params IModule[] moduleList)
        {
            Debug.Log("<color=blue>=======StartAddHotfixModule=======</color>");
            foreach(var module in moduleList)
            {
                modules.Add(module);
            }
        }

        //加载所有的module
        public static void Load()
        {
            Debug.Log("<color=blue>=======StartLoadHotfixModule=======</color>");

            for(int i = 0; i < modules.Count; i++)
            {
                IModule module = modules[i];
                LoadQueue.Enqueue(module);
                InitQueue.Enqueue(module);
            }
        }

        static void LoadModules()
        {
            if(LoadQueue.Count == 0)
            {
                return;
            }

            if(currentOperateModule == null)
            {
                currentOperateModule = LoadQueue.Dequeue();
                Type type = currentOperateModule.GetType();
                Debug.Log($"<color=blue>LoadModule=>{currentOperateModule.GetType().Name}</color>");
                currentOperateModule.OnLoad();
            }

            if(currentOperateModule.LoadComplete == true)
            {
                currentOperateModule = null;
            }
        }

        static void InitModules()
        {
            if(InitQueue.Count == 0)
            {
                initComplete = true;
                Debug.Log($"<color=blue>------------------InitHotfixModulesComplete-----------------</color>");
                return;
            }

            if(LoadQueue.Count > 0)
            {
                return;
            }

            if (currentOperateModule == null)
            {
                currentOperateModule = InitQueue.Dequeue();
                Type type = currentOperateModule.GetType();
                Debug.Log($"<color=blue>InitModule=>{currentOperateModule.GetType().Name}</color>");
                currentOperateModule.OnInit();
            }

            if (currentOperateModule.LoadComplete == true)
            {
                currentOperateModule = null;
            }
        }

        public static void Update()
        {
            if(initComplete == false)
            {
                LoadModules();
                InitModules();
                return;
            }

            for(int i = 0; i < modules.Count; i++)
            {
                modules[i].OnUpdate();
            }
        }

        public static void LateUpdate()
        {
            if(initComplete == false)
            {
                return;
            }

            for(int i = 0; i < modules.Count; i++)
            {
                modules[i].OnLateUpdate();
            }
        }

        public static void FixedUpdate()
        {
            if (initComplete == false)
            {
                return;
            }

            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].OnFixedUpdate();
            }
        }

        public static void TearDown()
        {
            if (initComplete == false)
            {
                return;
            }

            for (int i = modules.Count - 1; i >= 0; i--)
            {
                modules[i].OnTearDown();
            }
        }

        /// <summary>
        /// 获取一个module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetModule<T>()
        {
            if(modules == null)
            {
                Debug.LogError("Moduel还没有加载完成！！！！");
                return default;
            }

            for (int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                if(modules[i] is T)
                {
                    return (T)module;
                }
            }

            return default;
        }

        public static IModule GetModule(Type type)
        {
            if (modules == null)
            {
                Debug.LogError("Moduel还没有加载完成！！！！");
                return default;
            }

            for(int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];
                if (module.GetType().IsAssignableFrom(type))
                {
                    return module;
                }
            }

            return default;
        }
    }
}
