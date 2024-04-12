using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using TempID.VR.Skeleton.Gestures.Utilities;

namespace TempID.VR.Skeleton.Gestures
{
    /// <summary>
    /// Used to preview a gesture against a VRSkeleton.
    /// 
    /// This class provides a custom editor that handles the positioning
    /// of a defined VR skeleton within the Unity GUI and scene space.
    /// </summary>
    /// <seealso cref="StaticGesturePreviewEditor"/>
    public class StaticGesturePreview : MonoBehaviour {
        [Header("Previewing")]
        [SerializeField]
        public VRSkeletonUtility.PreviewMode previewMode;

        [Header("Editing")]

        [Tooltip("The left hand gesture bounding box")]
        public StaticGesture gesture;

        #region Instance Values

        [HideInInspector]
        [SerializeField]
        public GameObject skeletonPreview;

        [HideInInspector]
        public bool isLeftHandEditingEnabled = false;
        [HideInInspector]
        public List<Vector3> leftHandEditingRotationValues = new List<Vector3>();

        [HideInInspector]
        public bool isRightHandEditingEnabled = false;
        [HideInInspector]
        public List<Vector3> rightHandEditingRotationValues = new List<Vector3>();

        #endregion
    }

    /// <summary>
    /// Creates an in-Unity editor for static gestures with the following
    /// features:
    /// 1. Ability to add hand rotations to gestures directly
    /// 2. Ability to calculate an optimal gesture range
    /// 3. Ability to copy gestures between the left and right hands
    /// 4. Automatic visual updates for changes against a StaticGesture directly
    ///    within the game editor
    /// </summary>
    [CustomEditor(typeof(StaticGesturePreview)), CanEditMultipleObjects]
    public class StaticGesturePreviewEditor : Editor {
        #region Properties

        /// <summary>
        /// Lazily-instantiated object used when previewing the skeleton against
        /// the edited gesture in the editor.
        /// 
        /// When updated, 
        /// </summary>
        private GameObject serializedSkeletonObject {
            get {
                SerializedProperty skeletonPreviewProperty = serializedObject.FindProperty("skeletonPreview");
                if (skeletonPreviewProperty is null || skeletonPreviewProperty.objectReferenceValue is null) {
                    return null;
                }
                return skeletonPreviewProperty.objectReferenceValue as GameObject;
            } 
            set {
                skeletonPreview = value;
                serializedObject.FindProperty("skeletonPreview").objectReferenceValue = value;
            }
        }
        #endregion

        #region Private variables

        /// <summary>
        /// The preview skeleton object that is shown to the user to preview
        /// the gesture being edited
        /// </summary>
        private GameObject skeletonPreview;

        /// <summary>
        /// The left horizontal handle for angular editing
        /// </summary>
        private JointAngularLimitHandle leftHorizontalHandle = new JointAngularLimitHandle();

        /// <summary>
        /// The left vertical handle for angular editing
        /// </summary>
        private JointAngularLimitHandle leftVerticalHandle = new JointAngularLimitHandle();

        /// <summary>
        /// The right horizontal handle for angular editing
        /// </summary>
        private JointAngularLimitHandle rightHorizontalHandle = new JointAngularLimitHandle();

        /// <summary>
        /// The right vertical handle for angular editing
        /// </summary>
        private JointAngularLimitHandle rightVerticalHandle = new JointAngularLimitHandle();

        /// <summary>
        /// The editor drawer for the gesture for this object
        /// </summary>
        private Editor gestureEditor;

        #endregion

        /// <summary>
        /// Set up the preview on enable of preview
        /// </summary>
        private void OnEnable() {
            StaticGesturePreview gesturePreview = target as StaticGesturePreview;
            if (serializedSkeletonObject == null) {
                GesturePreviewSettings settings = Resources.Load<GesturePreviewSettings>("GestureSettings");
                GameObject previewPrefab = settings.editingPreviewSkeleton;
                serializedSkeletonObject = Instantiate(previewPrefab, gesturePreview.transform);
            } else {
                skeletonPreview = serializedSkeletonObject;
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Sets up the GUI for inspector editing.
        /// 
        /// Creates sections for editing the left and right hands of the 
        /// gesture.
        /// </summary>
        /// <details> 
        /// This is setup to create a GUI in the following flow:
        /// ##################################################################
        /// "Left hand rotation editing"
        /// [Button: Start rotation editing]
        /// 
        /// "Right hand rotation editing"
        /// [Button: Start rotation editing]
        /// 
        /// [Button: Copy left hand to right][Button: Copy right hand to left]
        /// [Button: Save Changes]
        /// ##################################################################
        /// 
        /// If rotation editing for a hand is enabled, the flow will be updated
        /// to the following (assume left hand has started editing):
        /// ##################################################################
        /// "Left hand rotation editing"
        /// [Button: "Add rotation][Button: "Cancel Editing"]
        /// [Button: "Stop rotation editing"]
        /// 
        /// "Right hand rotation editing"
        /// [Button: Start rotation editing]
        /// 
        /// [Button: Copy left hand to right][Button: Copy right hand to left]
        /// [Button: Save Changes]
        /// ##################################################################
        /// </details>
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            VRSkeleton skeleton = skeletonPreview.GetComponent<VRSkeleton>();
            StaticGesturePreview gesturePreviewer = target as StaticGesturePreview;
            
            StaticGesture gesture = gesturePreviewer.gesture;
            if (gesture is null) {
                return;
            }

            CreateCachedEditor(gesture, null, ref gestureEditor);
            gestureEditor.OnInspectorGUI();

            GUILayout.Space(10);
            GUILayout.Label("Left hand rotation editing");
            HandleHandRotationEditing(gesturePreviewer.gesture.leftHandGesture, 
                ref gesturePreviewer.isLeftHandEditingEnabled, 
                gesturePreviewer.leftHandEditingRotationValues, 
                skeleton.leftHand);
            GUILayout.Space(10);

            GUILayout.Label("Right hand rotation editing");
            HandleHandRotationEditing(gesturePreviewer.gesture.rightHandGesture, 
                ref gesturePreviewer.isRightHandEditingEnabled,
                gesturePreviewer.leftHandEditingRotationValues, 
                skeleton.rightHand);
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy left hand to right"))
            {
                gesturePreviewer.gesture.rightHandGesture = 
                    gesturePreviewer.gesture.leftHandGesture.GetGestureMirroredAcrossBodyAxis();
            }
            if (GUILayout.Button("Copy right hand to left"))
            {
                gesturePreviewer.gesture.leftHandGesture =
                    gesturePreviewer.gesture.rightHandGesture.GetGestureMirroredAcrossBodyAxis();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Save Changes"))
            {
                EditorUtility.SetDirty(gesture);
                serializedObject.ApplyModifiedProperties();
            }
        }

        // Handles one hand for editing rotations.
        /// <summary>
        /// Handles one hand for editing rotations
        /// </summary>
        /// <param name="editingGesture">The hand gesture to edit</param>
        /// <param name="isEditingEnabled">
        /// Whether editing for this hand was enabled on a previous frame
        /// </param>
        /// <param name="currentRotations">
        /// The current stored rotations of the hand object
        /// </param>
        /// <param name="previewHandTransform">
        /// The transform of the preview hand to get rotations from
        /// </param>
        /// <details>
        /// This will set up GUI buttons in the following flows:
        /// 
        /// if editing is disabled:
        /// ##################################################################
        /// [Start rotation editing]
        /// ##################################################################
        /// 
        /// if editing is enabled:
        /// ##################################################################
        /// [Add rotation][Cancel editing]
        /// [Stop rotation editing]
        /// ##################################################################
        /// </details>
        private void HandleHandRotationEditing(
            HandGesture editingGesture, 
            ref bool isEditingEnabled, 
            List<Vector3> currentRotations, 
            Transform previewHandTransform
        ) {
            GUILayout.BeginHorizontal();
            GUI.enabled = isEditingEnabled;
            if (GUILayout.Button("Add rotation")) {
                Vector3 wrappedHandRotValues = new Vector3(
                    FloatUtilities.WrapRotation(previewHandTransform.localEulerAngles.x), 
                    FloatUtilities.WrapRotation(previewHandTransform.localEulerAngles.y), 
                    FloatUtilities.WrapRotation(previewHandTransform.localEulerAngles.z));
                Debug.Log($"Gesture Editor: Added rotation {wrappedHandRotValues}");
                currentRotations.Add(wrappedHandRotValues);
            } 
            if (GUILayout.Button("Cancel editing")) {
                currentRotations.Clear();
                isEditingEnabled = false;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            string toggleHint = isEditingEnabled ? "Stop rotation editing" : "Start rotation editing";
            if (GUILayout.Button(toggleHint)) {
                isEditingEnabled = !isEditingEnabled;
                if (isEditingEnabled) {
                    // Just started; clear old rotations
                    currentRotations.Clear();
                } else {
                    // Just ended; populate new values
                    float[] xValues = currentRotations.Select(r => r.x).ToArray();
                    editingGesture.xRotBounds.min = Mathf.Min(xValues);
                    editingGesture.xRotBounds.max = Mathf.Max(xValues);

                    float[] yValues = currentRotations.Select(r => r.y).ToArray();
                    editingGesture.yRotBounds.min = Mathf.Min(yValues);
                    editingGesture.yRotBounds.max = Mathf.Max(yValues);

                    float[] zValues = currentRotations.Select(r => r.z).ToArray();
                    editingGesture.zRotBounds.min = Mathf.Min(zValues);
                    editingGesture.zRotBounds.max = Mathf.Max(zValues);
                    currentRotations.Clear();
                }
            }
        }

        /// <summary>
        /// Whenever the scene loads this script, update the preview skeleton
        /// such that is matches the preview settings of the input gesture.
        /// 
        /// Additionally, create joint angles to allow for easy edit the bounds 
        /// of the hands against a defined unit circle.
        /// </summary>
        protected void OnSceneGUI() {
            StaticGesturePreview gesturePreviewer = target as StaticGesturePreview;
            StaticGesture gesture = gesturePreviewer.gesture;
            VRSkeleton skeleton = skeletonPreview.GetComponent<VRSkeleton>();

            if (gesture is null) {
                return;
            }

            // Left hand/right hand previewing
            UpdateGestureUsingHandleAngle(
                gesture, skeleton.leftShoulder, gesture.leftHandGesture, leftHorizontalHandle, leftVerticalHandle);
            UpdateGestureUsingHandleAngle(
                gesture, skeleton.rightShoulder, gesture.rightHandGesture, rightHorizontalHandle, rightVerticalHandle);

            VRSkeletonUtility.PoseSkeletonForGesture(
                skeleton, gesture, gesturePreviewer.previewMode,
                updateLeftHandAngle: !gesturePreviewer.isLeftHandEditingEnabled,
                updateRightHandAngle: !gesturePreviewer.isRightHandEditingEnabled
            );
        }

        /// <summary>
        /// Updates the current settings of the gesture based on the provided
        /// joint angles.
        /// 
        /// Allows for easy click-and-drag editing of gesture angles.
        /// </summary>
        private void UpdateGestureUsingHandleAngle(
            StaticGesture editingGesture, 
            Transform previewLocation, 
            HandGesture handGesture, 
            JointAngularLimitHandle horizontalHandle, 
            JointAngularLimitHandle verticalHandle
        ) {
            verticalHandle.zMotion = ConfigurableJointMotion.Locked;
            verticalHandle.zHandleColor = Color.clear;
            verticalHandle.yMotion = ConfigurableJointMotion.Locked;
            verticalHandle.yHandleColor = Color.clear;

            verticalHandle.xRange = new Vector2(-90, 90);
            verticalHandle.xMin = handGesture.verticalBounds.min;
            verticalHandle.xMax = handGesture.verticalBounds.max;

            horizontalHandle.xMotion = ConfigurableJointMotion.Locked;
            horizontalHandle.xHandleColor = Color.clear;
            horizontalHandle.zMotion = ConfigurableJointMotion.Locked;
            horizontalHandle.zHandleColor = Color.clear;
            
            horizontalHandle.yMin = handGesture.horizontalBounds.min;
            horizontalHandle.yMax = handGesture.horizontalBounds.max;

            // set the handle matrix to match the object's position/rotation with a uniform scale
            Matrix4x4 handleMatrix = Matrix4x4.TRS(
                previewLocation.position,
                Quaternion.identity,
                Vector3.one
            );

            EditorGUI.BeginChangeCheck();

            using (new Handles.DrawingScope(handleMatrix)) {
                // maintain a constant screen-space size for the 
                // handle's radius based on the origin of the handle matrix
                horizontalHandle.radius = HandleUtility.GetHandleSize(Vector3.zero);
                verticalHandle.radius = HandleUtility.GetHandleSize(Vector3.zero);

                // draw the handle
                EditorGUI.BeginChangeCheck();
                horizontalHandle.DrawHandle();
                verticalHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck()) {
                    // record the target object before setting 
                    // new values so changes can be undone/redone
                    Undo.RecordObject(editingGesture, "Change Hand Gesture Properties");

                    handGesture.horizontalBounds.min = horizontalHandle.yMin;
                    handGesture.horizontalBounds.max = horizontalHandle.yMax;

                    handGesture.verticalBounds.min = verticalHandle.xMin;
                    handGesture.verticalBounds.max = verticalHandle.xMax;
                }
            }
        }
    }
}
