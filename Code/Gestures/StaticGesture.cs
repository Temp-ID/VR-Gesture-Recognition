using UnityEngine;

using TempID.VR.Skeleton;
using TempID.VR.Skeleton.Gestures.Utilities;

namespace TempID.VR.Skeleton.Gestures {
    // <summary>
    // A StaticGesture is a method of capturing the pose being performed
    // by a VRSkeleton.
    // 
    // A StaticGesture is considered fulfilled if both the left and right hands
    // are performing their specified poses.
    // </summary> 
    // <details>
    // An extended version of a static gesture may include the rotation of the
    // Skeleton head. This was not included in the current version as there was
    // no use-case for this check as the player head may express different
    // states of emotion while performing the same gesture.
    // </details>
    public class StaticGesture : ScriptableObject {
        /// <summary>
        /// The gesture's left hand settings
        /// </summary>
        public HandGesture leftHandGesture;

        /// <summary>
        /// The gesture's right hand settings
        /// </summary>
        public HandGesture rightHandGesture;

        /// <summary>
        /// Returns whether a gesture is fulfilled based on the input VRSkeleton
        /// </summary>
        /// <param name="checkedSkeleton">The skeleton to check</param>
        /// <param name="isCheckMirrored">
        /// Whether to mirror the pose when checking for fulfillment.
        /// Default: false
        /// </param>
        /// <returns>True if the gesture is fulfilled; false otherwise</returns>
        public bool IsFulfilled(VRSkeleton checkedSkeleton, bool isCheckMirrored = false) {
            HandGesture leftGestureToCheck;
            HandGesture rightGestureToCheck;
            if (isCheckMirrored) {
                leftGestureToCheck = rightHandGesture.GetGestureMirroredAcrossBodyAxis();
                rightGestureToCheck = leftHandGesture.GetGestureMirroredAcrossBodyAxis();
            } else {
                leftGestureToCheck = leftHandGesture;
                rightGestureToCheck = rightHandGesture;
            }

            bool isLeftHandFulfilled = 
                leftGestureToCheck.IsFulfilled(checkedSkeleton.leftHand, checkedSkeleton.leftShoulder);
            bool isRightHandFulfilled = 
                rightGestureToCheck.IsFulfilled(checkedSkeleton.rightHand, checkedSkeleton.rightShoulder);
            return isLeftHandFulfilled && isRightHandFulfilled;
        }
    }
}
