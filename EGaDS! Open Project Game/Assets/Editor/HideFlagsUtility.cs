using UnityEngine;
using UnityEditor;

// Shows all GameObjects in scene, including those with hide flags
// from https://gist.github.com/FlaShG/38d860c836d9e09d606def9e6069024a
public static class HideFlagsUtility
{
    [MenuItem("GameObject/Show All Objects")]
    private static void ShowAll()
    {
        var allGameObjects = Object.FindObjectsOfType<GameObject>();
        foreach (var go in allGameObjects)
        {
            switch (go.hideFlags)
            {
                case HideFlags.HideAndDontSave:
                    go.hideFlags = HideFlags.DontSave;
                    break;
                case HideFlags.HideInHierarchy:
                case HideFlags.HideInInspector:
                    go.hideFlags = HideFlags.None;
                    break;
            }
        }
    }
}