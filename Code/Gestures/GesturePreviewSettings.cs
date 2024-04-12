using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempID.VR.Skeleton.Gestures
{
    /// <summary>
    /// Saveable settings that define how a gesture should be previewed.
    /// This can be used to define the base setup to use when defining an object
    /// </summary>
    [System.Serializable]
    public class GesturePreviewSettings : ScriptableObject {
        [Header("General")]

        [Tooltip("The skeleton that should be used to preview a gesture during editing")]
        public GameObject editingPreviewSkeleton;

        [Header("Timed Gesture Preview Settings")]

        [Tooltip("The default material to display")]
        public Material defaultMaterial;

        [Tooltip("The material to show when a pose is recognizable")]
        public Material poseRecognizableMaterial;
    }
}