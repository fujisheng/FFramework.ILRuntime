namespace Framework.ILR.Service.UI
{
    public static class Layer
    {
        public const int NORMAL = 0;
        public const int POPUP = 1;
        public const int TOP = 10000;
    }

    public static class Flag
    {
        public const int NONE = 1 << 0;
        public const int CLEARSTACK = 1 << 1;
        public const int CACHE = 1 << 2;
        public const int CLEARBACK = 1 << 3;
        public const int SHOWBLUR = 1 << 4;
    }
}
