using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;

namespace Sarachan.UniTiya.Utility
{
    /// <summary>
    /// 定时对象回收器，会通过 IGameObjectPool 回收。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Utility/Timed GameObject Recycler")]
    public class TimedObjRecycler : MonoBehaviour
    {
        public float RecycleDecay = 10f;

        private void OnEnable()
        {
            Observable.Timer(System.TimeSpan.FromSeconds(RecycleDecay))
                .Subscribe(_ => TiyaGameSystem.Pool.RecyclePrefab(gameObject))
                .AddTo(this);
        }
    }
}
