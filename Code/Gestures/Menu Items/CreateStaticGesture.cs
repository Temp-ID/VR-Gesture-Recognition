using UnityEngine;
using UnityEditor;

namespace TempID.VR.Skeleton.Gestures.Menu {
    /// <summary>
    /// Utility class that enables the creation of new gestures from the Unity
    /// interface.
    /// </summary>
    public static class CreateStaticGesture {
        /// <summary>
        /// Generates a new StaticGesture in the current project window folder
        /// </summary>
        [MenuItem("Assets/Create/Gestures/StaticGesture")]
        public static void CreateEventGraph() {
            StaticGesture newGesture = ScriptableObject.CreateInstance<StaticGesture>();
            ProjectWindowUtil.CreateAsset(newGesture, "NewGesture.asset");
        }
    }
}
