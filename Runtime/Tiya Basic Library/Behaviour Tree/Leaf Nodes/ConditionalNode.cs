using System;
using System.Collections.Generic;

namespace Sarachan.UniTiya.BehaviourTree
{
    /// <summary>
    /// 条件行为节点，通过指定 Func<bool> 来决定节点的返回值。
    /// </summary>
    public class ConditionalNode : LeafNode
    {
        Func<bool> ConditionFunc { get; }

        public ConditionalNode(Func<bool> conditionFunc)
        {
            ConditionFunc = conditionFunc ?? throw new ArgumentNullException(nameof(conditionFunc));
        }

        protected override BehaviourTreeResult OnUpdateOverride()
        {
            if (ConditionFunc())
            {
                return BehaviourTreeResult.Success;
            }
            else
            {
                return BehaviourTreeResult.Failure;
            }
        }
    }
}
