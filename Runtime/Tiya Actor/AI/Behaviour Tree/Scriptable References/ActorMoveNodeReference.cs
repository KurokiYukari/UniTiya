using UnityEngine;

using Sarachan.UniTiya.BehaviourTree;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaActor.AI
{
    /// <summary>
    /// 节点 <see cref="ActorMoveNode"/> 的序列化形式。
    /// </summary>
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Behaviour Tree/Actor Nodes/Actor Move Node")]
    sealed class ActorMoveNodeReference : ScriptableObject, IBehaviourTreeNodeReference, ISerializationCallbackReceiver
    {
        [SerializeField] float _moveSustainTimeSpan = 1f;

        [SerializeField] bool _randomDirection = true;

        [Disable(nameof(_randomDirection), true)]
        [SerializeField] bool _relativeSpace = true;

        [Disable(nameof(_randomDirection), true)]
        [SerializeField] Vector3 _direction;

        readonly ActorMoveNode _moveNode;
        public BehaviourTreeNodeBase Node => _moveNode;

        public void OnAfterDeserialize()
        {
            _moveNode.MoveSustainedTimeSpan = _moveSustainTimeSpan;
            _moveNode.RandomDirection = _randomDirection;

            if (_randomDirection)
            {
                _moveNode.RelativeSpace = true;
            }
            else
            {
                _moveNode.RelativeSpace = _relativeSpace;
            }
            
            _moveNode.Direction = _direction;
        }

        public void OnBeforeSerialize()
        {
            _moveSustainTimeSpan = _moveNode.MoveSustainedTimeSpan;
            _randomDirection = _moveNode.RandomDirection;
            _relativeSpace = _moveNode.RelativeSpace;
            _direction = _moveNode.Direction;
        }

        ActorMoveNodeReference()
        {
            _moveNode = new ActorMoveNode(_moveSustainTimeSpan);
        }
    }
}
