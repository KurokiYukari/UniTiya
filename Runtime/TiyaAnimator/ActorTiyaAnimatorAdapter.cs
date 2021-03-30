using System.Collections;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaAnimator
{
    /// <summary>
    /// TiyaAnimator 对于 IActorController 的适配器
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Animator/Actor Tiya Animator Adapter")]
    public class ActorTiyaAnimatorAdapter : MonoBehaviour, IActorTiyaAnimatorAdapter
    {
        IActorController _actor;
        protected IActorController Actor => _actor ??= GetComponent<IActorController>();
        protected Animator ActorAnimator => Actor.Animator;

        protected void OnEnable()
        {
            Actor.OnJump += OnJumpAction;
        }

        protected void OnDisable()
        {
            Actor.OnJump -= OnJumpAction;
        }

        protected void Update()
        {
            // Animator Params Setting
            ActorAnimator.SetBool(TiyaAnimatorTools.Params.IsGround_B, Actor.IsGround);
            ActorAnimator.SetFloat(TiyaAnimatorTools.Params.ScaledSpeed_F, Actor.ScaledSpeed);

            if (Actor.IsMoving)
            {
                var direction = transform.InverseTransformDirection(Actor.Velocity);
                direction = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;

                ActorAnimator.SetFloat("DirectionX", direction.x);
                ActorAnimator.SetFloat("DirectionZ", direction.z);
            }
        }

        void OnJumpAction()
        {
            ActorAnimator.SetTrigger(TiyaAnimatorTools.Params.Jump_T);
        }
    }
}