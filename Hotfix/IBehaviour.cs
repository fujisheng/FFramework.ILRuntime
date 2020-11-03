using UnityEngine;

namespace Framework.IL.Hotfix
{
    public interface IBehaviour
    {
        GameObject gameObject { get;}
        Transform transform { get;}
        void OnCreated(GameObject gameObject);
        void OnDestroy();
    }
}
