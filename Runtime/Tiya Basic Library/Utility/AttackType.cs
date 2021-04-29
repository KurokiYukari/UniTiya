using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// <see cref="IWeaponActorActions"/> 行为的类型。可以通过其中的 static 方法获取对应类型的枚举。
    /// FIXME: 拥有 [SerializedField] 的该类型是不可修改的。
    /// </summary>
    [System.Serializable]
    public sealed class AttackType : ISerializationCallbackReceiver
    {
        enum TypeEnum
        {
            NormalAttack = 1, SpecialAttack = 1 << 1, ExtraSkills = 1 << 2
        }

        [SerializeField] TypeEnum _type = TypeEnum.NormalAttack;

#if UNITY_EDITOR
        [HideInInspector] [SerializeField] bool _hideExtraSkillIndex;

        [HideIf(nameof(_hideExtraSkillIndex), true)]
#endif
        [SerializeField] int _extraSkillIndex;

        public int Value { get; private set; }

        AttackType(int value)
        {
            Value = value;
        }

        public int ToExtraSkillIndex()
        {
            return Value >= 0 ? Value : -1;
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            _hideExtraSkillIndex = _type != TypeEnum.ExtraSkills;
#endif

            if (Value == NormalAttack)
            {
                _type = TypeEnum.NormalAttack;
            }
            else if (Value == SpecialAttack)
            {
                _type = TypeEnum.SpecialAttack;
            }
            else
            {
                _type = TypeEnum.ExtraSkills;
                _extraSkillIndex = ToExtraSkillIndex();
            }
        }

        public void OnAfterDeserialize()
        {
            Value = _type switch
            {
                TypeEnum.NormalAttack => NormalAttack,
                TypeEnum.SpecialAttack => SpecialAttack,
                TypeEnum.ExtraSkills => GetExtraSkillAttackType(_extraSkillIndex),
                _ => throw new System.InvalidOperationException()
            };
        }

        public override bool Equals(object obj)
        {
            return obj is AttackType type &&
                   Value == type.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }

        public static implicit operator int(AttackType attackType)
        {
            return attackType.Value;
        }

        public static bool operator ==(AttackType left, AttackType right)
        {
            return EqualityComparer<AttackType>.Default.Equals(left, right);
        }

        public static bool operator !=(AttackType left, AttackType right)
        {
            return !(left == right);
        }

        public static AttackType NormalAttack => new AttackType(-1);
        public static AttackType SpecialAttack => new AttackType(-2);

        public static AttackType GetExtraSkillAttackType(int extraSkillIndex)
        {
            return extraSkillIndex >= 0 ? new AttackType(extraSkillIndex) :
                throw new System.IndexOutOfRangeException(nameof(extraSkillIndex));
        }
    }
}
