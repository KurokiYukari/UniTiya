namespace Sarachan.UniTiya
{
    //public interface IObject
    //{
    //    Object Object { get; }
    //}

    //public interface IGameObject : IObject
    //{
    //    new GameObject Object { get; }
    //}

    /// <summary>
    /// 有 Enable 功能的接口。
    /// 不建议给 Monobehaviour 之类的本来就有 Enable 功能的类实现该接口，
    /// 而是只给自定义类使用。
    /// </summary>
    public interface IEnable
    {
        bool Enabled { get; set; }
        void Enable();
        void Disable();

        event System.Action OnEnable;
        event System.Action OnDisable;
    }
}
