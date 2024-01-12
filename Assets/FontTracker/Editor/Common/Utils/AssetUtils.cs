using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Voodoo.Sauce.Font
{
    public class ObjectTypeComparer : IComparer<Object>
    {
        public int Compare(Object x, Object y) =>x.GetType().Name.CompareTo(y.GetType().Name);
    }

    public static class AssetUtils
    {
        static string _replacementReport;

        public static List<string> FindAssetsWhere(System.Predicate<string> predicate)
        {
            var result = new List<string>();
            var paths = AssetDatabase.GetAllAssetPaths();

            for (int i = 0; i < paths.Length; i++)
            {
                if (predicate(paths[i]))
                {
                    result.Add(paths[i]);
                }
            }

            return result;
        }

        public static List<Object> FindReferencesInFiles(Object toFind) 
        {
			EditorUtility.DisplayProgressBar("Searching", "Generating file paths", 0.0f);

			var paths = new List<string>(AssetDatabase.GetAllAssetPaths());
			
			string toFindName = AssetDatabase.GetAssetPath(toFind);
			toFindName = Path.GetFileNameWithoutExtension(toFindName);
			Object tmpArray;
			List<Object> references = new List<Object>();

			// Loop through all files, and add any that have the selected object in it's list of dependencies
			int numPaths = paths.Count;
			for (int i = 0; i < numPaths; ++i)
			{
				tmpArray = AssetDatabase.LoadMainAssetAtPath(paths[i]);
				if (tmpArray != null && tmpArray != toFind)
				{
					Object[] dependencies = AssetDatabase.GetDependencies(paths[i], false).Select(p => AssetDatabase.LoadMainAssetAtPath(p)).ToArray();
					if (System.Array.Exists(dependencies, item => item == toFind))
					{
						references.Add(tmpArray);
					}
				}

				EditorUtility.DisplayProgressBar("Searching", "Searching dependencies", (float)i / numPaths);
			}

			EditorUtility.ClearProgressBar();

            references.Sort(new ObjectTypeComparer());

			return references;
		}

        public static List<Object> FindReferencesInScenes(Object toFind)
        {
            var referencedBy = new List<Object>();
            var scenes = SceneUtils.GetAllOpennedScenes();

            for (int i = 0; i < scenes.Length; i++)
            {
                referencedBy.AddRange(FindReferencesInScene(scenes[i], toFind));
            }

            return referencedBy;
        }

        public static List<Object> FindReferencesInScene(Scene scene, Object toFind)
        {
            var referencedBy = new List<Object>();
            var allObjects = scene.GetRootGameObjects();
            for (int j = 0; j < allObjects.Length; j++)
            {
                var go = allObjects[j];

                if (PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.Connected &&
                    PrefabUtility.GetCorrespondingObjectFromSource(go) == toFind)
                {
                    continue;
                }

                var components = go.GetComponentsInChildren<Component>(true);
                for (int i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    if (component == null)
                    {
                        continue;
                    }

                    var so = new SerializedObject(component);
                    var sp = so.GetIterator();

                    while (sp.NextVisible(true))
                    {
                        if (sp.propertyType == SerializedPropertyType.ObjectReference &&
                            sp.objectReferenceValue == toFind)
                        {
                            referencedBy.Add(component.gameObject);
                        }
                    }
                }
            }

            return referencedBy;
        }

        public async static Task ReplaceAllReferences(Object[] toFind, Object replacement)
        {
            for (int i = 0; i < toFind.Length; i++)
            {
                await ReplaceAllReferences(toFind[i], replacement);
            }
        }

        public async static Task ReplaceAllReferences(Object toFind, Object replacement)
        {
            _replacementReport = $"Replacing reference of {toFind.name} by {replacement.name}";

            var sources = FindReferencesInFiles(toFind);

            for (int i = 0; i < sources.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Replacing", $"Replacing references in {sources[i].name}", (float)i / sources.Count);
                await ReplaceReferences((dynamic)sources[i], toFind, replacement);

                Saves((dynamic)sources[i]);
            }

            EditorUtility.ClearProgressBar();

            AssetDatabase.SaveAssets();

            Debug.Log(_replacementReport);
        }

        private static void Saves(Object source) {}

        private static void Saves(GameObject source)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(source))
            {
                PrefabUtility.SavePrefabAsset(source);
            }

            if (PrefabUtility.IsPartOfPrefabInstance(source))
            {
                PrefabUtility.ApplyPrefabInstance(source, InteractionMode.UserAction);
            }
        }

        public async static Task ReplaceReferences(SceneAsset source, Object toFind, Object replacement)
        {
            _replacementReport += $"\n  References for {source.name}";

            bool isOpenned = EditorSceneManager.GetActiveScene().name == source.name;
            string scenepath = AssetDatabase.GetAssetPath(source);
            EditorSceneManager.sceneOpened += (_, __) => { isOpenned = true; };
            Scene scene = EditorSceneManager.OpenScene(scenepath, OpenSceneMode.Single);

            while (isOpenned == false) 
            {
                await Task.Yield();
            }

            var allObjects = Object.FindObjectsOfType<GameObject>();
            for (int i = 0; i < allObjects.Length; i++)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(source) || PrefabUtility.IsPartOfPrefabInstance(source))
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("Replacing in scene", $"Replacing references in scene {source.name}", (float)i / allObjects.Length);
                await ReplaceReferences(allObjects[i], toFind, replacement);
            }

            EditorUtility.ClearProgressBar();

            bool isSaved = false;

            EditorSceneManager.sceneSaved += (_) => { isSaved = true; } ;
            EditorSceneManager.SaveScene(scene);

            while (isSaved == false)
            {
                await Task.Yield();
            }
        }

        public async static Task ReplaceReferences(Object source, Object toFind, Object replacement) 
        {
            var go = source as GameObject;
            if (go == null)
            {
                return;
            }

            var components = go.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (component == null)
                {
                    continue;
                }

                var so = new SerializedObject(component);
                var sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference &&
                        sp.objectReferenceValue == toFind)
                    {
                        _replacementReport += $"\n    -Replaced reference inside of {component.gameObject.name}";
                        sp.objectReferenceValue = replacement;
                    }
                }
                
                so.ApplyModifiedProperties();
            }
                        
            await Task.Yield();
        }
    }
}
