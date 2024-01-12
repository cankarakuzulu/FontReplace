using UnityEditor;
using UnityEngine;
using Voodoo.Sauce.Font.Common;

namespace Voodoo.Sauce.Font
{
    public class FontRefFilterEditor : IFilterEditor<FontRef>
    {
        string _filter;

        public bool IsValid(FontRef value) => string.IsNullOrEmpty(_filter) || value.reference.name.ToLower().IndexOf(_filter.ToLower()) >= 0;

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Search :");

            EditorGUILayout.BeginHorizontal();

            _filter = EditorGUILayout.TextField(_filter);

            var searchIcon = EditorGUIUtility.IconContent("Search Icon");
            GUILayout.Button(searchIcon, GUIStyle.none, GUILayout.Width(20f), GUILayout.Height(20f));

            EditorGUILayout.EndHorizontal();
        }
    }
}
