using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

using UniRx;

using Sarachan.UniTiya.Utility;
using Sarachan.UniTiya.Consumer;

namespace Sarachan.UniTiya.Skill
{
    [AddComponentMenu("/Skill/Ammunition Skill")]
    public class AmmunitionSkillBehaviour : SkillRefBehaviourBase<AmmunitionSkill>
    {
        [SerializeField] AmmunitionSkill _skill;
        public override AmmunitionSkill Skill => _skill;

        public int CurrentBulletsCount { get => _skill.CurrentBulletsCount; set => _skill.CurrentBulletsCount = value; }

#if UNITY_EDITOR
        [ContextMenu("CreateAmmunitionConsumer")]
        public void CreateAmmunitionConsumer()
        {
            var consumerReference = gameObject.AddComponent<IntConsumerBehaviour>();
            consumerReference.IntConsumer = new IntConsumer(this, nameof(CurrentBulletsCount));

            UnityEditor.EditorUtility.SetDirty(gameObject);
        }
#endif
    }

    /// <summary>
    /// 弹药控制 Skill，Perform 是进行 Reload。
    /// </summary>
    /// TODO: 将 Reloading 弹药的检测作为 Consumer，这样可以删去 OnBeginReloading 这个事件（同 OnPerforming
    [System.Serializable]
    public class AmmunitionSkill : TiyaSkill
    {
        [SerializeField] private IntPropertyValueSynchronizer _totleAmmunitionReference;
        [SerializeField] private int _magazineCapacity = 50;
        [SerializeField] private float _reloadTime = 2;
        [SerializeField] UnityEvent _onBeginReloading;
        [SerializeField] private UnityEvent _onCompleteReloading;

        [SerializeField] private int _currentBulletsCount = 0;

        public float ReloadTime { get => _reloadTime; set => _reloadTime = value; }

        public int CurrentBulletsCount { get => _currentBulletsCount; set => _currentBulletsCount = value; }
        public UnityEvent OnBeginReloading => _onBeginReloading;
        public UnityEvent OnCompleteReloading => _onCompleteReloading;

        void OnPerformAction()
        {
            var reloadAmmunition = CalculateReloadAmmunition();
            if (reloadAmmunition <= 0)
            {
                return;
            }

            OnBeginReloading.Invoke();
            Observable.Timer(System.TimeSpan.FromSeconds(ReloadTime))
                    .Subscribe(_ =>
                    {
                        if (!_totleAmmunitionReference.IsEmpty())
                        {
                            _totleAmmunitionReference.Value -= reloadAmmunition;
                        }
                        CurrentBulletsCount += reloadAmmunition;

                        OnCompleteReloading.Invoke();
                    });

            int CalculateReloadAmmunition()
            {
                var deltaAmmunition = _magazineCapacity - CurrentBulletsCount;
                if (_totleAmmunitionReference.IsEmpty())
                {
                    return deltaAmmunition;
                }
                else
                {
                    var totleAmmunition = _totleAmmunitionReference.Value;
                    if (totleAmmunition >= deltaAmmunition)
                    {
                        return deltaAmmunition;
                    }
                    else
                    {
                        return totleAmmunition;
                    }
                }
            }
        }

        protected override bool OnInit()
        {
            OnPerforming += OnPerformAction;
            return true;
        }
    }
}
