using UnityEngine;

using TempID.VR.Skeleton.Gestures.Utilities;

namespace TempID.VR.Skeleton.Gestures {
    /// <summary>
    /// HandGesture is used to define states that indicate a gesture is being
    /// performed by a single hand.
    ///
    /// HandGestures are defined by both direction and rotation.
    /// - Direction is handled by the vector from the hand to the connected
    ///   shoulder joint to the hand. For example, a gesture for a raised hand
    ///   would want the direction of the hand to be upwards of the shoulder,
    ///   while a gesture for a handshake may be lower and directly in front
    ///   of the body.
    /// - Rotation is handled by the way the hand is angled, relative to
    ///   user-space.
    /// </summary>
    [System.Serializable]
    public class HandGesture {
        #region Inspector Properties

        [Header("Directional values")]

        [Tooltip("The y bounds of the direction to the hands from the shoulder")]
        public BoundedFloat verticalBounds;

        [Tooltip("Defines how valid directions are handled for the left/right plane")]
        public RecognitionMode verticalDirectionMode = RecognitionMode.Inside;

        [Tooltip("The z bounds of the direction to the hands from the shoulder")]
        public BoundedFloat horizontalBounds;

        [Tooltip("Defines how valid directions are handled for the front/back plane")]
        public RecognitionMode horizontalDirectionMode = RecognitionMode.Inside;

        [Header("Rotational values")]

        [Tooltip("The x rotation bounds of the hand")]
        public BoundedFloat xRotBounds;

        [Tooltip("Defines how valid rotations handled for the x axis")]
        public RecognitionMode xRotRecognitionMode = RecognitionMode.Inside;

        [Tooltip("The y rotation bounds of the hand")]
        public BoundedFloat yRotBounds;

        [Tooltip("Defines how valid rotations handled for the y axis")]
        public RecognitionMode yRotRecognitionMode = RecognitionMode.Inside;

        [Tooltip("The z rotation bounds of the hand")]
        public BoundedFloat zRotBounds;

        [Tooltip("Defines how valid rotations handled for the z axis")]
        public RecognitionMode zRotRecognitionMode = RecognitionMode.Inside;

        #endregion

        #region Helper Functions

        /// <summary>
        /// Returns the mirrored version of this HandGesture as if it was
        /// reflected about the axis formed by the body-forward's plane
        /// </summary>
        public HandGesture GetGestureMirroredAcrossBodyAxis() {
            // Create a new HandGesture without keeping references to the
            // existing bounded floats (this works cos they're value-types)
            return new HandGesture{
                verticalBounds = new BoundedFloat{
                    min = verticalBounds.min,
                    max = verticalBounds.max
                },
                horizontalBounds = new BoundedFloat{
                    min = -1 * horizontalBounds.max,
                    max = -1 * horizontalBounds.min
                },
                xRotBounds = new BoundedFloat{
                    min = xRotBounds.min,
                    max = xRotBounds.max
                },
                yRotBounds = new BoundedFloat{
                    min = -1 * yRotBounds.max,
                    max = -1 * yRotBounds.min
                },
                zRotBounds = new BoundedFloat{
                    min = -1 * zRotBounds.max,
                    max = -1 * zRotBounds.min
                }
            };
        }

        /// <summary>
        /// Checks whether the input hand, represented by its Transform,
        /// fulfills this gesture.
        ///
        /// A hand gesture is considered fulfilled if:
        /// - The hand is within the direction left/right and
        ///   front/back angle bounds
        /// - The hand is within the allowed rotations
        /// </summary>
        /// <param name="handTransform">
        /// The world-space transform of the hand
        /// </param>
        /// <param name="shoulderTransform">
        /// The world-space transform of the shoulder-joint connected to the
        /// input handTransform
        /// </param>
        /// <param name="handGesture">The hand gesture to fulfill</param>
        /// <returns>true if the gesture is fulfilled; false otherwise</returns>
        public bool IsFulfilled(
            Transform handTransform,
            Transform shoulderTransform
        ) {
            Vector3 shoulderToHandDir =
                (shoulderTransform.InverseTransformPoint(handTransform.position)
                - shoulderTransform.localPosition).normalized;

            // Get the angle using the normal vector between the y and x point
            float horizontalAngle = Mathf.Rad2Deg * Mathf.Atan2(shoulderToHandDir.x, shoulderToHandDir.z);
            // Offset vertical angle; Acos(...) straight up is 0 degrees,
            // and straight down is 180 degrees.
            float verticalAngle = Mathf.Rad2Deg * (Mathf.PI / 2 -  Mathf.Acos(shoulderToHandDir.y));

            // Regarding why we have (Equals) here: we need to check the
            // mode that determines whether a selection is valid.
            // Essentially, let A = whether the angle is between min and max,
            // and let I = whether selection mode is inside.
            // We end up with the fulfillment being equal to
            // (A ^ I) v (~A ^ ~I), which can be simplified to A = I.
            bool horizontalDirFulfilled =
                IsValueInBounds(horizontalAngle, horizontalBounds.min, horizontalBounds.max)
                == Equals(horizontalDirectionMode, RecognitionMode.Inside);
            bool verticalDirFulfilled =
                IsValueInBounds(verticalAngle, verticalBounds.min, verticalBounds.max)
                == Equals(verticalDirectionMode, RecognitionMode.Inside);

            // Convert hand-space rotation to shoulder-space rotation,
            // used because the hands are not parented to the shoulders
            // due to physics reasons.
            //
            // This is needed because the hand's "y" rotation will differ
            // depending on the transform of the shoulder (rotation
            // of the player) otherwise.
            Vector3 localHandRotation =
                (Quaternion.Inverse(shoulderTransform.rotation) * handTransform.rotation).eulerAngles;

            // Wrap rotation to be bounded between -180 and 180 degrees
            // This function is not contained
            Vector3 handRotationWrapped =new Vector3(
                FloatUtilities.WrapRotation(localHandRotation.x),
                FloatUtilities.WrapRotation(localHandRotation.y),
                FloatUtilities.WrapRotation(localHandRotation.z)
            );

            bool isXAngleFulfilled =
                IsValueInBounds(handRotationWrapped.x, xRotBounds.min, xRotBounds.max)
                == Equals(xRotRecognitionMode, RecognitionMode.Inside);

            bool isYAngleFulfilled =
                IsValueInBounds(handRotationWrapped.y, yRotBounds.min, yRotBounds.max)
                == Equals(yRotRecognitionMode, RecognitionMode.Inside);

            bool isZAngleFulfilled =
                IsValueInBounds(handRotationWrapped.z, zRotBounds.min, zRotBounds.max)
                == Equals(zRotRecognitionMode, RecognitionMode.Inside);

            return horizontalDirFulfilled && verticalDirFulfilled
                && isXAngleFulfilled && isYAngleFulfilled && isZAngleFulfilled;
        }

        /// <summary>
        /// IsValueInBounds checks `minBound <= value <= maxBound`
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="minBound">The minimum value for the input</param>
        /// <param name="maxBound">The maximum value for the input</param>
        /// <returns>
        /// true if value is within minBound and maxBound; false otherwise
        /// </returns>
        private bool IsValueInBounds(float value, float minBound, float maxBound) {
            return value >= minBound && value <= maxBound;
        }

        #endregion
    }
}
