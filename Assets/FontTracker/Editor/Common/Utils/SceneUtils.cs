using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Voodoo.Sauce.Font
{
    public static class SceneUtils
    {
        public static SceneSetup[] _cachedScenes;
        
        public static void CacheState() =>_cachedScenes = EditorSceneManager.GetSceneManagerSetup();

        public static void RestoreScene() => EditorSceneManager.RestoreSceneManagerSetup(_cachedScenes);

        public static Scene[] GetAllOpennedScenes() 
        {
            int countLoaded = EditorSceneManager.sceneCount;
            var scenes = new Scene[countLoaded];

            for (int i = 0; i < countLoaded; i++)
            {
                scenes[i] = EditorSceneManager.GetSceneAt(i);
            }

            return scenes;
        }
    }
}
