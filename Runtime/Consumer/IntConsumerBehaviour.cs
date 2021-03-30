using UnityEngine;

using Sarachan.UniTiya.Utility;

namespace Sarachan.UniTiya.Consumer
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Consumer/Int Consumer")]
    public class IntConsumerBehaviour : BehaviourConsumerRef
    {
        [SerializeField] IntConsumer _consumer;

        public IntConsumer IntConsumer { get => _consumer; set => _consumer = value; }

        protected override IConsumer Consumer => _consumer;
    }

    [System.Serializable]
    public class IntConsumer : IConsumer
    {
        [SerializeField] IntPropertyValueSynchronizer _synchronizedValue;

        [SerializeField] int _step = 1;

        public bool CanConsume => CurrentValue > 0;

        public int CurrentValue
        {
            get => _synchronizedValue.Value;
            set => _synchronizedValue.Value = value;
        }

        public int Step
        {
            get => _step;
            set => _step = value > 0 ? value : 0;
        }

        public IntConsumer(object obj, string propertyName, int step = 1)
        {
            Step = step;
            _synchronizedValue = new IntPropertyValueSynchronizer(obj, propertyName);
        }

        public bool Consume()
        {
            if (CurrentValue - Step < 0)
            {
                return false;
            }
            else
            {
                var result = CurrentValue - Step;
                CurrentValue = result;
                return true;
            }
        }
    }
}
