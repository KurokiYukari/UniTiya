using UnityEngine;

namespace Sarachan.UniTiya.Consumer
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Consumer/ColdDown Consumer")]
    public class ColdDownConsumerBehaviour : BehaviourConsumerRef
    {
        [SerializeField] private ColdDownConsumer _consumer;

        protected override IConsumer Consumer => _consumer;
    }

    [System.Serializable]
    public class ColdDownConsumer : IConsumer
    {
        [SerializeField] private float _coldDowmTime;
        public float ColdDownTime { get => _coldDowmTime; set => _coldDowmTime = value; }

        private float _remainedCD = 0f;
        private float _lastRefTime = 0f;
        public float RemainedCD
        {
            get
            {
                var deltaTime = Time.time - _lastRefTime;

                RemainedCD = _remainedCD - deltaTime;

                return _remainedCD;
            }
            set
            {
                _lastRefTime = Time.time;
                _remainedCD = value <= 0 ? 0 : value;
            }
        }

        public ColdDownConsumer(float coldDownTime)
        {
            ColdDownTime = coldDownTime;
        }

        public bool CanConsume => RemainedCD == 0;

        public bool Consume()
        {
            if (CanConsume)
            {
                RemainedCD = ColdDownTime;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
