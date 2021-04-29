using UnityEngine;

using UniRx;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.ItemSystem.Listeners
{
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/Item System/Listener/Item Health Listener")]
    public class ItemHealthListener : ScriptableObject
    {
        [SerializeField] bool _continuousHealth = false;

        [SerializeField] TiyaTools.HealthValueType _healthValueType;

        [SerializeField] [HideIf(nameof(_continuousHealth), true)] float _healthValue;

        [SerializeField] [HideIf(nameof(_continuousHealth), false)] float _healthValuePerSecond;
        [SerializeField] [HideIf(nameof(_continuousHealth), false)] float _healthDuration;

        public void Health(IItem item, IActorController actor)
        {
            if (_continuousHealth)
            {
                actor.GameProperties.ContinuousHealth(_healthValue, _healthValueType, _healthDuration);
            }
            else
            {
                actor.GameProperties.InstantHealth(_healthValue, _healthValueType);
            }
        }
    }
}
