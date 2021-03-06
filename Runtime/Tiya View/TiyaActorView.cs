using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using UniRx;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.TiyaView
{
    /// <summary>
    /// IActorView 的实现
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/View/Tiya Actor View")]
    public class TiyaActorView : MonoBehaviour, IActorView
    {
        const float _SLERP_THREDHOLD = 2f;

        [TypeRestriction(typeof(IActorController))]
        [SerializeField] Object _actorObject;
        IActorController _actor;
        public IActorController Actor => _actor ??= _actorObject.ConvertTo<IActorController>();

        [SerializeField] Transform _viewTransfrom;

        [Header("Lock Setting")]
        [SerializeField] LayerMask _lockableLayer;
        [SerializeField] Transform[] _selfLockables; // TODO: 改为 Serialized HashSet？
        readonly HashSet<GameObject> _selfLockableHashSet = new HashSet<GameObject>();

        [SerializeField] float _lockSlerpRatio = 10;

        [Tooltip("锁定的半范围")]
        [SerializeField] Vector3 _lockHalfExtents = new Vector3(2f, 2f, 10f);

        [SerializeField] ViewLockEvent _onLock;
        [SerializeField] ViewLockEvent _onUnlock;

        [System.Serializable]
        class ViewLockEvent : UnityEvent<IActorView, GameObject> { }

        public virtual Transform ViewTransform => _viewTransfrom;

        Quaternion _viewInitialLocalRotation;

        public bool IsLocked => LockTarget != null;
        public GameObject LockTarget { get; private set; }

        /// <summary>
        /// 锁定的半范围
        /// </summary>
        public Vector3 LockHalfExtents { get => _lockHalfExtents; set => _lockHalfExtents = value; }

        public event System.Action<GameObject> OnLock;
        public event System.Action<GameObject> OnUnlock;

        protected void Awake()
        {
            _viewInitialLocalRotation = _viewTransfrom.localRotation;
            foreach (var selfLockable in _selfLockables)
            {
                _selfLockableHashSet.Add(selfLockable.gameObject);
            }
        }

        protected void Start()
        {
            Actor.OnDie += () =>
            {
                foreach (var selfLockable in _selfLockableHashSet)
                {
                    selfLockable.SetActive(false);
                }
            };
        }

        public void View(Vector2 deltaRotation)
        {
            if (IsLocked)
            {
                return;
            }

            var deltaEular = new Vector3(-deltaRotation.y, deltaRotation.x, 0);

            ViewTransform.rotation = Quaternion.Euler(deltaEular) * ViewTransform.rotation;
        }

        public void ResetView()
        {
            if (IsLocked)
            {
                return;
            }

            ViewTransform.localRotation = _viewInitialLocalRotation;
        }

        public void Lock(LockCmdType lockCmdType, System.Predicate<GameObject> predicate = null)
        {
            switch (lockCmdType)
            {
                case LockCmdType.Lock:
                    if (!IsLocked)
                    {
                        LockTo(FindLockTargets(predicate).FirstOrDefault());
                    }
                    break;
                case LockCmdType.Unlock:
                    UnLock();
                    break;
                case LockCmdType.LockToNextTarget:
                    ChangeLockTarget(predicate: predicate);
                    break;
                case LockCmdType.LockToPreTarget:
                    ChangeLockTarget(true, predicate);
                    break;
                default:
                    break;
            }
        }

        public void TempRotate(float pitchAngle)
        {
            ViewTransform.RotateAround(ViewTransform.position, ViewTransform.right, pitchAngle);
        }

        void ChangeLockTarget(bool lockToPre = false, System.Predicate<GameObject> predicate = null)
        {
            if (!IsLocked)
            {
                return;
            }

            var targets = FindLockTargets(predicate).ToList();

            if (targets.Count == 0)
            {
                return;
            }

            int index = targets.FindIndex(item => item.Equals(LockTarget));
            if (index == -1)
            {
                LockTo(targets.FirstOrDefault());
                return;
            }

            var targetIndex = lockToPre ? index - 1 : index + 1;
            targetIndex = targetIndex >= 0 ? targetIndex % targets.Count : targetIndex + targets.Count;

            LockTo(targets[targetIndex]);
        }

        public IEnumerable<GameObject> FindLockTargets(System.Predicate<GameObject> predicate = null)
        {
            Vector3 center = ViewTransform.position + ViewTransform.forward * _lockHalfExtents.z;

            var cols = Physics.OverlapBox(center, _lockHalfExtents, ViewTransform.rotation, _lockableLayer);
            var orderedObjs = from col in cols
                              where (predicate == null || predicate(col.gameObject)) && !_selfLockableHashSet.Contains(col.gameObject)
                              orderby Vector3.Distance(col.transform.position, ViewTransform.position) ascending
                              select col.gameObject;

            return orderedObjs;
        }

        System.IDisposable _lockSubscribe; // lock 时保持 view 指向 lockTarget 的 Subscribe
        public void LockTo(GameObject target)
        {
            if (target == null)
            {
                UnLock();
                return;
            }

            if (target == LockTarget)
            {
                return;
            }

            LockTarget = target;

            // Subscribe observer
            _lockSubscribe?.Dispose();
            _lockSubscribe = Observable.EveryLateUpdate().TakeUntilDisable(this)
                .Subscribe(OnLockingLateUpdate);

            // Invoke OnLock
            _onLock.Invoke(this, target);
            OnLock?.Invoke(target);
        }
        void UnLock()
        {
            if (!IsLocked)
            {
                return;
            }

            LockTarget = null;

            _lockSubscribe?.Dispose();

            // Invoke OnUnLock
            _onUnlock.Invoke(this, LockTarget);
            OnUnlock?.Invoke(LockTarget);
        }

        /// <summary>
        /// 锁定时每帧的回调
        /// </summary>
        /// <param name="frameCount"></param>
        protected virtual void OnLockingLateUpdate(long frameCount)
        {
            if (IsLocked)
            {
                if (LockTarget.activeInHierarchy)
                {
                    var q = Quaternion.LookRotation(LockTarget.transform.position - transform.position);
                    if (Quaternion.Angle(transform.rotation, q) < _SLERP_THREDHOLD) // 防止小距离移动的抖动
                    {
                        transform.rotation = q;
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, q, _lockSlerpRatio * Time.deltaTime);
                    }
                }
                else
                {
                    UnLock();
                }
            }
        }
    }
}