using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Sauce.Font
{
    public class ReferencesWindow : EditorWindow
    {
        static EditorWindow _window;

        List<Object> _references = new List<Object>();
        Vector2 _scrollview;

        [MenuItem("Assets/Voodoo/FindAllReferences")]
        static void FindAllRef()
        {
            if (_window != null)
            {
                _window.Close();
            }

            _window = GetWindow<ReferencesWindow>(false, "Voodoo Sauce font tracker");
            _window.Show();
        }

        public void OnEnable()
        {
            if (Selection.activeObject == null)
            {
                return;
            }

            _references = AssetUtils.FindReferencesInFiles(Selection.activeObject);
        }

        public void OnGUI()
        {
            _scrollview = EditorGUILayout.BeginScrollView(_scrollview);
            foreach (var item in _references)
            {
                EditorGUILayout.ObjectField(item, null);
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
