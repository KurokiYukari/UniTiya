using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sarachan.UniTiya.Utility
{
    /// <summary>
    /// 一个可配置的 IGameProperties 实现
    /// </summary>
    [CreateAssetMenu(menuName = TiyaTools.UniTiyaName + "/PropertyConfiguration")]
    public class GamePropertyConfiguration : ScriptableObject, 
        IGameProperties, ISerializationCallbackReceiver
    {
        // 这两个字段都用于给其 Editor 提供信息
        [SerializeField] [HideInInspector] internal bool _hasConflict;
        [SerializeField] [HideInInspector] internal string _conflictName;

        [System.Serializable]
        public class TiyaGameFixedPropertyDictionary : SerializableDictionary<string, TiyaGameFixedNumericalProperty> { }
        [SerializeField] TiyaGameFixedPropertyDictionary _gameFixedNumProperty;

        [System.Serializable]
        public class TiyaGameDynamicPropertyDictionary : SerializableDictionary<string, TiyaGameDynamicNumericalProperty> { }
        [SerializeField] TiyaGameDynamicPropertyDictionary _gameDynamicNumProperty;

        [System.Serializable]
        public struct StringProperty
        {
            public string value;
            public bool isReadonly;
        }
        [System.Serializable]
        public class StringPropertyDictionary : SerializableDictionary<string, StringProperty> { }
        [SerializeField] StringPropertyDictionary _stringProperty;

        [System.Serializable]
        public struct IntProperty
        {
            public int value;
            public bool isReadonly;
        }
        [System.Serializable]
        public class IntPropertyDictionary : SerializableDictionary<string, IntProperty> { }
        [SerializeField] IntPropertyDictionary _intProperty;

        [System.Serializable]
        public struct FloatProperty
        {
            public float value;
            public bool isReadonly;
        }
        [System.Serializable]
        public class FloatPropertyDictionary : SerializableDictionary<string, FloatProperty> { }
        [SerializeField] FloatPropertyDictionary _floatProperty;

        [System.Serializable]
        public struct UnityObjectProperty
        {
            public Object value;
            public bool isReadonly;
        }
        [System.Serializable]
        public class ObjectPropertyDictionary : SerializableDictionary<string, UnityObjectProperty> { }
        [SerializeField] ObjectPropertyDictionary _unityObjectProperty;

        RuntimePropertyConfiguration _runtimeProperties;
        RuntimePropertyConfiguration RuntimeProperties => _runtimeProperties ??= new RuntimePropertyConfiguration();

        public object this[string propertyName]
        {
            get => RuntimeProperties[propertyName];
            set => RuntimeProperties[propertyName] = value;
        }

        public T GetProperty<T>(string propertyName, out bool isReadonly) =>
            RuntimeProperties.GetProperty<T>(propertyName, out isReadonly);

        public T GetProperty<T>(string propertyName) => GetProperty<T>(propertyName, out _);

        public bool AddProperty(string propertyName, object value, bool isReadOnly = false) =>
            RuntimeProperties.AddProperty(propertyName, value, isReadOnly);

        public bool RemoveProperty(string propertyName) => RuntimeProperties.RemoveProperty(propertyName);

        public bool ContainsProperty(string propertyName) => RuntimeProperties.ContainsProperty(propertyName);

        public bool IsReadonlyProperty(string propertyName) => RuntimeProperties.IsReadonlyProperty(propertyName);

        public void SetProperty(string propertyName, object value, bool isReadOnly = false) =>
            RuntimeProperties.SetProperty(propertyName, value, isReadOnly);

        public int Count => RuntimeProperties.Count;

        public IEnumerator<(string propertyName, object propertyValue, bool isReadonly)> GetEnumerator()
        {
            foreach (var (propertyName, propertyValue, isReadonly) in RuntimeProperties)
            {
                yield return (propertyName, propertyValue, isReadonly);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            RuntimeProperties.Clear();
            _hasConflict = false;

            IEnumerable<(string name, object value, bool isReadonly)> propertyQuery = 
                from property in _gameFixedNumProperty select (property.Key, (object)property.Value, false);
            propertyQuery = propertyQuery.Concat(
                from property in _gameDynamicNumProperty select (property.Key, (object)property.Value, false));
            propertyQuery = propertyQuery.Concat(
                from property in _stringProperty select (property.Key, (object)property.Value.value, property.Value.isReadonly));
            propertyQuery = propertyQuery.Concat(
                from property in _intProperty select (property.Key, (object)property.Value.value, property.Value.isReadonly));
            propertyQuery = propertyQuery.Concat(
                from property in _floatProperty select (property.Key, (object)property.Value.value, property.Value.isReadonly));
            propertyQuery = propertyQuery.Concat(
                from property in _unityObjectProperty select (property.Key, (object)property.Value.value, property.Value.isReadonly));

            foreach (var (name, value, isReadonly) in propertyQuery)
            {
                if (!AddProperty(name, value, isReadonly))
                {
                    _hasConflict = true;
                    _conflictName = name;
                }
            }
        }
    }

    public sealed class RuntimePropertyConfiguration 
        : IGameProperties
    {
        readonly Dictionary<string, (object value, bool isReadonly)> _propertyDictionary = new Dictionary<string, (object value, bool isReadonly)>();

        public void SetPropertiesFrom(IGameProperties properties)
        {
            foreach (var (propertyName, propertyValue, isReadonly) in properties)
            {
                SetProperty(propertyName, propertyValue, isReadonly);
            }
        }

        public object this[string propertyName]
        {
            get => GetProperty<object>(propertyName);
            set
            {
                if (_propertyDictionary.ContainsKey(propertyName))
                {
                    var tuple = _propertyDictionary[propertyName];
                    if (tuple.isReadonly)
                    {
                        throw new System.InvalidOperationException($"Set property {propertyName}'s value failed. This property is readonly.");
                    }
                    else
                    {
                        tuple.value = value;
                        _propertyDictionary[propertyName] = tuple;
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"PropertyConfiguration doesn't contain a property named {propertyName}");
                }
            }
        }

        public T GetProperty<T>(string propertyName, out bool isReadonly)
        {
            try
            {
                object value;
                (value, isReadonly) = _propertyDictionary[propertyName];
                return (T)value;
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"PropertyConfiguration doesn't contain a property named {propertyName}");
            }
            catch (System.InvalidCastException)
            {
                throw new System.InvalidCastException($"Can't convert property {propertyName} to type {typeof(T)}");
            }
        }

        public T GetProperty<T>(string propertyName) => GetProperty<T>(propertyName, out _);

        public bool AddProperty(string propertyName, object value, bool isReadOnly = false)
        {
            if (_propertyDictionary.ContainsKey(propertyName))
            {
                return false;
            }
            else
            {
                _propertyDictionary[propertyName] = (value, isReadOnly);
                return true;
            }
        }

        public void SetProperty(string propertyName, object value, bool isReadOnly = false) =>
            _propertyDictionary[propertyName] = (value, isReadOnly);

        public bool RemoveProperty(string propertyName) => _propertyDictionary.Remove(propertyName);

        public bool ContainsProperty(string propertyName) => _propertyDictionary.ContainsKey(propertyName);

        public bool IsReadonlyProperty(string propertyName)
        {
            try
            {
                (_, var isReadonly) = _propertyDictionary[propertyName];
                return isReadonly;
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"PropertyConfiguration doesn't contain a property named {propertyName}");
            }
        }

        public int Count => _propertyDictionary.Count;

        public void Clear() => _propertyDictionary.Clear();

        public IEnumerator<(string propertyName, object propertyValue, bool isReadonly)> GetEnumerator()
        {
            foreach (var pair in _propertyDictionary)
            {
                yield return (pair.Key, pair.Value.value, pair.Value.isReadonly);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
