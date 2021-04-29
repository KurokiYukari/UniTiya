using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaView
{
    /// <summary>
    /// 辅助 Behaviour，由 <see cref="TiyaActorCameraView"/> 动态创建
    /// </summary>
    [AddComponentMenu("")]
    class VCFollowCtrl : MonoBehaviour
    {
        private const float _SLERP_THREDHOLD = 2f;

        public GameObject ConnectedObj;
        public GameObject LockTarget;

        private TiyaActorCameraView _viewController;
        public TiyaActorCameraView ViewController => _viewController ??= transform.parent.GetComponent<TiyaActorCameraView>();

        // TODO: 在 ViewController 中开放接口
        private float _rotateSpeed = 10f;
        public float RotateSpeed { get => _rotateSpeed; set => _rotateSpeed = Mathf.Abs(value); }

        private void Start()
        {
            transform.rotation = ConnectedObj.transform.rotation;
        }

        private void LateUpdate()
        {
            transform.position = ConnectedObj.transform.position;

            // 锁定时，追踪指向 LockTarget
            if (LockTarget)
            {
                var q = Quaternion.LookRotation(LockTarget.transform.position - transform.position, Vector3.up);
                if (Quaternion.Angle(transform.rotation, q) < _SLERP_THREDHOLD) // 防止小距离移动的抖动
                {
                    transform.rotation = q;
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, q, RotateSpeed * Time.deltaTime);
                }
            }
            else
            {
                // 第三人称无锁定时，保持自身 up 指向世界 y 轴
                // 一般在取消锁定时发生。
                //if (ViewController.Mode == ViewController.ViewMode.Third && transform.up != Vector3.up)
                //{
                //    var endEular = transform.rotation.eulerAngles;
                //    endEular.x = 0;
                //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(endEular), RotateSpeed * Time.deltaTime);
                //}

                if (transform.up != Vector3.up)
                {
                    var endEular = transform.rotation.eulerAngles;
                    endEular.x = 0;
                    endEular.z = 0;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(endEular), RotateSpeed * Time.deltaTime);
                }
            }
        }
    }
}
