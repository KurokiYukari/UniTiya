using System.Text;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Sarachan.UniTiya
{
    public static partial class TiyaEditorTools
    {
        /// <summary>
        /// 添加拓展 Editor
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="header">标题</param>
        /// <param name="itemNames">选项名称，如果是 null 则直接使用对应的 extension 类型的名称</param>
        /// <param name="extensions">可选的拓展们</param>
        /// <param name="OnAddExtension">添加完成后的回调方法，一般用来执行拓展的参数设置等工作</param>
        public static void AddExtensionField(this Editor editor, string header, string[] itemNames, System.Type[] extensions, System.Action<Component> OnAddExtension = null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Add Extension");

                if (EditorGUILayout.DropdownButton(new GUIContent("(select...)"), FocusType.Keyboard))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < extensions.Length; i++)
                    {
                        var extension = extensions[i];
                        var name = itemNames?[i] ?? extension.Name;

                        menu.AddItem(new GUIContent(name),
                            false,
                            () => {
                                var component = (editor.target as MonoBehaviour).gameObject.AddComponent(extension);
                                OnAddExtension?.Invoke(component);
                                EditorUtility.SetDirty(editor.target);
                            });
                    }

                    menu.ShowAsContext();
                }
            }
        }

        /// <summary>
        /// 在与 <paramref name="property"/> 同级寻找名为 <paramref name="fieldName"/> 的 SerializedProperty
        /// </summary>
        /// <param name="property"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static SerializedProperty FindSiblingSerializedProperty(this SerializedProperty property, string fieldName)
        {
            var path = property.propertyPath;
            var elements = path.Split('.');

            StringBuilder fullPath = new StringBuilder();
            for (int i = 0; i < elements.Length - 1; i++)
            {
                fullPath.Append($"{elements[i]}.");
            }
            fullPath.Append(fieldName);

            return property.serializedObject.FindProperty(fullPath.ToString());
        }

        public static object GetTargetObject(this SerializedProperty property)
        {
            if (property == null) return null;

            var path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        public static void SetTargetObjectOfProperty(this SerializedProperty prop, object value)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            if (obj is null) return;

            try
            {
                var element = elements.Last();

                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    if (GetValue_Imp(obj, elementName) is System.Collections.IList arr)
                    {
                        arr[index] = value;
                    }
                }
                else
                {
                    var tp = obj.GetType();
                    var field = tp.GetField(element, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                    }
                }

            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// 通过反射获取 <paramref name="source"/> 中 <paramref name="name"/> 字段的值。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// 适用与 <paramref name="source"/> 为 <see cref="System.Collections.IEnumerable"/> 的情况。
        /// 通过反射获取 <paramref name="source"/> 中 <paramref name="name"/> 字段的第 <paramref name="index"/> 个值。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object GetValue_Imp(object source, string name, int index)
        {
            if (!(GetValue_Imp(source, name) is System.Collections.IEnumerable enumerable)) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
}
