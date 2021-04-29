using UnityEngine;
using UnityEngine.Events;

using Sarachan.UniTiya.Consumer;

namespace Sarachan.UniTiya.TiyaActor
{
    public class TiyaFootstepTool : MonoBehaviour
    {
        [SerializeField] AnimationCurve _ScaledSpeed_FootstepInterval_Curve;

        [SerializeField] float _startFootstepOffset = 0.2f;

        [SerializeField] UnityEvent _onFootstep;
        public event System.Action OnFootstep;

        readonly ColdDownConsumer _footstepTimer = new ColdDownConsumer(1);

        public IActorController Actor { get; private set; }

        protected void Awake()
        {
            Actor = GetComponent<IActorController>() ?? throw new MissingComponentException(nameof(IActorController));
        }

        protected void OnEnable()
        {
            Actor.OnStartMoving += StartMovingListener;
        }

        protected void OnDisable()
        {
            Actor.OnStartMoving -= StartMovingListener;
        }

        void StartMovingListener()
        {
            _footstepTimer.ColdDownTime = _startFootstepOffset;
            _footstepTimer.Consume();
        }

        protected void FixedUpdate()
        {
            if (Actor.IsMoving)
            {
                var footstepInterval = _ScaledSpeed_FootstepInterval_Curve.Evaluate(Actor.ScaledSpeed);
                _footstepTimer.ColdDownTime = footstepInterval;
                if (_footstepTimer.Consume())
                {
                    _onFootstep.Invoke();
                    OnFootstep?.Invoke();
                }
            }
        }
    }
}
