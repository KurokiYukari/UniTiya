using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UniRx;

namespace Sarachan.UniTiya.Commands
{
    /// <summary>
    /// 基础的命令处理器，在每次 Update 顺序执行命令队列里所有的命令。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicCommandProcessor<T> : ICommandProcessor<T>, IEnable
    {
        readonly T _context; // 要使用该 Processor 的 Component

        readonly Queue<ICommand<T>> _commandQueue = new Queue<ICommand<T>>(8);

        readonly Utility.TiyaEnableAgent _enableAgent;

        public BasicCommandProcessor(T context)
        {
            _context = context;

            _enableAgent = new Utility.TiyaEnableAgent(OnEnableAction, OnDisableAction);
        }

        public event Action BeforeHandleCommands;
        public event Action AfterHandleCommands;

        public event Action OnEnable
        {
            add
            {
                _enableAgent.OnEnable += value;
            }

            remove
            {
                _enableAgent.OnEnable -= value;
            }
        }

        public event Action OnDisable
        {
            add
            {
                _enableAgent.OnDisable += value;
            }

            remove
            {
                _enableAgent.OnDisable -= value;
            }
        }

        public bool Enabled
        {
            get => _enableAgent.Enabled;
            set => _enableAgent.Enabled = value;
        }

        public void Enable() => _enableAgent.Enable();
        public void Disable() => _enableAgent.Disable();

        IDisposable _handleCommandSubscribe = null;
        void OnEnableAction()
        {
            if (_handleCommandSubscribe == null)
            {
                _handleCommandSubscribe = Observable.EveryUpdate()
                    .Subscribe(_ => OnHandleCommand());
            }
        }
        void OnDisableAction()
        {
            if (_handleCommandSubscribe != null)
            {
                _handleCommandSubscribe.Dispose();
                _handleCommandSubscribe = null;
            }
        }

        void OnHandleCommand()
        {
            BeforeHandleCommands?.Invoke();
            while (_commandQueue.Count != 0)
            {
                _commandQueue.Dequeue().Execute(_context);
            }
            AfterHandleCommands?.Invoke();
        }

        public void AddCommand(ICommand<T> cmd) => _commandQueue.Enqueue(cmd);
    }
}
