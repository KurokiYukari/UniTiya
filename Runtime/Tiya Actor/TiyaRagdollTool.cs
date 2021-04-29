using UnityEngine;

using UniRx;

using Sarachan.UniTiya.Utility;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaActor
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Actor/Tiya Ragdoll Tool")]
    public class TiyaRagdollTool : MonoBehaviour
    {
        [SerializeField] GameObject _ragdollRoot;

        [SerializeField] bool _enableRagdollOnDie;
        [HideIf(nameof(_enableRagdollOnDie), false)]
        [SerializeField] float _enableDelay = 0f;

        public Rigidbody[] RagdollRigidbodies { get; private set; }
        public Collider[] RagdollColliders { get; private set; }

        public IActorController Actor { get; private set; }

        public IEnable RagdollEnabler { get; private set; }

        protected void Awake()
        {
            if (_ragdollRoot == null)
            {
                throw new MissingReferenceException(nameof(_ragdollRoot));
            }

            RagdollRigidbodies = _ragdollRoot.GetComponentsInChildren<Rigidbody>();
            RagdollColliders = new Collider[RagdollRigidbodies.Length];
            for (int i = 0; i < RagdollRigidbodies.Length; i++)
            {
                var collider = RagdollRigidbodies[i].GetComponent<Collider>();
                Debug.Assert(collider, $"Ragdoll rigidbody {RagdollRigidbodies[i].name} missing Collider component.");
                RagdollColliders[i] = collider;
            }

            Actor = GetComponent<IActorController>() ?? throw new MissingComponentException(nameof(Actor));

            RagdollEnabler = new TiyaEnableAgent(EnableRagdollListener, DisableRagdollListener, true);

            void EnableRagdollListener()
            {
                foreach (var rigidbody in RagdollRigidbodies)
                {
                    rigidbody.isKinematic = false;
                }
                foreach (var collider in RagdollColliders)
                {
                    collider.enabled = true;
                }

                Actor.Animator.enabled = false;
            }

            void DisableRagdollListener()
            {
                foreach (var rigidbody in RagdollRigidbodies)
                {
                    rigidbody.isKinematic = true;
                }
                foreach (var collider in RagdollColliders)
                {
                    collider.enabled = false;
                }

                Actor.Animator.enabled = true;
            }
        }

        protected void Start()
        {
            RagdollEnabler.Disable();
        }

        protected void OnEnable()
        {
            if (_enableRagdollOnDie)
            {
                Actor.OnDie += OnDieListener;
                Actor.OnRelive += RagdollEnabler.Disable;
            }
        }

        protected void OnDisable()
        {
            Actor.OnDie -= OnDieListener;
            Actor.OnRelive -= RagdollEnabler.Disable;
        }

        void OnDieListener()
        {
            if (_enableDelay <= 0)
            {
                RagdollEnabler.Enable();
            }
            else
            {
                Observable.Timer(System.TimeSpan.FromSeconds(_enableDelay))
                    .Subscribe(_ => RagdollEnabler.Enable())
                    .AddTo(this);
            }
        }
    }
}
