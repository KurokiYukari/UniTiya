namespace Sarachan.UniTiya
{
    /// <summary>
    /// 玩家 Actor 的 View，它将 View 控制与 Camera 控制结合在一起
    /// </summary>
    public interface IActorCameraView : IActorView
    {
        /// <summary>
        /// 启用 / 禁用对 Camera 的控制
        /// </summary>
        IEnable ControllerEnabler { get; }

        /// <summary>
        /// 控制当前的视角操控模式
        /// </summary>
        CameraViewMode Mode { get; set; }

        /// <summary>
        /// 视野控制
        /// </summary>
        float FieldOfView { get; set; }
    }

    /// <summary>
    /// 可选的视角模式
    /// </summary>
    public enum CameraViewMode { First, Third }
}
