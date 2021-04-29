using System.Collections.Generic;
using UnityEngine;

using UniRx;

namespace Sarachan.UniTiya.Skill
{
    [AddComponentMenu("")]
    public class WeaponMeleeSkillAnimationEventsHandler : MonoBehaviour, IMeleeWeaponAnimationEvents
    {
        public WeaponAnimatedMeleeSkill Skill { get; set; }

        readonly List<Vector3> _lastLinecastPositions = new List<Vector3>();
        readonly HashSet<GameObject> _onceAttackedDamageableGameObjects = new HashSet<GameObject>();
        System.IDisposable _checkingAttackPathSubscribe;
        public void AE_OnBeginOnceAttack()
        {
            _lastLinecastPositions.Clear();
            _onceAttackedDamageableGameObjects.Clear();

            CheckingAttackPathListener(Time.frameCount);
            _checkingAttackPathSubscribe ??= Observable.IntervalFrame((int)Skill.CheckAttackingPathFrameInterval)
                .Subscribe(CheckingAttackPathListener);
        }

        public void AE_OnOnceAttackEnd()
        {
            _lastLinecastPositions.Clear();
            _onceAttackedDamageableGameObjects.Clear();

            _checkingAttackPathSubscribe?.Dispose();
            _checkingAttackPathSubscribe = null;
        }

        readonly RaycastHit[] _raycastHitsBuffer = new RaycastHit[8];
        void CheckingAttackPathListener(long frame)
        {
            var currentLinecastPositions = Skill.LinecastPositions;
            var currentLinecastPositionsEnumerator = currentLinecastPositions.GetEnumerator();
            currentLinecastPositionsEnumerator.MoveNext();
            if (_lastLinecastPositions != null)
            {
                foreach (var lastPosition in _lastLinecastPositions)
                {
                    var currentPosition = currentLinecastPositionsEnumerator.Current;
                    currentLinecastPositionsEnumerator.MoveNext();

                    if (Skill.ShowLinecastPath)
                    {
                        Debug.DrawLine(lastPosition, currentPosition, Color.red, 1f);
                    }

                    var distance = (currentPosition - lastPosition).magnitude;
                    int count = Physics.RaycastNonAlloc(lastPosition, currentPosition - lastPosition, _raycastHitsBuffer, distance);

                    for (int i = 0; i < count; i++)
                    {
                        var collider = _raycastHitsBuffer[i].collider;
                        if (!_onceAttackedDamageableGameObjects.Contains(collider.gameObject))
                        {
                            var damageSource = Skill.DamageSource;
                            if (damageSource.DoDamageTo(collider.gameObject))
                            {
                                _onceAttackedDamageableGameObjects.Add(collider.gameObject);
                            }
                        }
                    }
                }
            }

            _lastLinecastPositions.Clear();
            _lastLinecastPositions.AddRange(currentLinecastPositions);
        }
    }

    public interface IMeleeWeaponAnimationEvents
    {
        /// <summary>
        /// 重置攻击判定，应该在每一次独立的攻击开始时被动画事件触发。
        /// </summary>
        void AE_OnBeginOnceAttack();

        ///// <summary>
        ///// 执行攻击判定检测
        ///// </summary>
        //void AE_OnCheckingAttackPath();

        /// <summary>
        /// 重置攻击判定，应该在每一次独立的攻击动画的末尾被动画事件触发。
        /// </summary>
        void AE_OnOnceAttackEnd();
    }
}
