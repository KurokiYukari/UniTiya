using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaView
{
    // TODO: 添加 field of view 选项
    // TODO: 在 TiyaViewController 中添加配置默认设置的字段 
    [System.Serializable]
    public struct TiyaActorCameraViewConfiguration
    {
        public static TiyaActorCameraViewConfiguration DefaultFirstView { get; } = new TiyaActorCameraViewConfiguration()
        {
            TopRig = new RigConfiguration(0.02f, 0.01f),
            MiddleRig = new RigConfiguration(0, 0.01f),
            BottomRig = new RigConfiguration(-0.02f, 0.01f),
            X_Speed = 200f,
            Y_Speed = 1f
        };
        public static TiyaActorCameraViewConfiguration DefaultThirdView { get; } = new TiyaActorCameraViewConfiguration()
        {
            TopRig = new RigConfiguration(3f, 2f),
            MiddleRig = new RigConfiguration(0f, 6f),
            BottomRig = new RigConfiguration(-3f, 2f),
            X_Speed = 300f,
            Y_Speed = 3f
        };

        public RigConfiguration TopRig { get; set; }
        public RigConfiguration MiddleRig { get; set; }
        public RigConfiguration BottomRig { get; set; }

        private float _X_Speed;
        public float X_Speed { get => _X_Speed; set => _X_Speed = Mathf.Abs(value); }
        private float _Y_Speed;
        public float Y_Speed { get => _Y_Speed; set => _Y_Speed = Mathf.Abs(value); }

        [System.Serializable]
        public struct RigConfiguration
        {
            public float Height { get; set; }

            private float _radius;
            public float Radius { get => _radius; set => _radius = Mathf.Abs(value); }

            public RigConfiguration(float height, float radius) : this()
            {
                Height = height;
                Radius = radius;
            }
        }
    }
}
