using UnityEditor;

namespace Sarachan.UniTiya.Utility
{
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.StringPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.IntPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.FloatPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.ObjectPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.TiyaGameDynamicPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.TiyaGameFixedPropertyDictionary))]
    [CustomPropertyDrawer(typeof(Skill.Projectile_CollisionEffectExtension.PhysicMaterial_RandomObjEmitters_Dictionary))]
    class TiyaSerializedDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }

    [CustomPropertyDrawer(typeof(Skill.Projectile_CollisionEffectExtension.PhysicMaterial_RandomObjEmitters_Dictionary.Storage))]
    class TiyaSerializedDictionaryStoragePropertyDrawer : SerializableDictionaryStoragePropertyDrawer { }
}
