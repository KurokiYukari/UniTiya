using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using UniRx;

using Sarachan.UniTiya.Commands;
using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaActor
{
    /// <summary>
    /// IActorController 的实现
    /// </summary>
    [RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Actor/Tiya Actor Controller")]
    public class TiyaActorController : MonoBehaviour, IActorController
    {
        public GameObject GameObject => gameObject;

        [SerializeField] [TypeRestriction(typeof(IActorActions))] Object _actorActionsOverride;

        [SerializeField] GameObject _actorModelObject;

        [SerializeField] bool _isPlayer = false;
        public bool IsPlayer => _isPlayer;

        // TODO: 对非 Player 的 ViewController 做特殊处理
        [TypeRestriction(typeof(IActorCameraView)), HideIf(nameof(_isPlayer), false)]
        [SerializeField] GameObject _actorViewControllerObject;

        public IActorView ActorView { get; private set; }  

        [SerializeField] bool _enableGravity = true;
        [SerializeField] float _gravity = 9.8f;

        [SerializeField] bool _canMove = true;
        public bool CanMove
        {
            get => _canMove;
            set
            {
                if (_canMove != value)
                {
                    _canMove = value;
                    if (!value)
                    {
                        StopMoving();
                    }
                }
            }
        }

        [SerializeField] bool _canJump;
        public bool CanJump { get => _canJump; set => _canJump = value; }

        [SerializeField] float _leavingGroundDelayOfJumping;

        [SerializeField, SetProperty(nameof(Acceleration))] float _acceleration = 20f;
        public float Acceleration { get => _acceleration; set => _acceleration = Mathf.Abs(value); }

        [SerializeField] private ActorAimMode _aimMode;

        [Header("Events")]
        [SerializeField] ActorEvent _onStartMoving;
        [SerializeField] ActorEvent _onStopMoving;
        [SerializeField] ActorEvent _onJump;
        [SerializeField] ActorEvent _onLeavingGround;
        [SerializeField] ActorEvent _onLanding;
        [SerializeField] ActorChangeLocomotionModeEvent _onChangeLocomotionMode;

        [System.Serializable]
        class ActorEvent : UnityEvent<IActorController> { }
        [System.Serializable]
        class ActorChangeLocomotionModeEvent : UnityEvent<IActorController, ActorLocomotionMode, ActorLocomotionMode> { }

        public event System.Action OnStartMoving;
        public event System.Action OnStopMoving;
        public event System.Action OnJump;
        public event System.Action OnLeavingGround;
        public event System.Action OnLanding;
        public event System.Action<ActorLocomotionMode, ActorLocomotionMode> OnChangeLocomotionMode;

        BasicCommandProcessor<TiyaActorController> _commandProcessor;
        public ICommandProcessor<TiyaActorController> CommandProcessor => _commandProcessor;
        ICommandProcessor<IActorController> IActorController.CommandProcessor => _commandProcessor;

        IActorActions _defaultActions;
        public IActorActions DefaultActions => _defaultActions ??= new TiyaActorActions(this);

        IActorActions _actorActions;
        public IActorActions ActorActions => _actorActions ??= _actorActionsOverride == null ?
            DefaultActions : _actorActionsOverride.ConvertTo<IActorActions>();

        public Animator Animator { get; private set; }
        public bool ApplyRootMotion { get; set; } = false;

        CharacterController _chrtMover; // 用此属性的方法来进行实际的移动

        bool _preIsGround;
        Vector3 _targetMoveDirection; // 目标世界坐标移动方向
        Vector3 _moveDirection; // 当前世界坐标移动方向
        float _targetHorizontalVelocity = 0;
        float _currentHorizontalVelocity = 0;
        float _currentVerticalVelocity = -0.5f; // 注意，因为 CharacterController.IsGround 的问题，这里的默认值建议为稍大的负数
                                                // 另，不知道为什么设为负数就正常了 qwq

        public IActorProperties GameProperties { get; private set; }

        public Transform ActorTransform => transform;

        public IEquipmentManager EquipmentManager { get; private set; }

        public IInventory Inventory { get; private set; }

        public ActorAimMode AimMode
        {
            get => _aimMode;
            set => _aimMode = value;
        }

        public Vector3 Velocity => _chrtMover.velocity;

        ActorLocomotionMode _locomotionMode = ActorLocomotionMode.Run;
        public ActorLocomotionMode LocomotionMode
        {
            get => _locomotionMode;
            set
            {
                if (_locomotionMode != value)
                {
                    var preMode = _locomotionMode;
                    _locomotionMode = value;

                    _onChangeLocomotionMode.Invoke(this, preMode, value);
                    OnChangeLocomotionMode?.Invoke(preMode, value);
                }
            }
        }

        public float ScaledSpeed
        {
            get
            {
                var walkSpeed = GameProperties.WalkSpeed.Value;
                var runSpeed = GameProperties.RunSpeed.Value;
                var sprintSpeed = GameProperties.SprintSpeed.Value;

                if (_currentHorizontalVelocity <= walkSpeed)
                {
                    return _currentHorizontalVelocity / walkSpeed;
                }
                else if (_currentHorizontalVelocity <= runSpeed)
                {
                    return 1 + (_currentHorizontalVelocity - walkSpeed) / (runSpeed - walkSpeed);
                }
                else if (_currentHorizontalVelocity <= sprintSpeed)
                {
                    return 2 + (_currentHorizontalVelocity - runSpeed) / (sprintSpeed - runSpeed);
                }
                else
                {
                    return 3;
                }
            }
        }

        public bool IsGround => _chrtMover.isGrounded;
        public bool IsMoving => _moveDirection != Vector3.zero;

        #region Unity Events
        private void Awake()
        {
            _commandProcessor = new BasicCommandProcessor<TiyaActorController>(this);

            if (IsPlayer)
            {
                ActorView = _actorViewControllerObject != null ? _actorViewControllerObject.ConvertTo<IActorView>() :
                    throw new MissingReferenceException($"Player's ActorController's {_actorViewControllerObject} can't be null.");
            }
            else
            {
                ActorView = GetComponent<IActorView>() ?? throw new MissingComponentException($"Component {nameof(IActorView)} missing in GameObject {name}.");
            }

            GameProperties = GetComponent<IActorProperties>() ?? throw new MissingComponentException($"Component {nameof(IActorProperties)} missing in GameObject {name}.");

            Animator = GetComponent<Animator>();
            _chrtMover = GetComponent<CharacterController>();

            EquipmentManager = GetComponent<IEquipmentManager>() ?? gameObject.AddComponent<TiyaActorEquipmentManager>();
            Inventory = GetComponent<IInventory>() ?? gameObject.AddComponent<ItemSystem.TiyaInventory>();
        }

        private void OnEnable()
        {
            _preIsGround = IsGround;
            _commandProcessor.Enable();
        }

        private void OnDisable()
        {
            _commandProcessor.Disable();
        }

        private void Update()
        {
            if (_preIsGround != IsGround)
            {
                if (IsGround)
                {
                    _onLanding.Invoke(this);
                    OnLanding?.Invoke();

                    _currentVerticalVelocity = -0.5f;
                }
                else
                {
                    _onLeavingGround.Invoke(this);
                    OnLeavingGround?.Invoke();
                }

                _preIsGround = IsGround;
            }
        }

        private void FixedUpdate()
        {
            // Handle Gravity
            if (_enableGravity && !IsGround)
            {
                _currentVerticalVelocity -= _gravity * Time.deltaTime;
            }
        }

        private void OnAnimatorMove()
        {
            if (IsGround)
            {
                _moveDirection = _targetMoveDirection;
            }

            if (IsMoving)
            {
                _currentHorizontalVelocity = Mathf.MoveTowards(_currentHorizontalVelocity, _targetHorizontalVelocity, Acceleration * Time.deltaTime);
            }
            else
            {
                // 停止移动
                // TAG: 或许也可以改成减速？
                _currentHorizontalVelocity = 0;
            }

            Vector3 movement = Vector3.up * _currentVerticalVelocity * Time.deltaTime;

            movement += _moveDirection * _currentHorizontalVelocity * Time.deltaTime;

            // Handle RootMotion
            if (ApplyRootMotion)
            {
                transform.rotation *= Animator.deltaRotation;
                movement += Animator.deltaPosition;
            }

            _chrtMover.Move(movement);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            switch (AimMode)
            {
                case ActorAimMode.LookAtActorForward:
                    Animator.SetLookAtWeight(0);
                    break;
                case ActorAimMode.LookAtAimPos:
                    Animator.SetLookAtWeight(1);
                    Animator.SetLookAtPosition(this.GetAimFocusPosition());
                    break;
                case ActorAimMode.TurnUpperBodyToAimDirection:
                    // ActorTrans.forward 与 AimPose.forward 夹角不超过 180 度情况，旋转上半身
                    if (Vector3.Dot(ActorTransform.forward, ActorView.ViewTransform.forward) >= 0)
                    {
                        var chestBone = Animator.GetBoneTransform(HumanBodyBones.Chest);
                        var chestTargetLocalForward = chestBone.parent.transform.InverseTransformDirection(ActorView.ViewTransform.forward);

                        Animator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.LookRotation(chestTargetLocalForward, chestBone.up));
                    }
                    // 超过 180 度，绕 aimPose 直立坐标 y 轴旋转 actor
                    else
                    {
                        // 建议在第三人称不要出现这种情况，因为会有比较不自然自然的突然旋转
                        // 这可以通过强制 actor forward 同 aimPose forward 在 xz 上的投影做到。 
                        transform.RotateAround(ActorView.ViewTransform.position, Vector3.up, Vector3.Angle(ActorTransform.forward, ActorView.ViewTransform.forward));
                    }
                    break;
            }
        }
        #endregion

        public void SimpleMove(Vector3 displacement) => _chrtMover.Move(displacement);

        public void TurnThenMoveForward(Vector3 direction)
        {
            if (CanMove)
            {
                TurnTo(Vector3.ProjectOnPlane(direction, Vector3.up));

                var newDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
                if (IsMoving && newDirection == Vector3.zero)
                {
                    StopMoving();
                    return;
                }

                if (!IsMoving && newDirection != Vector3.zero)
                {
                    _onStartMoving.Invoke(this);
                    OnStartMoving?.Invoke();
                }

                _targetHorizontalVelocity = LocomotionMode switch
                {
                    ActorLocomotionMode.Walk => GameProperties.WalkSpeed.Value,
                    ActorLocomotionMode.Run => GameProperties.RunSpeed.Value,
                    ActorLocomotionMode.Sprint => GameProperties.SprintSpeed.Value,
                    _ => throw new System.InvalidOperationException()
                };

                _targetMoveDirection = newDirection;
            }
        }

        public void StrafeMove(Vector3 direction)
        {
            if (CanMove)
            {
                if (LocomotionMode == ActorLocomotionMode.Sprint)
                {
                    TurnThenMoveForward(direction);
                    return;
                }

                var newDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
                if (IsMoving && newDirection == Vector3.zero)
                {
                    StopMoving();
                    return;
                }

                if (!IsMoving && newDirection != Vector3.zero)
                {
                    _onStartMoving.Invoke(this);
                    OnStartMoving?.Invoke();
                }

                _targetMoveDirection = newDirection;

                var localDirection = transform.InverseTransformDirection(_targetMoveDirection);
                // 根据自身坐标方向插值计算速度
                _targetHorizontalVelocity = Mathf.Pow(localDirection.x, 2) * GameProperties.RunSpeed.Value
                    + Mathf.Pow(localDirection.z, 2) * (localDirection.z > 0 ? GameProperties.RunSpeed.Value : GameProperties.WalkSpeed.Value);
            }
        }

        public void StopMoving()
        {
            // Invoke OnStopMoving
            OnStopMoving?.Invoke();
            _onStopMoving.Invoke(this);

            _targetMoveDirection = Vector3.zero;
            _targetHorizontalVelocity = 0;
        }

        public void TurnTo(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                return;
            }

            if (!IsGround)
            {
                return;
            }

            // 执行旋转
            transform.rotation = Quaternion.LookRotation(direction);
        }

        public void Jump()
        {
            if (CanJump)
            {
                // Invoke OnJump
                _onJump.Invoke(this);
                OnJump?.Invoke();

                if (_leavingGroundDelayOfJumping > 0)
                {
                    Observable.Timer(System.TimeSpan.FromSeconds(_leavingGroundDelayOfJumping))
                        .Subscribe(_ => OnJumpLeavingGroundAction())
                        .AddTo(this);
                }
                else
                {
                    OnJumpLeavingGroundAction();
                }
            }

            void OnJumpLeavingGroundAction()
            {
                _currentVerticalVelocity = GameProperties.JumpInitialSpeed.Value;
            }
        }

        /// <summary>
        /// 将 modelPrefab 替换当前 Controller 的 model
        /// </summary>
        /// <param name="modelPrefab">一个具有 chrtModelInfo 组件的 GameObject</param>
        public void ChangeModelTo(GameObject modelPrefab)
        {
            // 创建 model
            GameObject modelObj = Instantiate(modelPrefab, transform);

            // 删除当前 model
            Destroy(_actorModelObject);

            // 更新 ChrtModel，将新 model 置于 ChrtController 层级下
            _actorModelObject = modelObj;
            _actorModelObject.transform.parent = transform;
        }

        #region Inner Types
        class TiyaActorActions : IActorActions
        {
            TiyaActorController Actor { get; }

            public TiyaActorActions(TiyaActorController actor)
            {
                Actor = actor;
            }

            public void Move(Vector3 direction)
            {
                if (Actor.IsPlayer && (Actor.ActorView as IActorCameraView).Mode == CameraViewMode.First)
                {
                    Actor.StrafeMove(direction);
                }
                else
                {
                    if (Actor.ActorView.IsLocked)
                    {
                        Actor.TurnTo(Vector3.ProjectOnPlane(Actor.ActorView.ViewTransform.forward, Vector3.up));
                        Actor.StrafeMove(direction);
                    }
                    else
                    {
                        Actor.TurnThenMoveForward(direction);
                    }
                }
            }

            public void SimpleMove(Vector3 displacement) => Actor.SimpleMove(displacement);

            public void Jump() => Actor.Jump();

            public void Lock(LockCmdType cmdType) => Actor.ActorView.Lock(cmdType);

            public void SwitchLocomotionMode(ActorLocomotionMode mode) => Actor.LocomotionMode = mode;

            public void View(Vector2 direction) => Actor.ActorView.View(direction);
        }
        #endregion
    }
}
