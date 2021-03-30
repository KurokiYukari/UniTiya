using UnityEngine;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    /// <summary>
    /// 将一个 SerializedField 根据条件隐藏
    /// </summary>
    public class HideIfAttribute : PropertyAttribute
    {
        public string HideConditionFieldName { get; }
        public bool HideIfCondition { get; }

        public HideIfAttribute(string hideConditionFieldName , bool hideIfCondition)
        {
            HideConditionFieldName = hideConditionFieldName;
            HideIfCondition = hideIfCondition;
        }
    }
}
