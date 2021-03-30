using UniRx;

namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class DelayNode : DecoratorNode
    {
        public float DelayTime { get; set; }

        bool _isDelayRunning = false; 

        public DelayNode(TiyaBehaviourTree behaviourTree, BehaviourTreeNodeBase decoratedNode) : base(behaviourTree, decoratedNode)
        {
        }

        public override BehaviourTreeResult Update()
        {
            if (!_isDelayRunning && CanRun)
            {
                _isDelayRunning = true;
                Observable.Timer(System.TimeSpan.FromSeconds(DelayTime))
                .Subscribe(_ => {
                    _isDelayRunning = false;
                });
            }

            if (!_isDelayRunning)
            {
                return NodeResult = BehaviourTreeResult.Running;
            }
            else
            {
                return NodeResult = DecoratedNode.Update();
            }
        }

        public override void Reset()
        {
            base.Reset();

            _isDelayRunning = false;
        }
    }
}
