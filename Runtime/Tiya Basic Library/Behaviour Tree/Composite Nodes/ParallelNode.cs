using System.Collections.Generic;

namespace Sarachan.UniTiya.BehaviourTree
{
    public sealed class ParallelNode : CompositeNode
    {
        readonly HashSet<int> _runningChildIndex = new HashSet<int>();

        BehaviourTreeResult _tempResult = BehaviourTreeResult.Success;

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            if (_runningChildIndex.Count == 0)
            {
                for (int i = 0; i < ChildNodes.Count; i++)
                {
                    switch (ChildNodes[i].Update())
                    {
                        case BehaviourTreeResult.Success:
                            break;
                        case BehaviourTreeResult.Failure:
                            _tempResult = BehaviourTreeResult.Failure;
                            break;
                        case BehaviourTreeResult.Running:
                            _runningChildIndex.Add(i);
                            break;
                    }
                }
            }
            else
            {
                foreach (var index in _runningChildIndex)
                {
                    switch (ChildNodes[index].Update())
                    {
                        case BehaviourTreeResult.Success:
                            _runningChildIndex.Remove(index);
                            break;
                        case BehaviourTreeResult.Failure:
                            _runningChildIndex.Remove(index);
                            _tempResult = BehaviourTreeResult.Failure;
                            break;
                        case BehaviourTreeResult.Running:
                            break;
                    }
                }
            }

            if (_runningChildIndex.Count == 0)
            {
                return _tempResult;
            }
            else
            {
                return BehaviourTreeResult.Running;
            }
        }

        protected internal override void Reset()
        {
            base.Reset();

            _tempResult = BehaviourTreeResult.Success;
        }
    }
}
