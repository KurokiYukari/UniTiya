using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaPropertyAttributes
{
    /// <summary>
    /// 修饰任何派生自 <see cref="UnityEngine.Object"/> SerializedField。
    /// 该 Attribute 保证该字段一定能通过 <see cref="TiyaTools.ConvertTo{T}(UnityEngine.Object)"/> 方法
    /// 转换成指定类型的对象。
    /// </summary>
    public class TypeRestrictionAttribute : PropertyAttribute
    {
        public Type RestrictedType { get; }

        public TypeRestrictionAttribute(Type restrictedType)
        {
            RestrictedType = restrictedType;
        }
    }
}
