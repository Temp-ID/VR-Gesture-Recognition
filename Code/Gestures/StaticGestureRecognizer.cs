using System;
using UnityEngine;
using UnityEngine.Events;

namespace TempID.VR.Skeleton.Gestures {
    /// <summary>
    /// Used to recognize if a VRSkeleton is performing a StaticGesture.
    /// 
    /// Provides hooks for listeners to react posing events.
    /// - OnGestureStart
    /// - OnGestureHeld
    /// - OnGestureEnd
    /// </summary>
    public class StaticGestureRecognizer : MonoBehaviour {
        #region Types

        /// <summary>
        /// The type of check to perform when determining if the gesture is
        /// being performed.
        /// </summary>
        public enum CheckType {
            // Standard checks will only be fulfilled if the gesture is 
            // performed exactly as defined
            Standard,
            // Mirrored checks will only be fulfilled if the gesture is 
            // performed such that the gesture if mirrored across the body-axis
            Mirrored,
            // StandardOrMirror checks will be fulfilled if the gesture is
            // fulfilled in either Standard or in Mirrored checks.
            StandardOrMirrored
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns whether or not the gesture is currently being recognized
        /// </summary>
        public bool IsGesturePerformed {
            get { 
                return wasGesturePerformedPreviousFrame;
            }
        }

        #region Inspector Properties

        [Header("Recognition Targets")]

        [Tooltip("The gesture to recognized")]
        public StaticGesture gestureTarget;

        [Tooltip("The skeleton we're checking against.")]
        public VRSkeleton skeletonToCheck;

        [Header("Recognition Options")]

        [Tooltip("The type of check to perform when determining if the gesture is fulfilled.")]
        public CheckType checkType = CheckType.Standard;

        [Tooltip("Called when the gesture is first recognized")]
        public UnityEvent<StaticGestureRecognizer, StaticGesture> OnGestureStart;

        [Tooltip("Called every frame the gesture is held")]
        public UnityEvent<StaticGestureRecognizer, StaticGesture> OnGestureHeld;

        [Tooltip("Called the first frame when the gesture stops being performed")]
        public UnityEvent<StaticGestureRecognizer, StaticGesture> OnGestureEnd;

        #endregion

        /// <summary>
        /// Holds whether or not the gesture was performed in the previous game
        /// frame.
        /// </summary>
        private bool wasGesturePerformedPreviousFrame = false;

        #endregion

        #region Life Cycle

        /// <summary>
        /// Checks if the gesture was performed, and holds the gesture if it is
        /// being performed.
        /// </summary>
        void Update() {
            bool isFulfilled = IsGestureFulfilled(gestureTarget, skeletonToCheck, checkType);
            if (isFulfilled) {
                if (wasGesturePerformedPreviousFrame) {
                    OnGestureHeld.Invoke(this, gestureTarget);
                } else {
                    OnGestureStart.Invoke(this, gestureTarget);
                }
            } else if (wasGesturePerformedPreviousFrame) {
                OnGestureEnd.Invoke(this, gestureTarget);
            }
            wasGesturePerformedPreviousFrame = isFulfilled;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Determines if the input gesture is being performed by the input
        /// VRSkeleton.
        /// </summary>
        /// <param name="gestureToFulfill">The gesture</param>
        /// <param name="skeleton">The skeleton to check against</param>
        /// <param name="checkType">The type of check to perform</param>
        /// <returns>
        /// true if the gesture is being performed; false otherwise
        /// </returns>
        private bool IsGestureFulfilled(StaticGesture gestureToFulfill, VRSkeleton skeleton, CheckType checkType) {
            return checkType switch {
                CheckType.Standard => gestureToFulfill.IsFulfilled(skeleton, isCheckMirrored: false),
                CheckType.Mirrored => gestureToFulfill.IsFulfilled(skeleton, isCheckMirrored: true),
                CheckType.StandardOrMirrored => gestureToFulfill.IsFulfilled(skeleton, isCheckMirrored: false)
                    || gestureToFulfill.IsFulfilled(skeleton, isCheckMirrored: true),
                _ => throw new NotImplementedException($"IsPosePerformed missing case for {checkType}"),
            };
        }

        #endregion
    }
}
