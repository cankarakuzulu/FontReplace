using System;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Sauce.Font
{
    [Serializable,
        CreateAssetMenu(fileName = "Assets/FontTracker/Resources/FT_Version", menuName = "VoodooSauce/FontTracker_Version", order = 1000)]
    public class FontTrackerVersion : ScriptableObject
    {
        public Version value = new Version(1, 0, 0, 0);

        public static FontTrackerVersion Load() => Resources.Load<FontTrackerVersion>("FT_Version");
    }

    [CustomEditor(typeof(FontTrackerVersion))]
    public class FontTrackerVersionEditor : UnityEditor.Editor
    {
        Version _version;

        void OnEnable()
        {
            _version = (target as FontTrackerVersion).value;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int major = EditorGUILayout.IntField(_version.Major, GUILayout.Width(20));
            EditorGUILayout.LabelField(".", GUILayout.Width(10));
            int minor = EditorGUILayout.IntField(_version.Minor, GUILayout.Width(20));
            EditorGUILayout.LabelField(".", GUILayout.Width(10));
            int build = EditorGUILayout.IntField(_version.Build, GUILayout.Width(20));
            EditorGUILayout.LabelField(".", GUILayout.Width(10));
            int revision = EditorGUILayout.IntField(_version.Revision, GUILayout.Width(20));

            if (EditorGUI.EndChangeCheck())
            {
                (target as FontTrackerVersion).value = new Version(major, minor, build, revision);
            }

            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }
    }
}

