using UnityEngine;

namespace Sarachan.UniTiya.BehaviourTree
{
    /// <summary>
    /// 发呆的节点 <see cref="TimerNode"/> 的引用。
    /// </summary>
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Behaviour Tree/Timer Node")]
    class TimerNodeReference : ScriptableObject, IBehaviourTreeNodeReference, ISerializationCallbackReceiver
    {
        [SerializeField] float _sustainTimeSpan = 1f;

        readonly TimerNode _timer;
        public BehaviourTreeNodeBase Node => _timer;

        TimerNodeReference()
        {
            _timer = new TimerNode(_sustainTimeSpan);
        }

        public void OnAfterDeserialize()
        {
            _timer.TimeSpan = _sustainTimeSpan;
        }

        public void OnBeforeSerialize()
        {
            _sustainTimeSpan = _timer.TimeSpan;
        }
    }
}
