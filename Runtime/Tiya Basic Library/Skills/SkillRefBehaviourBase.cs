using System;
using UnityEngine;

namespace Sarachan.UniTiya.Skill
{
    /// <summary>
    /// skill 的 Monobehaviour 形式，通过重写 <see cref="Skill"/> 实现。
    /// </summary>
    public abstract class SkillRefBehaviourBase<T> : MonoBehaviour, ISkill
        where T : ISkill
    {
        public abstract T Skill { get; }

        public bool Enabled { get => Skill.Enabled; set => Skill.Enabled = value; }

        public event Action OnPerforming
        {
            add
            {
                Skill.OnPerforming += value;
            }

            remove
            {
                Skill.OnPerforming -= value;
            }
        }

        public event Action OnPerformingFailed
        {
            add
            {
                Skill.OnPerformingFailed += value;
            }

            remove
            {
                Skill.OnPerformingFailed -= value;
            }
        }

        public event Action OnCanceling
        {
            add
            {
                Skill.OnCanceling += value;
            }

            remove
            {
                Skill.OnCanceling -= value;
            }
        }

        public event Action OnEnable
        {
            add
            {
                Skill.OnEnable += value;
            }

            remove
            {
                Skill.OnEnable -= value;
            }
        }

        public event Action OnDisable
        {
            add
            {
                Skill.OnDisable += value;
            }

            remove
            {
                Skill.OnDisable -= value;
            }
        }

        public void Disable() => Skill.Disable();
        public void Enable() => Skill.Enable();
        public bool TryToPerform() => Skill.TryToPerform();
        public void Cancel() => Skill.Cancel();
    }
}
