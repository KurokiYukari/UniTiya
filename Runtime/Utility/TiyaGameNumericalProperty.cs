using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.Utility
{
    [System.Serializable]
    public class TiyaGameDynamicNumericalProperty : IGameDynamicNumericalProperty
    {
        [SerializeField] bool _allowNegativeValue = false;
        [SerializeField, SetProperty(nameof(BaseValue))] float _baseValue;

        public float BaseValue { get => _baseValue; set => _baseValue = Mathf.Clamp(value, 0, float.MaxValue); }
        public float ExtraValue { get; set; }

        public float MaxValue => Mathf.Clamp(BaseValue + ExtraValue, 0, float.MaxValue);

        float _value;
        public float Value
        {
            get => _value;
            set
            {
                if (_allowNegativeValue)
                {
                    _value = Mathf.Clamp(value, float.MinValue, MaxValue);
                }
                else
                {
                    _value = Mathf.Clamp(value, 0, MaxValue);
                }
            }
        }
    }

    [System.Serializable]
    public class TiyaGameFixedNumericalProperty : IGameFixedNumericalProperty
    {
        [SerializeField] bool _allowNegativeValue = false;
        [SerializeField, SetProperty(nameof(BaseValue))] float _baseValue;

        public float BaseValue { get => _baseValue; set => _baseValue = Mathf.Clamp(value, 0, float.MaxValue); }
        public float ExtraValue { get; set; }

        public float Value => Mathf.Clamp(BaseValue + ExtraValue, _allowNegativeValue ? float.MinValue : 0, float.MaxValue);
    }
}
