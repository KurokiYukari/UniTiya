using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Sarachan.UniTiya.Utility;
using Sarachan.UniTiya.Consumer;

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

        readonly TiyaEnableAgent _leftHandIK = new TiyaEnableAgent();
        public IEnable LeftHandIK => _leftHandIK;

        public Transform LeftHandIKHandler { get; set; }

        public RuntimeAnimatorController OriginAnimatorController { get; private set; }

        protected void Awake()
        {
            OriginAnimatorController = ActorAnimator.runtimeAnimatorController;
        }

        protected void OnEnable()
        {
            Actor.OnJump += OnJumpListener;

            LeftHandIK.Enable();
        }

        protected void OnDisable()
        {
            Actor.OnJump -= OnJumpListener;

            LeftHandIK.Disable();
        }

        protected void Update()
        {
            // Animator Params Setting
            ActorAnimator.SetBool(TiyaAnimatorTools.Params.IsAlive_B, Actor.IsAlive);
            ActorAnimator.SetBool(TiyaAnimatorTools.Params.IsGround_B, Actor.IsGround);
            ActorAnimator.SetFloat(TiyaAnimatorTools.Params.ScaledSpeed_F, Actor.ScaledSpeed);

            if (Actor.IsMoving)
            {
                var direction = transform.InverseTransformDirection(Actor.Velocity);
                direction = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;

                ActorAnimator.SetFloat(TiyaAnimatorTools.Params.DirectionX_F, direction.x);
                ActorAnimator.SetFloat(TiyaAnimatorTools.Params.DirectionZ_F, direction.z);
            }

            // Layer Move
            TiyaAnimatorTools.SingleLayerWeightLerper(ActorAnimator,
                TiyaAnimatorTools.Layer.FullBody,
                Actor.IsAlive ? 0 : 1);
        }

        protected void OnAnimatorIK(int layerIndex)
        {
            if (LeftHandIK.Enabled)
            {
                if (LeftHandIKHandler)
                {
                    ActorAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    ActorAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    ActorAnimator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKHandler.position);
                    ActorAnimator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKHandler.rotation);
                }
            }
            else
            {
                ActorAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                ActorAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }

        void OnJumpListener()
        {
            ActorAnimator.SetTrigger(TiyaAnimatorTools.Params.Jump_T);
        }

        // 动画事件
        public void AE_EnableLeftHandIK()
        {
            LeftHandIK.Enable();
        }

        public void AE_DisableLeftHandIK()
        {
            LeftHandIK.Disable();
        }
    }
}