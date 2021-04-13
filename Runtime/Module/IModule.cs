namespace Framework.ILR.Module
{
    public interface IModule
    {
        //优先级
        int Priority { get;}
        bool LoadComplete { get; }
        bool InitComplete { get; }
        void OnLoad();
        void OnInit();
        void OnUpdate();
        void OnLateUpdate();
        void OnFixedUpdate();
        void OnTearDown();
    }
}