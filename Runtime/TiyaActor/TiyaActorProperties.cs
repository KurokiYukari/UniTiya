using UnityEngine;

using Sarachan.UniTiya.Utility;
using System.Collections.Generic;
using System.Collections;

namespace Sarachan.UniTiya.TiyaActor
{
    // TODO: 改为 Scriptable Object
    public class TiyaActorProperties : MonoBehaviour, IActorProperties
    {
        [SerializeField] string _actorName;

        [SerializeField] TiyaGameDynamicNumericalProperty _actorHP;
        [SerializeField] TiyaGameFixedNumericalProperty _walkSpeed;
        [SerializeField] TiyaGameFixedNumericalProperty _runSpeed;
        [SerializeField] TiyaGameFixedNumericalProperty _sprintSpeed;
        [SerializeField] TiyaGameFixedNumericalProperty _jumpInitialSpeed;

        [SerializeField] GamePropertyConfiguration _actorExtraProperties;

        readonly RuntimePropertyConfiguration _runtimeProperties = new RuntimePropertyConfiguration();

        public string ActorName => GetProperty<string>(nameof(ActorName));
        public IGameDynamicNumericalProperty ActorHP => GetProperty<TiyaGameDynamicNumericalProperty>(nameof(ActorHP));
        public IGameFixedNumericalProperty WalkSpeed => GetProperty<TiyaGameFixedNumericalProperty>(nameof(WalkSpeed));
        public IGameFixedNumericalProperty RunSpeed => GetProperty<TiyaGameFixedNumericalProperty>(nameof(RunSpeed));
        public IGameFixedNumericalProperty SprintSpeed => GetProperty<TiyaGameFixedNumericalProperty>(nameof(SprintSpeed));
        public IGameFixedNumericalProperty JumpInitialSpeed => GetProperty<TiyaGameFixedNumericalProperty>(nameof(JumpInitialSpeed));

        private void Awake()
        {
            if (_actorExtraProperties)
            {
                _runtimeProperties.SetPropertiesFrom(_actorExtraProperties);
            }

            SetProperty(nameof(ActorName), _actorName, true);
            SetProperty(nameof(ActorHP), _actorHP, true);
            SetProperty(nameof(WalkSpeed), _walkSpeed, true);
            SetProperty(nameof(RunSpeed), _runSpeed, true);
            SetProperty(nameof(SprintSpeed), _sprintSpeed, true);
            SetProperty(nameof(JumpInitialSpeed), _jumpInitialSpeed, true);
        }

        private void Start()
        {
            ActorHP.Value = ActorHP.MaxValue;
        }

        public object this[string propertyName]
        {
            get => _runtimeProperties[propertyName];
            set => _runtimeProperties[propertyName] = value;
        }

        public bool AddProperty(string propertyName, object value, bool isReadOnly = false) =>
            _runtimeProperties.AddProperty(propertyName, value, isReadOnly);

        public bool ContainsProperty(string propertyName) =>
            _runtimeProperties.ContainsProperty(propertyName);

        public T GetProperty<T>(string propertyName) =>
            _runtimeProperties.GetProperty<T>(propertyName);

        public T GetProperty<T>(string propertyName, out bool isReadonly) =>
            _runtimeProperties.GetProperty<T>(propertyName, out isReadonly);

        public bool IsReadonlyProperty(string propertyName) =>
            _runtimeProperties.IsReadonlyProperty(propertyName);

        public bool RemoveProperty(string propertyName) =>
            _runtimeProperties.RemoveProperty(propertyName);

        public void SetProperty(string propertyName, object value, bool isReadOnly = false) =>
            _runtimeProperties.SetProperty(propertyName, value, isReadOnly);

        public IEnumerator<(string propertyName, object propertyValue, bool isReadonly)> GetEnumerator() =>
            _runtimeProperties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _runtimeProperties.GetEnumerator();
    }
}
