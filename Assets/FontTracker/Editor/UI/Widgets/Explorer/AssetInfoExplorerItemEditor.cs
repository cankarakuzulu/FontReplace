using System.IO;
using UnityEditor;
using UnityEngine;
using Voodoo.Sauce.Font.Common;

namespace Voodoo.Sauce.Font
{
    public class AssetInfoExplorerItemEditor : IExplorerItem<AssetInfo>
    {
        public Rect Rect { get; private set; }

        public AssetInfo Value { get; set; }

        public bool IsSelected { get; set; } = false;

        public void ContextClick()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, Delete);
            menu.AddItem(new GUIContent("Select"), false, Select);
            menu.ShowAsContext();
        }

        void Delete()
        {
            bool remove = EditorUtility.DisplayDialog($"Remove font", $" Do you wish to remove asset at path {Value.path}", "yes", "no");
            if (remove)
            {
                File.Delete(Value.path);
                File.Delete(Value.path + ".meta");
                AssetDatabase.Refresh();
            }
        }

        void Select()
        {
            EditorGUIUtility.PingObject(Value.Object);
            Selection.activeObject = Value.Object;
        }

        public void OnGUI()
        {
            GUI.backgroundColor = Value.isLegal ? Color.green : Color.red;
            
            Rect = EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUI.backgroundColor = Color.white;

                EditorGUILayout.LabelField(Value.path);

                var crossIcon = EditorGUIUtility.IconContent("CrossIcon");
                if (GUILayout.Button(crossIcon, GUIStyle.none, GUILayout.Width(20f), GUILayout.Height(20f)))
                {
                    Delete();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
