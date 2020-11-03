using Hotfix.Utility;
using Framework.Module.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using ModuleManager = Framework.Module.ModuleManager;

namespace Framework.IL.Hotfix.Module.UI
{
    struct ViewRecording
    {
        public bool isFull { get; }
        public int layer { get; }
        public string viewName { get; }
        public object param { get; }

        public ViewRecording(string viewName, int layer, bool isFull, object param)
        {
            this.viewName = viewName;
            this.layer = layer;
            this.isFull = isFull;
            this.param = param;
        }

        public static ViewRecording Empty = new ViewRecording(null, 0, false, null);

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(viewName)
                && layer == 0
                && isFull == false
                && param == null;
        }
    }

    static class UICache
    {
        static Dictionary<string, Bind> viewInfoCache = new Dictionary<string, Bind>();
        static Dictionary<string, IView> viewInstanceCache = new Dictionary<string, IView>();
        //static Dictionary<string, IAsset> viewAssetCache = new Dictionary<string, IAsset>();
        static Dictionary<string, GameObject> viewGameObjectCache = new Dictionary<string, GameObject>();
        static Dictionary<string, IViewModel> viewModelCache = new Dictionary<string, IViewModel>();
        static Dictionary<string, IView> viewRegistry = new Dictionary<string, IView>();
        static Stack<ViewRecording> viewStack = new Stack<ViewRecording>();
        static Dictionary<string, ViewRecording> freeView = new Dictionary<string, ViewRecording>();

        static IResourceManager resourceManager;

        public static Bind GetBindInfo(string viewName)
        {
            bool get = viewInfoCache.TryGetValue(viewName, out Bind viewInfo);
            if (get)
            {
                return viewInfo;
            }
            viewInfo = ViewUtility.GetViewInfo(viewName);
            if (viewInfo == null)
            {
                Debug.LogError($"没有找到这个view的viewInfo:{viewName}");
                return null;
            }
            viewInfoCache.Add(viewName, viewInfo);
            return viewInfo;
        }

        public static IView GetOrCreateViewInstance(string viewName)
        {
            bool get = viewInstanceCache.TryGetValue(viewName, out IView view);
            if (get)
            {
                return view;
            }
            view = ViewUtility.CreatView(viewName);
            if(view == null)
            {
                Debug.LogError($"没有找到这个view:{viewName}");
                return null;
            }
            viewInstanceCache.Add(viewName, view);
            return view;
        }

        public static void DestroyViewInstance(string viewName)
        {
            bool get = viewInstanceCache.TryGetValue(viewName, out IView view);
            if (!get)
            {
                return;
            }
            view = null;
            viewInstanceCache.Remove(viewName);
        }

        public static async Task<GameObject> GetOrCreateViewGameObject(string gameObjectName)
        {
            bool get = viewGameObjectCache.TryGetValue(gameObjectName, out GameObject gameObject);
            if (get)
            {
                return gameObject;
            }

            //resourceManager = resourceManager ?? (resourceManager = Framework.Module.ModuleManager.GetModule<IResourceManager>());
            //IAsset asset = await resourceManager.LoadAsync<GameObject>(gameObjectName);
            //asset.Retain();
            //viewAssetCache.Add(gameObjectName, asset);
            //gameObject = GameObject.Instantiate(asset.asset as GameObject);
            //gameObject.SetActive(false);
            //viewGameObjectCache.Add(gameObjectName, gameObject);
            return gameObject;
        }

        public static void DestroyViewGameObject(string gameObjectName)
        {
            bool get = viewGameObjectCache.TryGetValue(gameObjectName, out GameObject gameObject);
            if (!get)
            {
                return;
            }
            viewGameObjectCache.Remove(gameObjectName);
            Object.DestroyImmediate(gameObject, false);
            //IAsset asset = viewAssetCache[gameObjectName];
            //viewAssetCache.Remove(gameObjectName);
            //asset.Release();
        }

        public static void RegistView(string viewName, IView view)
        {
            if (viewRegistry.ContainsKey(viewName))
            {
                return;
            }

            viewRegistry.Add(viewName, view);
        }

        public static IView GetRegistedView(string viewName)
        {
            bool get = viewRegistry.TryGetValue(viewName, out IView view);
            if (get)
            {
                return view;
            }
            return null;
        }

        public static void UnregistView(string viewName)
        {
            if (!viewRegistry.ContainsKey(viewName))
            {
                return;
            }

            viewRegistry.Remove(viewName);
        }

        public static void FrontgroundView(string viewName)
        {
            foreach(var kv in viewRegistry)
            {
                string name = kv.Key;
                IView view = kv.Value;
                if(name == viewName)
                {
                    view.OnFrontground();
                    continue;
                }

                view.OnBackground();
            }
        }

        public static IViewModel GetOrCreateViewModel(string viewModelName)
        {
            bool get = viewModelCache.TryGetValue(viewModelName, out IViewModel viewModel);
            if (get)
            {
                return viewModel;
            }
            viewModel = ViewUtility.CreateViewModel(viewModelName);
            if(viewModel == null)
            {
                Debug.LogError($"没有找到这个viewModel:{viewModelName}");
                return null;
            }
            viewModel.Init();
            viewModel.BindProperty();
            viewModelCache.Add(viewModelName, viewModel);
            return viewModel;
        }

        public static void PushView(string viewName, int layer, bool isFull, object param)
        {
            ViewRecording recording = new ViewRecording(viewName, layer, isFull, param);
            if(viewStack.Count != 0 && viewStack.Peek().viewName == viewName)
            {
                return;
            }
            viewStack.Push(recording);
        }

        public static ViewRecording PopView()
        {
            if(viewStack.Count == 0)
            {
                return ViewRecording.Empty;
            }

            return viewStack.Pop();
        }

        public static ViewRecording GetTopView()
        {
            if(viewStack.Count == 0)
            {
                return ViewRecording.Empty;
            }

            return viewStack.Peek();
        }

        public static int GetStackCount()
        {
            return viewStack.Count;
        }

        public static void PushFreeView(string viewName, int layer, object param)
        {
            if (freeView.ContainsKey(viewName))
            {
                return;
            }
            freeView.Add(viewName, new ViewRecording(viewName, layer, false, param));
        }

        public static ViewRecording GetTopFreeView()
        {
            if(freeView.Count == 0)
            {
                return ViewRecording.Empty;
            }

            int maxLayer = 0;
            string maxViewName = "";
            foreach(var kv in freeView)
            {
                if(kv.Value.layer > maxLayer)
                {
                    maxLayer = kv.Value.layer;
                    maxViewName = kv.Key;
                }
            }
            if (string.IsNullOrEmpty(maxViewName))
            {
                return ViewRecording.Empty;
            }
            return freeView[maxViewName];
        }
    }
}
