using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Voodoo.Sauce.Font.Common;

namespace Voodoo.Sauce.Font
{
    public class FontReplacerEditor : IFontReplacerEditor
    {
        public string[] blacklistedFontPaths = new string[0];
        public string[] blacklistedFonts = new string[0];
        public string[] whitelistedFontPaths = new string[0];
        public string[] whitelistedFonts = new string[0];

        bool _isFoldout = true;

        int _blacklistedFontIndex = -1;
        int _whitelistedFontIndex = -1;
        int _allWhitelistedFontIndex = -1;

        bool _typeMatches = false;

        IExplorerEditor<FontRef> _fontRefSearchPanel;

        public FontReplacerEditor() 
        {
            _fontRefSearchPanel = new ExplorerEditor<FontRef>(new FontRefFilterEditor(), new MultiSelection(), null);
        }

        void UpdateItems()
        {
            var assetsInfo = FontFinder.FindAllFont();

            blacklistedFontPaths = assetsInfo.Where(font => font.isLegal == false).Select(font => font.path).ToArray();
            blacklistedFonts = blacklistedFontPaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();

            whitelistedFontPaths = assetsInfo.Where(font => font.isLegal).Select(font => font.path).ToArray();
            whitelistedFonts = whitelistedFontPaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();

            EditorWindow.focusedWindow.Repaint();
        }
        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            _isFoldout =  EditorGUILayout.Foldout(_isFoldout, "Font replacer");
            var refreshIcon = EditorGUIUtility.IconContent("Refresh");
            if (GUILayout.Button(refreshIcon, GUIStyle.none, GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                UpdateItems();
            }
            EditorGUILayout.EndHorizontal();

            if (_isFoldout)
            {
                //ShowReplaceAll();
                
                GUILayout.Space(20f);

                ShowPreciseReplace();
            }

            EditorGUILayout.EndVertical();
        }

        void ShowReplaceAll() 
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Replace all blacklisted Ref by"))
            {
                ReplaceAll();
            }
            _allWhitelistedFontIndex = EditorGUILayout.Popup(_allWhitelistedFontIndex, whitelistedFonts);
            EditorGUILayout.EndHorizontal();

        }

        void ShowPreciseReplace() 
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            _blacklistedFontIndex = EditorGUILayout.Popup(_blacklistedFontIndex, blacklistedFonts);
            EditorGUILayout.LabelField("->", GUILayout.Width(20f));
            _whitelistedFontIndex = EditorGUILayout.Popup(_whitelistedFontIndex, whitelistedFonts);
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck() && _blacklistedFontIndex >= 0 && _whitelistedFontIndex >= 0)
            {
                var blacklistedFont = AssetDatabase.LoadAssetAtPath<Object>(blacklistedFontPaths[_blacklistedFontIndex]);
                var whitelistedFont = AssetDatabase.LoadAssetAtPath<Object>(whitelistedFontPaths[_whitelistedFontIndex]);

                _typeMatches = blacklistedFont.GetType() == whitelistedFont.GetType(); 
            }

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find all references"))
            {
                UpdatedReferences();
            }

            EditorGUI.BeginDisabledGroup(_typeMatches == false);
            if (GUILayout.Button("Apply change for all"))
            {
                ReplaceReferences();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            if (_typeMatches == false)
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("You need to replace matching types, you can't replace FontAsset by TMP_FontAsset and the otherway around.");
                GUI.contentColor = Color.white;
            }

            _fontRefSearchPanel.OnGUI();
        }

        public void UpdatedReferences() 
        {
            var blacklistedFont = AssetDatabase.LoadAssetAtPath<Object>(blacklistedFontPaths[_blacklistedFontIndex]);
            var refs = AssetUtils.FindReferencesInFiles(blacklistedFont);

            var items = new List<IExplorerItem<FontRef>>();
            for (int i = 0; i < refs.Count; i++)
            {
                var value = new FontRef { reference = refs[i], font = blacklistedFont };
                var item = new FontRefExplorerItemEditor(value);
                items.Add(item);
            }

            _fontRefSearchPanel.Fill(items);
        }

        void ReplaceAll() 
        {
            var blacklisted = new Object[blacklistedFontPaths.Length];            

            for (int i = 0; i < blacklistedFontPaths.Length; i++)
            {
                blacklisted[i] = AssetDatabase.LoadAssetAtPath<Object>(blacklistedFontPaths[i]);
            }

            var whitelistedFont = AssetDatabase.LoadAssetAtPath<Object>(whitelistedFontPaths[_allWhitelistedFontIndex]);

            if ((blacklisted?.Length ?? 0) <= 0 || whitelistedFont == null)
            {
                return;
            }

            _ = AssetUtils.ReplaceAllReferences(blacklisted, whitelistedFont);
        }

        void ReplaceReferences() 
        {
            var blacklistedFont = AssetDatabase.LoadAssetAtPath<Object>(blacklistedFontPaths[_blacklistedFontIndex]);
            var whitelistedFont = AssetDatabase.LoadAssetAtPath<Object>(whitelistedFontPaths[_whitelistedFontIndex]);

            if (blacklistedFont == null ||whitelistedFont == null)
            {
                return;
            }

            _ = AssetUtils.ReplaceAllReferences(blacklistedFont, whitelistedFont);
        }
    }
}
