using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sarachan.UniTiya.Interaction
{
    /// <summary>
    /// 可配置的交互类。可以通过继承或配置 Event 自定义。
    /// 交互的的条件是 IInteractiveObjectHandler 是否在 TiyaInteractive GameObject 的 Trigger 内。
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Interaction/Tiya Interactive")]
    public class TiyaInteractive : MonoBehaviour, IInteractive
    {
        [SerializeField] string _interactLabel;
        public string InteractLabel => _interactLabel;

        [SerializeField] InteractEvent _onInteracting;

        private Collider _interactiveTrigger;

        public HashSet<IInteractiveObjectHandler> ConnettedHandlers { get; } = new HashSet<IInteractiveObjectHandler>();

        public GameObject InteractiveObject => gameObject;

        public event System.Action<IActorController> OnInteracting;

        public void Interact(IActorController actor)
        {
            _onInteracting.Invoke(actor);
            OnInteracting?.Invoke(actor);
            InteractOverride(actor);
        }

        protected virtual void InteractOverride(IActorController actor) { }

        protected void Awake()
        {
            _interactiveTrigger = GetComponent<Collider>();
            if (!_interactiveTrigger.isTrigger)
            {
                Debug.LogWarning($"Interactive GameObject {gameObject.name}'s Collider is not trigger.");
            }
        }

        protected void OnTriggerEnter(Collider other)
        {
            var interactiveObjectHandler = other.GetComponent<IInteractiveObjectHandler>();
            if (interactiveObjectHandler != null)
            {
                interactiveObjectHandler.Interactives.Add(this);
                ConnettedHandlers.Add(interactiveObjectHandler);
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            var interactiveObjectHandler = other.GetComponent<IInteractiveObjectHandler>();
            if (interactiveObjectHandler != null)
            {
                interactiveObjectHandler.Interactives.Remove(this);
                ConnettedHandlers.Remove(interactiveObjectHandler);
            }
        }

        public static TiyaInteractive CreateDefaultInteractiveObject(string objName = "Tiya Interactive", float radius = 4)
        {
            var interactiveObject = new GameObject
            {
                name = objName
            };

            var sphereCollider = interactiveObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = radius;

            return interactiveObject.AddComponent<TiyaInteractive>();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/" + TiyaTools.UniTiyaName + "/Tiya Interactive Object", false, 10)]
        public static void CreateDefaultInteractiveObject(MenuCommand menuCommand)
        {
            var interactive = CreateDefaultInteractiveObject();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(interactive.gameObject, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(interactive.gameObject, "Create " + interactive.gameObject.name);
            Selection.activeObject = interactive.gameObject;
        }
#endif

        [System.Serializable]
        public class InteractEvent : UnityEvent<IActorController> { }
    }
}
