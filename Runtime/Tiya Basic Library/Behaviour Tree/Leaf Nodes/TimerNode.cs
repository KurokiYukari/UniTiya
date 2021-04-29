using UnityEngine;

using UniRx;

namespace Sarachan.UniTiya.BehaviourTree
{
    public class TimerNode : LeafNode
    {
        public float TimeSpan { get; set; }

        bool _isTimerRunning = false;

        public TimerNode(float timeSpan)
        {
            TimeSpan = timeSpan;
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            if (CanRun)
            {
                _isTimerRunning = true;
                Observable.Timer(System.TimeSpan.FromSeconds(TimeSpan))
                    .Subscribe(_ => _isTimerRunning = false)
                    .AddTo(BehaviourTree);
            }

            if (_isTimerRunning)
            {
                return BehaviourTreeResult.Running;
            }
            else
            {
                return BehaviourTreeResult.Success;
            }
        }
    }
}
