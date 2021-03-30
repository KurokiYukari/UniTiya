﻿using UnityEngine;
using UnityEngine.InputSystem;

using UniRx;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// <see cref="WeaponShootSkill"/> 的 Behaviour 引用。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Skill/Weapon Shoot Skill")]
    public sealed class WeaponShootSkillBehaviour : SkillRefBehaviourBase<WeaponShootSkill>
    {
        [SerializeField] private WeaponShootSkill _skill;
        public override WeaponShootSkill Skill => _skill;
    }

    /// <summary>
    /// 依赖于 <see cref="WeaponProjectileSkill"/> 的射击 WeaponSkill。
    /// 其 Perform 是根据当前射击模式进行射击，并且提供额外 Action 来切换模式。
    /// </summary>
    [System.Serializable]
    public class WeaponShootSkill : TiyaSkill
    {
        [TiyaPropertyAttributes.MultiEnum]
        [SerializeField] ProjectileMode _avaliableModes;
        [SerializeField] InputAction _changeModeTrigger;

        [SerializeField] private WeaponProjectileSkill _singleShootSkill;
        public WeaponProjectileSkill SingleShootSkill => _singleShootSkill;

        private IWeaponController _weapon = null;
        public IWeaponController Weapon => _weapon ??= SkillPerformer.GetComponent<IWeaponController>();

        private void EnableAllPlayerInput()
        {
            _changeModeTrigger.Enable();
        }
        private void DisableAllPlayerInput()
        {
            _changeModeTrigger.Disable();
        }

        public ProjectileMode CurrentMode { get; private set; } = 0;
        public InputAction ChangeModeTrigger => _changeModeTrigger;

        void EnableAction()
        {
            _singleShootSkill.Enable();

            if (Weapon.Owner.IsPlayer)
            {
                EnableAllPlayerInput();
            }
        }
        void DisableAction()
        {
            _singleShootSkill.Disable();

            if (Weapon.Owner.IsPlayer)
            {
                DisableAllPlayerInput();
            }

            StopAutoShoot();
        }

        void PerformAction()
        {
            switch (CurrentMode)
            {
                case ProjectileMode.Single:
                    SingleShoot();
                    break;
                case ProjectileMode.Triple:
                    TripleShoot();
                    break;
                case ProjectileMode.Auto:
                    StartAutoShoot();
                    break;
            }
        }
        void CancelAction()
        {
            StopAutoShoot();
        }

        protected override void OnInit()
        {
            _changeModeTrigger.performed += _ => ChangeMode();
            ChangeMode();

            OnEnable += EnableAction;
            OnDisable += DisableAction;

            OnPerforming += PerformAction;
            OnCanceling += CancelAction;
        }

        public void SingleShoot() => _singleShootSkill.TryToPerform();

        public void TripleShoot()
        {
            _singleShootSkill.TryToPerform();

            Observable.Interval(System.TimeSpan.FromSeconds(_singleShootSkill.ShotCDConsumer.ColdDownTime))
                .Take(2)
                .Subscribe(_ => _singleShootSkill.TryToPerform())
                .AddTo(SkillPerformer);
        }

        System.IDisposable _autoShootSubscribe;
        public void StartAutoShoot()
        {
            if (_autoShootSubscribe == null)
            {
                _singleShootSkill.TryToPerform();

                _autoShootSubscribe = Observable.EveryFixedUpdate()
                    .Subscribe(_ => _singleShootSkill.TryToPerform())
                    .AddTo(SkillPerformer);
            }
        }
        public void StopAutoShoot()
        {
            if (_autoShootSubscribe != null)
            {
                _autoShootSubscribe.Dispose();
                _autoShootSubscribe = null;
            }
        }

        public void ChangeMode()
        {
            ProjectileMode? firstMode = null;
            bool currentModeFlag = false;
            foreach (ProjectileMode mode in System.Enum.GetValues(typeof(ProjectileMode)))
            {
                if ((mode & _avaliableModes) != 0)
                {
                    if (firstMode == null)
                    {
                        firstMode = mode;
                    }

                    if (currentModeFlag)
                    {
                        CurrentMode = mode;
                        return;
                    }
                }

                if (mode == CurrentMode)
                {
                    currentModeFlag = true;
                }
            }

            CurrentMode = firstMode.Value;
            return;
        }
    }
}
