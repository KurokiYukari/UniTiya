using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sarachan.UniTiya.Interaction
{
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Interaction/Tiya Interactive Object Handler")]
    public class TiyaInteractiveObjectHandler : MonoBehaviour, IInteractiveObjectHandler
    {
        [SerializeField] Object _actorOverride;

        [SerializeField] InputAction _interactTrigger;
        [SerializeField] InputAction _iterateInteractiveAction;

        private IActorController _actor;
        public IActorController Actor
        {
            get
            {
                _actorOverride = _actorOverride == null ? gameObject : _actorOverride;

                return _actor ??= _actorOverride.ConvertTo<IActorController>() ?? throw new MissingComponentException($"{nameof(TiyaInteractiveObjectHandler)} can't find {nameof(IActorController)} component.");
            }
        }

        readonly LinkedList<IInteractive> _interactives = new LinkedList<IInteractive>();

        public ICollection<IInteractive> Interactives => _interactives;

        private LinkedListNode<IInteractive> _selectedInteractiveNode;
        public IInteractive SelectedInteractive
        {
            get
            {
                if (_selectedInteractiveNode == null)
                {
                    _selectedInteractiveNode = _interactives.First;
                }
                else
                {
                    if (_selectedInteractiveNode.List == null)
                    {
                        _selectedInteractiveNode = _interactives.First;
                    }
                }

                return _selectedInteractiveNode?.Value;
            }
        }

        public void IterateInteractive(bool toNext = true)
        {
            if (toNext)
            {
                if (_selectedInteractiveNode?.Next == null)
                {
                    _selectedInteractiveNode = _interactives.First;
                }
                else
                {
                    _selectedInteractiveNode = _selectedInteractiveNode?.Next;
                }
            }
            else
            {
                if (_selectedInteractiveNode?.Previous == null)
                {
                    _selectedInteractiveNode = _interactives.Last;
                }
                else
                {
                    _selectedInteractiveNode = _selectedInteractiveNode?.Previous;
                }
            }
        }
        public void IterateInteractive(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    if (value > 0)
                    {
                        IterateInteractive();
                    }
                    else
                    {
                        IterateInteractive(false);
                    }
                    break;
                default:
                    break;
            }
        }

        public void SubmitInteractive(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    SelectedInteractive?.Interact(Actor);
                    break;
                case InputActionPhase.Canceled:
                    break;
            }
        }

        private void Start()
        {
            if (Actor.IsPlayer)
            {
                _interactTrigger.performed += SubmitInteractive;
                _iterateInteractiveAction.performed += IterateInteractive;
            }
        }

        private void OnEnable()
        {
            if (Actor.IsPlayer)
            {
                _interactTrigger.Enable();
                _iterateInteractiveAction.Enable();
            }
        }

        private void OnDisable()
        {
            _interactTrigger.Disable();
            _iterateInteractiveAction.Disable();
        }
    }
}
