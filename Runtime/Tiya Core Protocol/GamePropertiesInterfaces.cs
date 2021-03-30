using System.Collections.Generic;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// 自定义属性器，在 UniTiya 中被用来作为定义游戏属性的工具。
    /// </summary>
    public interface IGameProperties : IEnumerable<(string propertyName, object propertyValue, bool isReadonly)>
    {
        /// <summary>
        /// 获取 / 设置已经存在的 property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">不存在名为 propertyName 的属性</exception>
        /// <exception cref="System.InvalidOperationException">尝试 set 一个 readOnly 的 property</exception>
        object this[string propertyName] { get; set; }

        /// <summary>
        /// 获取名为 propertyName 的 Property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">不存在名为 propertyName 的属性</exception>
        /// <exception cref="System.InvalidCastException">无法将属性转换成类型 T</exception>
        T GetProperty<T>(string propertyName);

        /// <summary>
        /// 获取名为 propertyName 的 Property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="isReadonly"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">不存在名为 propertyName 的属性</exception>
        /// <exception cref="System.InvalidCastException">无法将属性转换成类型 T</exception>
        T GetProperty<T>(string propertyName, out bool isReadonly);

        /// <summary>
        /// 添加一个原本不存在的 property。
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="isReadOnly"></param>
        /// <returns>存在指定 property 则返回 false，否则返回 true</returns>
        bool AddProperty(string propertyName, object value, bool isReadOnly = false);

        /// <summary>
        /// 强制设置一个 property，无论其是否存在，是否是 readOnly。
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="isReadOnly"></param>
        void SetProperty(string propertyName, object value, bool isReadOnly = false);

        /// <summary>
        /// 删除指定 property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>存在指定 property 则返回 true，否则返回 false</returns>
        bool RemoveProperty(string propertyName);

        /// <summary>
        /// 是否包含指定 property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool ContainsProperty(string propertyName);

        /// <summary>
        /// 指定 proeprty 是否是 readonly
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool IsReadonlyProperty(string propertyName);
    }

    /// <summary>
    /// 代表游戏的动态属性值接口，比如角色 HP 之类的。
    /// </summary>
    public interface IGameDynamicNumericalProperty
    {
        /// <summary>
        /// 基础属性值，一般作为角色的基础预先配置
        /// </summary>
        float BaseValue { get; set; }
        /// <summary>
        /// 额外属性值，一般作为技能、装备等的效果影响而动态配置
        /// </summary>
        float ExtraValue { get; set; }
        /// <summary>
        /// 最终属性值结果，一般直接为 BaseValue + ExtraValue
        /// </summary>
        float MaxValue { get; }
        /// <summary>
        /// 当前属性值
        /// </summary>
        float Value { get; set; }
    }

    /// <summary>
    /// 代表游戏的固定属性值接口，比如角色的力量值之类的。
    /// </summary>
    public interface IGameFixedNumericalProperty
    {
        /// <summary>
        /// 基础属性值，一般作为角色的基础预先配置
        /// </summary>
        float BaseValue { get; set; }
        /// <summary>
        /// 额外属性值，一般作为技能、装备等的效果影响而动态配置
        /// </summary>
        float ExtraValue { get; set; }
        /// <summary>
        /// 最终属性值结果，一般直接为 BaseValue + ExtraValue
        /// </summary>
        float Value { get; }
    }
}
