using System.Reflection;
using UnityEngine;

namespace Sarachan.UniTiya.Utility
{
    /// <summary>
    /// 可配置的属性同步器。通过反射寻找指定 Unity.Object 中的某个属性，通过 <see cref="Value"/> 进行同步。
    /// </summary>
    [System.Serializable]
    public class PropertyValueSynchronizer
    {
        [SerializeField] private Object _subjectOverride;
        [SerializeField] private string _propertyName;

        object _subjectObject;
        public object SubjectObject
        {
            get => _subjectObject ??= _subjectOverride;
            set
            {
                _subjectOverride = null;
                _propertyInfo = null;

                _subjectObject = value;
            }
        }

        public string PropertyName
        {
            get => _propertyName;
            set
            {
                _propertyInfo = null;

                _propertyName = value;
            }
        }

        PropertyInfo _propertyInfo = null;
        protected PropertyInfo PropertyInfo
        {
            get
            {
                _propertyInfo ??= _subjectOverride.GetType().GetProperty(_propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (_propertyInfo == null)
                {
                    Debug.LogError($"Subject doesnt have a property named {_propertyName}.");
                }
                return _propertyInfo;
            }
        }

        public object Value
        {
            get => PropertyInfo.GetValue(_subjectOverride);
            set => PropertyInfo.SetValue(_subjectOverride, value);
        }

        public bool IsEmpty() => _subjectOverride == null;

        public PropertyValueSynchronizer(object subject, string propertyName)
        {
            SubjectObject = subject;
            PropertyName = propertyName;
        }
    }

    /// <summary>
    /// 可配置的属性同步器的泛型版本。通过反射寻找指定 Unity.Object 中的某个属性，通过 <see cref="Value"/> 进行同步。
    /// 如果需要序列化，则应该自行通过继承消除泛型并添加 <see cref="System.SerializableAttribute"/> 特性。
    /// </summary>
    /// <typeparam name="T">同步的属性的类型</typeparam>
    public class PropertyValueSynchronizer<T> : PropertyValueSynchronizer
    {
        public PropertyValueSynchronizer(object subject, string propertyName) : base(subject, propertyName) { }

        public new T Value
        {
            get => (T)base.Value;
            set => base.Value = value;
        }
    }

    /// <summary>
    /// int 类型的 <see cref="PropertyValueSynchronizer{T}"/>
    /// </summary>
    [System.Serializable]
    public class IntPropertyValueSynchronizer : PropertyValueSynchronizer<int>
    {
        public IntPropertyValueSynchronizer(object subject, string propertyName) : base(subject, propertyName)
        {
        }
    }
}
