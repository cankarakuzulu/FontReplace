using UnityEditor;
using UnityEngine;
using Voodoo.Sauce.Font.Common;

namespace Voodoo.Sauce.Font
{
    public class CodeReferenceFilterEditor : IFilterEditor<CodeReference>
    {
        string _filter;

        public bool IsValid(CodeReference value) => string.IsNullOrEmpty(_filter) || value.path.ToLower().IndexOf(_filter.ToLower()) >= 0;

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