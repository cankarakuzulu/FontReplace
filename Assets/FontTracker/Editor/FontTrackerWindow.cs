using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Sauce.Font
{ 
    public class FontTrackerWindow : EditorWindow
    {
        FontTrackerVersion _version;
        IFontTrackerEditor _editor;
        static EditorWindow _window;

        [MenuItem("VoodooSauce/FontTracker #f", false, -50)]
        static void Init()
        {
            if (_window != null)
            {
                _window.Close();
            }

            _window = GetWindow<FontTrackerWindow>(false, "Voodoo Sauce font tracker");
            _window.minSize = new Vector2(300f, 500f);
            _window.Show();
        }

        public void OnEnable()
        {
            _ = InitFinder();

            _version = FontTrackerVersion.Load();
            _editor = new FontTrackerEditor();
        }

        async Task InitFinder() 
        {
            string[] fonts = await FontListImporter.ImportGoogleSheetAsync();
            FontFinder.Init(fonts);
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField($"Version : {_version.value}");
            _editor?.OnGUI();
        }
    }
}
