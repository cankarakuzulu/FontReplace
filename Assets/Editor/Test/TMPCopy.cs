// // Copyright ©  2024 no-pact
// // Author: Can Karakuzulu

using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace @Editor.Test

{
    public static class TMPCopy
    {
        [MenuItem("no-pact/Component Copy/List Properties")]
        public static void ListComponentProperties()
        {
            StringBuilder sb = new StringBuilder();
            var selection = Selection.activeGameObject;
            var comp = selection.GetComponent<TextMeshProUGUI>();

            var ser = new SerializedObject(comp);

            var it = ser.GetIterator();

            do
            {
                sb.AppendLine($"{it.propertyPath}-{it.type}");

            } while (it.Next(true));

            var path = EditorUtility.SaveFilePanel("Save", "", "propertyData", "txt");
            File.WriteAllText(path, sb.ToString());
        }
    }
}