using UnityEditor;

namespace Sarachan.UniTiya.Utility
{
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.StringPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.IntPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.FloatPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.ObjectPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.TiyaGameDynamicPropertyDictionary))]
    [CustomPropertyDrawer(typeof(GamePropertyConfiguration.TiyaGameFixedPropertyDictionary))]
    class TiyaSerializedDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
}
