using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempID.VR.Skeleton.Gestures.Utilities {
    /// <summary>
    /// Static utilities class for VRSkeleton
    /// </summary>
    public static partial class VRSkeletonUtility {
        /// <summary>
        /// Specifies the mode to use when previewing a given skeleton gesture
        /// </summary>
        public enum PreviewMode {
            Minimum,
            Average,
            Maximum
        }

        /// <summary>
        /// Poses the input skeleton to mimic the input pose.
        /// </summary>
        /// <param name="skeleton">The skeleton to pose</param>
        /// <param name="gesture">The gesture to mimic</param>
        /// <param name="previewMode">
        /// The method of parsing the pose's bounds
        /// </param>
        public static void PoseSkeletonForGesture(
            VRSkeleton skeleton,
            StaticGesture gesture,
            PreviewMode previewMode,
            bool updateLeftHandAngle = true,
            bool updateRightHandAngle = true
        ) {
            skeleton.leftHand.position = GetHandPreviewPosition(previewMode, gesture.leftHandGesture,
                skeleton.leftShoulder, skeleton.transform.lossyScale);
            if (updateLeftHandAngle) {
                skeleton.leftHand.localEulerAngles = GetHandPreviewRotation(previewMode, gesture.leftHandGesture);
            }

            skeleton.rightHand.position = GetHandPreviewPosition(previewMode, gesture.rightHandGesture,
                skeleton.rightShoulder, skeleton.transform.lossyScale);
            if (updateRightHandAngle) {
                skeleton.rightHand.localEulerAngles = GetHandPreviewRotation(previewMode, gesture.rightHandGesture);
            }
        }

        /// <summary>
        /// Retrieves where the hand should be located in world space for the
        /// given HandGesture relative to the input shoulder transform.
        /// input shoulder
        /// </summary>
        /// <param name="previewMode">
        /// The methodology of hand preview calculation
        /// </param>
        /// <param name="handGesture">
        /// The gesture to get the position of the hand for
        /// </param>
        /// <param name="shoulderTransform">
        /// The shoulder used to find the position of the hand
        /// </param>
        /// <param name="skeletonScale">
        /// The scale of the skeleton. Used to determine how far to position
        /// the hand.
        /// </param>
        /// <returns>Where the hand should be positioned</returns>
        public static Vector3 GetHandPreviewPosition(
            PreviewMode previewMode,
            HandGesture handGesture,
            Transform shoulderTransform,
            Vector3 skeletonScale
        ) {
            // If matching "outside" angles during average previews,
            // then add 180 to get the area on the opposite side of the unit
            // circle 180 degree rotation is the "opposite" rotation.
            // Same as multiplying dir by -1
            float horizontalAngle = previewMode switch {
                PreviewMode.Average => (
                    (handGesture.horizontalBounds.min + handGesture.horizontalBounds.max) / 2f)
                    + (Equals(handGesture.horizontalDirectionMode, RecognitionMode.Outside) ? 180 : 0),
                PreviewMode.Maximum => handGesture.horizontalBounds.max,
                PreviewMode.Minimum => handGesture.horizontalBounds.min,
                _ => throw new System.Exception("You didn't implement a case bozo")
            };
            float verticalAngle = previewMode switch {
                PreviewMode.Average => (
                    (handGesture.verticalBounds.min + handGesture.verticalBounds.max) / 2f)
                    + (Equals(handGesture.verticalDirectionMode, RecognitionMode.Outside) ? 180 : 0),
                PreviewMode.Maximum => handGesture.verticalBounds.max,
                PreviewMode.Minimum => handGesture.verticalBounds.min,
                _ => throw new System.Exception("You didn't implement a case bozo")
            };
            Vector3 previewDir = Vector3.Scale(Quaternion.Euler(-1 * verticalAngle, horizontalAngle, 0)
                * shoulderTransform.forward, skeletonScale);

            return shoulderTransform.position + previewDir * 0.3f;
        }

        /// <summary>
        /// Gets the rotation the hand should be previewed with in Euler angles
        /// </summary>
        /// <param name="previewMode">The method of angle calculation</param>
        /// <param name="handGesture">
        /// The gesture to find the rotation for
        /// </param>
        /// <returns>The preview angle of the hand in euler angles</returns>
        public static Vector3 GetHandPreviewRotation(PreviewMode previewMode, HandGesture handGesture) {
            return previewMode switch {
                PreviewMode.Average => new Vector3(
                        // Add 180 if the selection mode if "outside".
                        // This turns "inside angles" to "outside angles"
                        (handGesture.xRotBounds.min + handGesture.xRotBounds.max) / 2f
                            + (handGesture.xRotRecognitionMode == RecognitionMode.Outside ? 180 : 0),
                        (handGesture.yRotBounds.min + handGesture.yRotBounds.max) / 2f
                            + (handGesture.yRotRecognitionMode == RecognitionMode.Outside ? 180 : 0),
                        (handGesture.zRotBounds.min + handGesture.zRotBounds.max) / 2f
                            + (handGesture.zRotRecognitionMode == RecognitionMode.Outside ? 180 : 0)
                    ),
                PreviewMode.Maximum => new Vector3(
                    handGesture.xRotBounds.max, handGesture.yRotBounds.max, handGesture.zRotBounds.max),
                PreviewMode.Minimum => new Vector3(
                    handGesture.xRotBounds.min, handGesture.yRotBounds.min, handGesture.zRotBounds.min),
                _ => throw new System.Exception("Received unknown preview mode ${previewMode}"),
            };
        }
    }
}
