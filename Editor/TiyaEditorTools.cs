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
    }
}
