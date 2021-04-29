using UnityEngine;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    /// <summary>
    /// 只在 Editor 非 Playing 时生效，如果处于 Playing 状态则会将之 Disable
    /// </summary>
    public class EditorOnlyAttribute : PropertyAttribute
    {
        public EditorOnlyAttribute() { }
    }
}
