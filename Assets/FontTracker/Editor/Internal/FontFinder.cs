using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Voodoo.Sauce.Font
{
    public static class FontFinder
    {
        public static List<string> fontExtensions = new List<string>
        {
            ".otf",
            ".ttf",
            ".woff2",
            ".woff",
            ".pfb",
            ".pfm",
            ".afm",
            ".inf",
            ".pfa",
            ".ofm",
            ".pcf",
            ".bdf",
            ".snf",
            ".dwf",
            ".bf",
            ".fon",
            ".bmf",
            ".psf",
            ".pk",
            ".fzx"
        };

        public static string[] WhitelistedFonts;

        public static void Init(string[] whitelistedFonts) => WhitelistedFonts = whitelistedFonts;

        public static List<AssetInfo> FindAllFont()
        {
            EditorUtility.DisplayProgressBar("Searching for fonts", "With unity filter...", 0f);
            var result = FindWithUnityFilter();
            EditorUtility.DisplayProgressBar("Searching for fonts", "With custom filter...", 0.5f);
            var customFonts = FindByExtensions();

            int count = customFonts.Count;
            for (int i = 0; i < count; i++)
            {
                if (result.Contains(customFonts[i]))
                {
                    continue;
                }

                result.Add(customFonts[i]);
            }

            EditorUtility.ClearProgressBar();

            return result;
        }

        static List<AssetInfo> FindWithUnityFilter()
        {
            var result = new List<AssetInfo>();
            var guids = AssetDatabase.FindAssets("t:Font", new string[] { "Assets" });

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                string references = AssetDatabase.GUIDToAssetPath(guids[i]);
                string name = Path.GetFileNameWithoutExtension(path);
                var info = new AssetInfo
                {
                    guid = guids[i],
                    path = path,
                    isLegal = IsBlacklisted(name) == false
                };

                result.Add(info);
            }

            return result;
        }

        static List<AssetInfo> FindByExtensions()
        {
            var result = new List<AssetInfo>();
            var paths = AssetUtils.FindAssetsWhere(IsFont);

            for (int i = 0; i < paths.Count; i++)
            {
                string guid = AssetDatabase.AssetPathToGUID(paths[i]);
                string name = Path.GetFileNameWithoutExtension(paths[i]);
                var info = new AssetInfo
                {
                    guid = guid,
                    path = paths[i],
                    isLegal = IsBlacklisted(name) == false
                };

                result.Add(info);
            }

            return result;
        }

        static bool IsFont(string path) =>IsRegularFont(path) || IsTMPFont(path);

        static bool IsRegularFont(string path) => fontExtensions?.Contains(Path.GetExtension(path).ToLower()) ?? false;

        static bool IsTMPFont(string path) => Path.GetExtension(path).ToLower() == ".asset" &&
                         AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(TMPro.TMP_FontAsset);

        //Improve by computing string distance and accepting a certain threshold, also comparing Hash
        public static bool IsBlacklisted(string fontName)
        {
            string name = Path.GetFileNameWithoutExtension(fontName.ToLower());
            name = Regex.Replace(name, @"\s+", "");

            for (int i = 0; i < WhitelistedFonts.Length; i++)
            {
                string whitelisted = Regex.Replace(WhitelistedFonts[i].ToLower(), @"\s+", "");
                if (name.ToLower().IndexOf(whitelisted) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static List<CodeReference> FindInCode(List<AssetInfo> assetInfos)
        {
            EditorUtility.DisplayProgressBar("Searching for fonts", "References in code...", 0f);

            var result = new List<CodeReference>();

            string[] filePaths = Directory.GetFiles(UnityEngine.Application.dataPath, "*.cs", SearchOption.AllDirectories);

            var blacklistedFonts = assetInfos.Where(f => f.isLegal == false).Select(f => Path.GetFileNameWithoutExtension(f.path)).ToArray();

            for (int i = 0; i < filePaths.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Searching for fonts", "References in code...", (float)i/filePaths.Length);

                if (filePaths[i].IndexOf("FontTracker") >= 0)
                { 
                    continue; 
                }

                string fileText = File.ReadAllText(filePaths[i]);

                var codeRef = new CodeReference
                {
                    path = filePaths[i],
                    referencedFont = new List<string>()
                };

                for (int j = 0; j < blacklistedFonts.Length; j++)
                {
                    if (fileText.IndexOf(blacklistedFonts[j]) >= 0)
                    {
                        codeRef.referencedFont.Add(blacklistedFonts[j]);
                    }
                }

                if (codeRef.referencedFont.Count >= 1)
                {
                    result.Add(codeRef);
                }
            }

            EditorUtility.ClearProgressBar();

            return result;
        }
    }
}
