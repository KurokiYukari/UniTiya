using UnityEngine;
using UnityEngine.Events;

using Sarachan.UniTiya.Consumer;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// <see cref="TiyaSkill"/> 的 Behaviour 引用。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Skill/Tiya Skill")]
    public sealed class TiyaSkillBehaviour : SkillRefBehaviourBase<TiyaSkill>
    {
        [SerializeField] TiyaSkill _skill;

        public override TiyaSkill Skill => _skill;
    }

    /// <summary>
    /// 可配置的 Skill，可以通过序列化配置，或者继承重写 <see cref="OnInit"/>，
    /// 在其中配置事件、consumer 等来实现自定义的 Skill。
    /// 初始状态下，Skill 是 Disable 的，需要先调用 <see cref="Enable"/> 才能使用。
    /// </summary>
    [System.Serializable]
    public class TiyaSkill : ISkill
    {
        [SerializeField] GameObject _skillPerformer;

        [SerializeField] MultiConsumer _skillConsumers;

        [SerializeField] SkillEvent _onPerforming;
        [SerializeField] SkillEvent _onCanceling;

        [System.Serializable]
        class SkillEvent : UnityEvent<ISkill> { }
        
        public GameObject SkillPerformer
        {
            get => _skillPerformer;
            set => _skillPerformer = value;
        }

        readonly Utility.TiyaEnableAgent _enableAgent = new Utility.TiyaEnableAgent();

        bool _isInitialized = false;
        bool _canCancel = false;

        public bool Enabled { get => _enableAgent.Enabled; set => _enableAgent.Enabled = value; }

        public event System.Action OnEnable { add => _enableAgent.OnEnable += value; remove => _enableAgent.OnEnable -= value; }
        public event System.Action OnDisable { add => _enableAgent.OnDisable += value; remove => _enableAgent.OnDisable -= value; }

        public MultiConsumer SkillConsumers => _skillConsumers;

        public event System.Action OnPerforming;
        public event System.Action OnCanceling;

        public bool TryToPerform()
        {
            if (Enabled && SkillConsumers.CanConsume)
            {
                SkillConsumers.Consume();

                OnPerforming?.Invoke();
                _onPerforming.Invoke(this);

                _canCancel = true;
                return true;
            }

            return false;
        }

        public void Cancel()
        {
            if (Enabled && _canCancel)
            {
                OnCanceling?.Invoke();
                _onCanceling.Invoke(this);

                _canCancel = false;
            }
        }

        public void Enable() 
        {
            Init();
            _enableAgent.Enable();
        }
        public void Disable() => _enableAgent.Disable();

        void Init()
        {
            if (!_isInitialized)
            {
                OnDisable += Cancel;

                OnInit();

                _isInitialized = true;
            }
        }
        protected virtual void OnInit() { }
    }
}
