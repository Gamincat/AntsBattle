using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GamincatKit.Tools
{
    public static class ScriptableObjectDirtyFlagTools
    {
        [MenuItem("Assets/GamincatKit/SetDirty and SaveAsset")]
        private static void ShowSelectionObjects()
        {
            var assets = Selection.GetFiltered(typeof(ScriptableObject), SelectionMode.Assets);
            foreach (var asset in assets) EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/GamincatKit/SetDirty and SaveAsset", true)]
        private static bool ValidateSelectionObjects()
        {
            return Selection.GetFiltered(typeof(ScriptableObject), SelectionMode.Assets).Any();
        }
    }
}