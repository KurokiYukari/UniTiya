using UnityEngine;

using Sarachan.UniTiya.BehaviourTree;

namespace Sarachan.UniTiya.TiyaActor.AI
{
    /// <summary>
    /// 节点 <see cref="ActorAttackNode"/> 的序列化形式。
    /// </summary>
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Behaviour Tree/Actor Nodes/Actor Attack Node")]
    sealed class ActorAttackNodeReference : ScriptableObject, IBehaviourTreeNodeReference, ISerializationCallbackReceiver
    {
        [SerializeField] AttackType _attackType;
        [SerializeField] float _attackSustainTimeSpan = 1f;

        readonly ActorAttackNode _attackNode;
        public BehaviourTreeNodeBase Node => _attackNode;

        public void OnAfterDeserialize()
        {
            _attackNode.AttackType = _attackType;
            _attackNode.AttackSustainTimeSpan = _attackSustainTimeSpan;
        }

        public void OnBeforeSerialize()
        {
            _attackType = _attackNode.AttackType;
            _attackSustainTimeSpan = _attackNode.AttackSustainTimeSpan;
        }

        ActorAttackNodeReference()
        {
            _attackNode = new ActorAttackNode(_attackType, _attackSustainTimeSpan);
        }
    }
}
