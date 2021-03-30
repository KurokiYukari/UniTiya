using System;
using System.Collections.Generic;
using System.Linq;

namespace Sarachan.UniTiya.Utility
{
    /// <summary>
    /// 一个实现了 <see cref="IEnable"/> 接口基础逻辑的代理类，通过配置其 event 来实现具体的功能。
    /// </summary>
    public sealed class TiyaEnableAgent : IEnable
    {
        bool _enabled = false;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
        }

        public event Action OnEnable;
        public event Action OnDisable;

        public TiyaEnableAgent() { }

        public TiyaEnableAgent(Action onEnable, Action onDisable)
        {
            if (onEnable != null)
            {
                OnEnable += onEnable;
            }
            if (onDisable != null)
            {
                OnDisable += onDisable;
            }
        }

        public TiyaEnableAgent(IEnumerable<Action> onEnableActions, IEnumerable<Action> onDisableActions)
        {
            if (onEnableActions != null)
            {
                foreach (var action in onEnableActions)
                {
                    if (action != null)
                    {
                        OnEnable += action;
                    }
                }
            }
            if (onDisableActions != null)
            {
                foreach (var action in onDisableActions)
                {
                    if (action != null)
                    {
                        OnDisable += action;
                    }
                }
            }
        }

        public void Enable()
        {
            if (_enabled != true)
            {
                _enabled = true;
                OnEnable?.Invoke();
            }
        }
        public void Disable()
        {
            if (_enabled != false)
            {
                _enabled = false;
                OnDisable?.Invoke();
            }
        }
    }
}
