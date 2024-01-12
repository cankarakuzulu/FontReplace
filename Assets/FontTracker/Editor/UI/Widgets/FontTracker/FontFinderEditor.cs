using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Voodoo.Sauce.Font.Common;

namespace Voodoo.Sauce.Font
{
    public class AssetInfoComparer : IComparer<AssetInfo>
    {
        public int Compare(AssetInfo x, AssetInfo y)
        {
            int value = x.isLegal.CompareTo(y.isLegal);
            return value == 0 ? x.path.CompareTo(y.path) : value;
        }
    }

    public class FontFinderEditor : IFontFinderEditor
    {
        IExplorerEditor<AssetInfo> _fontSearchPanel;
        IExplorerEditor<CodeReference> _codeRefSearchPanel;
        bool _displayFonts = true;
        bool _displayCodeRefs = true;

        public FontFinderEditor() 
        {
            MountExplorer();
        }

        void MountExplorer()
        {
            _fontSearchPanel = new ExplorerEditor<AssetInfo>(new AssetInfoFilterEditor(), new SingleSelection(), null);
            _codeRefSearchPanel = new ExplorerEditor<CodeReference>(new CodeReferenceFilterEditor(), new SingleSelection(), null);
        }

        void UpdateItems()
        {
            var assetsInfo = FontFinder.FindAllFont();
            assetsInfo.Sort(new AssetInfoComparer());

            var fontItems = new List<IExplorerItem<AssetInfo>>();
            for (int i = 0; i < assetsInfo.Count; i++)
            {
                var item = new AssetInfoExplorerItemEditor();
                item.Value = assetsInfo[i];
                fontItems.Add(item);
            }

            _fontSearchPanel.Fill(fontItems);

            var codeRefs = FontFinder.FindInCode(assetsInfo);
            var refItems = new List<IExplorerItem<CodeReference>>();
            for (int i = 0; i < codeRefs.Count; i++)
            {
                var item = new CodeReferenceExplorerItemEditor();
                item.Value = codeRefs[i];
                refItems.Add(item);
            }

            _codeRefSearchPanel.Fill(refItems);

            EditorWindow.focusedWindow.Repaint();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
           
            EditorGUILayout.LabelField("Font finder");

            var refreshIcon = EditorGUIUtility.IconContent("Refresh");
            if (GUILayout.Button(refreshIcon, GUIStyle.none, GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                UpdateItems();
            }

            EditorGUILayout.EndHorizontal();

            _displayFonts = EditorGUILayout.Foldout(_displayFonts, "Fonts in Assets/");

            if (_displayFonts)
            {
                _fontSearchPanel.OnGUI();
            }

            _displayCodeRefs = EditorGUILayout.Foldout(_displayCodeRefs, "Fonts reference in code");

            if (_displayCodeRefs)
            {
                _codeRefSearchPanel.OnGUI();
            }
            EditorGUILayout.EndVertical();
        }
    }
    
}
