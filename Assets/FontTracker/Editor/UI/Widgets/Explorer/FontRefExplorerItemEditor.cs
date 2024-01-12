using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Voodoo.Sauce.Font.Common;

namespace Voodoo.Sauce.Font
{
    public class FontRefExplorerItemEditor : IExplorerItem<FontRef>
    {
        public Rect Rect { get; private set; }

        public FontRef Value { get; set; }

        public bool IsSelected { get; set; } = false;

        bool _isFoldout = false;
        bool _wasSceneOpenned = false;
        Scene _scene;
        List<Object> _subRefs = new List<Object>();

        public FontRefExplorerItemEditor(FontRef fontRef) 
        {
            Value = fontRef;

            if (Value.reference is SceneAsset sceneAsset)
            {
                _wasSceneOpenned = System.Array.Exists(SceneUtils.GetAllOpennedScenes(), (scene) => scene.name == Value.reference.name);
                if (_wasSceneOpenned)
                {
                    _scene = System.Array.Find(SceneUtils.GetAllOpennedScenes(), (scene) => scene.name == Value.reference.name);
                }
            }
        }

        public void ContextClick()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Select"), false, Select);
            menu.ShowAsContext();
        }


        void Select()
        {
            EditorGUIUtility.PingObject(Value.reference);
            Selection.activeObject = Value.reference;
        }

        public void OnGUI()
        {
            Rect = EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                DisplayItem((dynamic)Value.reference);
            }
            EditorGUILayout.EndVertical();
        }

        void DisplayItem(Object value) 
        {
            EditorGUILayout.LabelField(Value.reference.name);
        }

        void DisplayItem(SceneAsset value)
        {
            EditorGUI.BeginChangeCheck();
            _isFoldout = EditorGUILayout.Foldout(_isFoldout, Value.reference.name);
            if (EditorGUI.EndChangeCheck())
            {
                OnFoldoutChange();
            }

            if (_isFoldout)
            {
                DisplaySubRefs();
            }
        }

        void OnFoldoutChange() 
        {
            if (_wasSceneOpenned)
            {
                if (_isFoldout)
                {
                    FindRefsInScene(_scene, OpenSceneMode.Additive);
                }

                return;
            }

            if (_isFoldout)
            {
                string scenepath = AssetDatabase.GetAssetPath(Value.reference);
                EditorSceneManager.sceneOpened += FindRefsInScene;
                _scene = EditorSceneManager.OpenScene(scenepath, OpenSceneMode.Additive);
            }
            else
            {
                EditorSceneManager.CloseScene(_scene, true);
            }
        }

        void FindRefsInScene(Scene scene, OpenSceneMode mode)
        {
            _subRefs = AssetUtils.FindReferencesInScene(scene, Value.font);
        }

        void DisplaySubRefs() 
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < _subRefs.Count; i++)
            {
                EditorGUILayout.ObjectField(_subRefs[i], null);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}
