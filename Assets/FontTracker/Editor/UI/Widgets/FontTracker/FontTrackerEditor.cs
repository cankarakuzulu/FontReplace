using UnityEditor;
using UnityEngine;

namespace Voodoo.Sauce.Font
{
    public class FontTrackerEditor : IFontTrackerEditor
    {
        public IFontFinderEditor Finder { get; private set; }

        public IFontReplacerEditor Replacer { get; private set; }

        public FontTrackerEditor() 
        {
            Finder = new FontFinderEditor();
            Replacer = new FontReplacerEditor();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            Finder.OnGUI();
            GUILayout.Space(20f);
            Replacer.OnGUI();
            EditorGUILayout.EndVertical();
        }
    }
}
