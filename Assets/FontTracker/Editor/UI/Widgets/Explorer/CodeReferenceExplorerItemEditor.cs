using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Voodoo.Sauce.Font.Common;

namespace Voodoo.Sauce.Font
{
    public class CodeReferenceExplorerItemEditor : IExplorerItem<CodeReference>
    {
        public Rect Rect { get; private set; }

        public CodeReference Value { get; set; }

        public bool IsSelected { get; set; } = false;

        public void ContextClick()
        {
#if UNITY_2020_3_OR_NEWER
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Open in code editor"), false, Open);
            menu.ShowAsContext();
#endif
        }


        void Open()
        {
#if UNITY_2020_3_OR_NEWER
            string filePath = Path.Combine(Application.dataPath, Value.path.Replace("Assets" + Path.DirectorySeparatorChar, ""))
                                  .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            Unity.CodeEditor.CodeEditor.OSOpenFile(Unity.CodeEditor.CodeEditor.CurrentEditorPath, filePath);
#endif
        }

        public void OnGUI()
        {
            Rect = EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Value.path);
                
                var scriptIcon = EditorGUIUtility.IconContent("cs Script Icon");
                if (GUILayout.Button(scriptIcon, GUIStyle.none,  GUILayout.Width(20f), GUILayout.Height(20f)))
                {
                    Open();
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUI.indentLevel++;
                for (int i = 0; i < Value.referencedFont.Count; i++)
                {
                    EditorGUILayout.LabelField(Value.referencedFont[i]);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

        }
    }
}
