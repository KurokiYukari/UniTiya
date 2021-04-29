using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaView
{
    /// <summary>
    /// 视角控制器，基于 Cinemachine 实现
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/View/Tiya Actor Camera View")]
    public class TiyaActorCameraView : TiyaActorView, IActorCameraView
    {
        [Header("Reference")]
        [SerializeField] Cinemachine.CinemachineFreeLook _freeLookVC;

        [SerializeField] bool _enableFirstViewPlayerCamera;
        [HideIf(nameof(_enableFirstViewPlayerCamera), false)]
        [SerializeField] FirstViewPlayerCameraSetting _firstViewPlayerCameraSetting;

        public bool EnableFirstViewCameraView
        {
            get => _enableFirstViewPlayerCamera;
            set
            {
                if (_enableFirstViewPlayerCamera != value)
                {
                    _enableFirstViewPlayerCamera = value;

                    RefreshFirstViewPlayerCamera();
                }
            }
        }
        public Camera FirstViewPlayerCamera => _firstViewPlayerCameraSetting.FirstViewPlayerCamera;
        public Transform FirstViewPlayerCameraFollow 
        {
            get => _firstViewPlayerCameraSetting.FirstViewPlayerCameraFollow;
            set => _firstViewPlayerCameraSetting.FirstViewPlayerCameraFollow = value; 
        }
        public LayerMask FirstViewPlayerCameraRenderLayer
        {
            get => _firstViewPlayerCameraSetting.FirstPlayerCameraRenderLayer;
            set
            {
                _firstViewPlayerCameraSetting.FirstPlayerCameraRenderLayer = value;

                RefreshFirstViewPlayerCamera();
            }
        }

        [Header("General")]
        public GameObject LookAt;

        [Header("InputSetting")]
        [SerializeField] InputActionReference _XY_Asix;
        [SerializeField] InputActionReference _Z_Asix;

        [SerializeField] CameraViewMode _mode = CameraViewMode.Third;
        /// <summary>
        /// 控制当前的视角操控模式
        /// </summary>
        public CameraViewMode Mode {
            get => _mode;
            set
            {
                _mode = value;

                switch (value)
                {
                    case CameraViewMode.First:
                        _followCtrl.ConnectedObj = LookAt;

                        LoadConfiguration(TiyaActorCameraViewConfiguration.DefaultFirstView);
                        RefreshFirstViewPlayerCamera();

                        //_freeLookVC.m_XAxis.Value = 0;
                        //_freeLookVC.m_YAxis.Value = 0.5f;
                        break;
                    case CameraViewMode.Third:
                        _followCtrl.ConnectedObj = ViewTransform.gameObject;

                        LoadConfiguration(TiyaActorCameraViewConfiguration.DefaultThirdView);
                        RefreshFirstViewPlayerCamera();
                        
                        break;
                    default:
                        break;
                }
            }
        }

        void RefreshFirstViewPlayerCamera()
        {
            var firstViewCamera = _firstViewPlayerCameraSetting.FirstViewPlayerCamera;

            if (!firstViewCamera)
            {
                return;
            }

            firstViewCamera.cullingMask = _firstViewPlayerCameraSetting.FirstPlayerCameraRenderLayer;

            switch (Mode)
            {
                case CameraViewMode.First:
                    if (_enableFirstViewPlayerCamera)
                    {
                        firstViewCamera.gameObject.SetActive(true);
                        Camera.main.cullingMask &= ~(_firstViewPlayerCameraSetting.FirstPlayerCameraRenderLayer
                            | 1 << LayerMask.NameToLayer(TiyaTools.TiyaLayers.Player)
                            | 1 << LayerMask.NameToLayer(TiyaTools.TiyaLayers.PlayerWeapon));
                    }
                    break;
                case CameraViewMode.Third:
                    firstViewCamera.gameObject.SetActive(false);
                    Camera.main.cullingMask |= (_firstViewPlayerCameraSetting.FirstPlayerCameraRenderLayer
                            | 1 << LayerMask.NameToLayer(TiyaTools.TiyaLayers.Player)
                            | 1 << LayerMask.NameToLayer(TiyaTools.TiyaLayers.PlayerWeapon));
                    break;
                default:
                    break;
            }
        }

        private Cinemachine.CinemachineBrain _cinemachineBrain;
        private VCInputProvider _freeLookVC_InputProvider;

        Transform _viewTransform;
        public override Transform ViewTransform => _viewTransform;

        public IEnable ControllerEnabler { get; private set; }

        public float FieldOfView
        {
            get => _freeLookVC.m_Lens.FieldOfView;
            set => _freeLookVC.m_Lens.FieldOfView = value;
        }

        // 视角控制辅助
        private VCFollowCtrl _followCtrl;

        #region Unity Events
        protected new void Awake()
        {
            base.Awake();

            _cinemachineBrain = GetComponentInChildren<Cinemachine.CinemachineBrain>();

            _freeLookVC_InputProvider = _freeLookVC.GetComponent<VCInputProvider>() ?? _freeLookVC.gameObject.AddComponent<VCInputProvider>();

            // 创建并初始化 _viewTransform
            _viewTransform = new GameObject("_Player View Tranform").transform;
            _viewTransform.parent = transform;

            // 创建并初始化 _followCtrl
            if (!_followCtrl)
            {
                _followCtrl = new GameObject(nameof(_followCtrl), typeof(VCFollowCtrl)).GetComponent<VCFollowCtrl>();
                _followCtrl.ConnectedObj = ViewTransform.gameObject;
                _followCtrl.transform.parent = transform;
            }

            ControllerEnabler = new Utility.TiyaEnableAgent(EnableController, DisableController);
            void EnableController()
            {
                if (_XY_Asix)
                {
                    _XY_Asix.action.Enable();
                }
                if (_Z_Asix)
                {
                    _Z_Asix.action.Enable();
                }
            }
            void DisableController()
            {
                if (_XY_Asix)
                {
                    _XY_Asix.action.Disable();
                }
                if (_Z_Asix)
                {
                    _Z_Asix.action.Disable();
                }
            }
        }

        protected new void Start()
        {
            base.Start();

            _freeLookVC.Follow = _followCtrl.transform;
            _freeLookVC.LookAt = LookAt.transform;
            _freeLookVC_InputProvider.XYAxis = _XY_Asix;
            _freeLookVC_InputProvider.ZAxis = _Z_Asix;

            Mode = _mode; // 执行 setter

            OnLock += OnLockListener;
            OnUnlock += OnUnlockListener;
        }

        void OnLockListener(GameObject obj)
        {
            ControllerEnabler.Disable();

            _freeLookVC.m_XAxis.Value = 0;
            _followCtrl.transform.rotation = Quaternion.LookRotation(ViewTransform.forward);

            _followCtrl.LockTarget = obj;
        }

        void OnUnlockListener(GameObject obj)
        {
            ControllerEnabler.Enable();

            _followCtrl.LockTarget = null;
        }

        private void OnEnable()
        {
            ControllerEnabler.Enable();
        }

        private void OnDisable()
        {
            ControllerEnabler.Disable();
        }

        private void Update()
        {
            // 同步 ViewTransform 与 CameraTransform
            ViewTransform.rotation = _cinemachineBrain.transform.rotation;
            ViewTransform.position = base.ViewTransform.position;

            if (IsLocked)
            {
                // 锁定时，插值移动 freeVC 到 MiddleRig (Y_Value = 0.5) 位置
                _freeLookVC.m_YAxis.Value = Mathf.Lerp(_freeLookVC.m_YAxis.Value, 0.5f, 20 * Time.deltaTime);
                // 保证 X_Value = 0
                _freeLookVC.m_XAxis.Value = 0;
            }

            // 第一人称，保证 Follow forward 与摄像机相同
            // TODO: 改为 lookAt forward 与之相同，然后再 chrtController 里控制上半身 / 头朝向 lookAt
            if (Mode == CameraViewMode.First)
            {
                var forward = Vector3.ProjectOnPlane(ViewTransform.forward, Vector3.up);
                Actor.TurnTo(forward);
                //Actor.ActorTransform.rotation = Quaternion.LookRotation(forward);

                // 同步 _firstViewPlayerCamera 位置
                if (_enableFirstViewPlayerCamera)
                {
                    FirstViewPlayerCamera.transform.position = FirstViewPlayerCameraFollow.position;
                    FirstViewPlayerCamera.transform.rotation = FirstViewPlayerCameraFollow.rotation;
                }
            }
        }
        #endregion

        // 这个方法逻辑内置于 FreeLook 中了
        public new void View(Vector2 direction)
        {
        }

        public new void TempRotate(float pitchAngle)
        {
            _followCtrl.transform.RotateAround(_followCtrl.transform.position, _cinemachineBrain.transform.right, pitchAngle);
        }

        /// <summary>
        /// 读取一个视角配置
        /// </summary>
        /// <param name="configuration"></param>
        public void LoadConfiguration(TiyaActorCameraViewConfiguration configuration)
        {
            _freeLookVC.m_Orbits[0].m_Radius = configuration.TopRig.Radius;
            _freeLookVC.m_Orbits[0].m_Height = configuration.TopRig.Height;
            _freeLookVC.m_Orbits[1].m_Radius = configuration.MiddleRig.Radius;
            _freeLookVC.m_Orbits[1].m_Height = configuration.MiddleRig.Height;
            _freeLookVC.m_Orbits[2].m_Radius = configuration.BottomRig.Radius;
            _freeLookVC.m_Orbits[2].m_Height = configuration.BottomRig.Height;

            _freeLookVC.m_XAxis.m_MaxSpeed = configuration.X_Speed;
            _freeLookVC.m_YAxis.m_MaxSpeed = configuration.Y_Speed;
        }

        #region InnerTypes
        [System.Serializable]
        struct FirstViewPlayerCameraSetting
        {
            [SerializeField] Camera _firstViewPlayerCamera;
            [SerializeField] Transform _firstViewPlayerCameraFollow;
            [SerializeField] LayerMask _firstPlayerCameraRenderLayer;

            public Camera FirstViewPlayerCamera { get => _firstViewPlayerCamera; set => _firstViewPlayerCamera = value; }
            public Transform FirstViewPlayerCameraFollow { get => _firstViewPlayerCameraFollow; set => _firstViewPlayerCameraFollow = value; }
            public LayerMask FirstPlayerCameraRenderLayer { get => _firstPlayerCameraRenderLayer; set => _firstPlayerCameraRenderLayer = value; }
        }
        #endregion
    }
}
