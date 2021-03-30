using UnityEngine;

namespace Sarachan.UniTiya.DamageInteract
{
    /// <summary>
    /// 适用于 <see cref="IActorController"/> 的 IDamageable 实现。
    /// 会根据 DamageSource 的 FinalDamageValue 减少 HP。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Damage Interaction/Tiya Basic Actor Damageable")]
    public class TiyaBasicActorDamageable : TiyaDamageable
    {
        IActorController _actor;
        protected IActorController Actor => _actor ??= GetComponent<IActorController>();

        protected override void ReceiveDamageOverride(IDamageSource damageSource)
        {
            var actorHPReference = Actor.GameProperties.ActorHP;

            actorHPReference.Value -= damageSource.FinalDamageValue;
        }
    }
}
