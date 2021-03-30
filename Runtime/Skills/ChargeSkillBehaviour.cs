using UnityEngine;

using UniRx;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// <see cref="ChargeDecoratorSkill"/> 的 Behaviour 引用。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Skill/Charge Decotator Skill")]
    public class ChargeSkillBehaviour : SkillRefBehaviourBase<ChargeDecoratorSkill>
    {
        [SerializeField] ChargeDecoratorSkill _chargeSkill;
        public override ChargeDecoratorSkill Skill => _chargeSkill;
    }

    // TODO: 改为继承新的 Decorator 类，重写 TryPerform 解决 consumer 问题
    [System.Serializable]
    public class ChargeDecoratorSkill : TiyaSkill
    {
        // 请谨慎配置 _decoratedSkill 的 consumer
        // 注意 Decorator 不会对 _decoratedSkill 进行 init
        [SerializeField] [TypeRestriction(typeof(ISkill))] Object _decoratedSkillObject;

        ISkill _decoratedSkill;
        public ISkill DecoratedSkill
        {
            get => _decoratedSkill ??= _decoratedSkillObject.ConvertTo<ISkill>();
            set => _decoratedSkill = value;
        }

        [Tooltip("Skill will auto Cancel when charge time is larger than MaxChargeTime. \n" +
            "This field only takes effect when its value is larger than 0.")]
        [SerializeField] float _maxChargeTime = 0;

        public event System.Action<ISkill, float> OnBeforeChargePerform;
        public event System.Action<ISkill, float> OnAfterChargePerform;

        float _chargeTime;
        public float ChargeTime
        {
            get => _chargeTime > _maxChargeTime ? _maxChargeTime : _chargeTime;
            set => _chargeTime = value;
        }

        void ChargePerform()
        {
            OnBeforeChargePerform?.Invoke(DecoratedSkill, ChargeTime);
            DecoratedSkill.TryToPerform();
            OnAfterChargePerform?.Invoke(DecoratedSkill, ChargeTime);
        }

        System.IDisposable _chargeSubscribe;
        void PerformAction()
        {
            if (_chargeSubscribe == null)
            {
                ChargeTime = 0;

                var updateStream = Observable.EveryUpdate();
                if (_maxChargeTime > 0)
                {
                    updateStream = updateStream.TakeUntil(Observable.Timer(System.TimeSpan.FromSeconds(_maxChargeTime)));
                }
                updateStream.Subscribe(_ => ChargeTime += Time.deltaTime, ChargePerform);
            }
        }
        void CancelAction()
        {
            if (_chargeSubscribe != null)
            {
                _chargeSubscribe.Dispose();
                _chargeSubscribe = null;
            }
        }

        protected override void OnInit()
        {
            OnPerforming += PerformAction;
            OnCanceling += CancelAction;
        }
    }
}
