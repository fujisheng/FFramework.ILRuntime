﻿using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.IL.Hotfix.Module.UI
{
    public interface IView
    {
        string viewName { get; }

        //View的生命周期
        void Init();
        void OnCreate(GameObject gameObject, Context context);
        UniTask Opening();
        void OnOpen(object param);
        void OnFrontground();
        void OnBackground();
        UniTask Closeing();
        void OnClose(object param);
        void OnDestroy();
    }
}
