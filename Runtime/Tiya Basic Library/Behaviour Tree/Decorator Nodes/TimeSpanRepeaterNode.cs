using UniRx;

namespace Sarachan.UniTiya.BehaviourTree
{
    /// <summary>
    /// 将 DecoratedNode 执行指定时间的 Decorator。在这段时间内该节点返回 <see cref="BehaviourTreeResult.Running"/>。
    /// 从逻辑上，尽量不要用该节点装饰会处于 Running 状态的节点。
    /// </summary>
    public sealed class TimeSpanRepeaterNode : DecoratorNode
    {
        public float RepeatTimeSpan { get; set; }

        bool _isTimerRunning = false;

        public TimeSpanRepeaterNode(BehaviourTreeNodeBase decoratedNode, float repeatTimeSpan) : base(decoratedNode)
        {
            RepeatTimeSpan = repeatTimeSpan;
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            if (CanRun)
            {
                _isTimerRunning = true;
                Observable.Timer(System.TimeSpan.FromSeconds(RepeatTimeSpan))
                    .Subscribe(_ => _isTimerRunning = false)
                    .AddTo(BehaviourTree);
            }

            if (_isTimerRunning)
            {
                DecoratedNode.Update();
                return BehaviourTreeResult.Running;
            }
            else
            {
                return DecoratedNode.Update();
            }
        }
    }
}
