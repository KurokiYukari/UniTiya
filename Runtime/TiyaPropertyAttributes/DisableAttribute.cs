using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    /// <summary>
    /// 将一个 SerializedField （根据条件）设为 Disable
    /// </summary>
    public class DisableAttribute : PropertyAttribute
    {
        public string ConditionFieldName { get; }
        public bool DisableIfCondition { get; }

        public DisableAttribute() : this(null, false) { }

        public DisableAttribute(string conditionFieldName, bool disableIfCondition)
        {
            ConditionFieldName = conditionFieldName;
            DisableIfCondition = disableIfCondition;
        }
    }
}
