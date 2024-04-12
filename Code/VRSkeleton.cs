using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempID.VR.Skeleton {
    /// <summary>
    /// VRSkeleton is used to define the body frame of a user in VR-Space.
    /// At the moment, this consists of three distinct body parts:
    /// - The user's head (calculated via their HMD)
    /// - The user's left and right shoulder (estimated via their HMD)
    /// - The user's left and right hands (calculated via their controllers)
    /// </summary>
    public class VRSkeleton : MonoBehaviour {
        [Tooltip("The user's head")]
        public Transform head;

        [Header("Left side")]

        [Tooltip("The transform of the left shoulder in the gesture")]
        public Transform leftShoulder;

        [Tooltip("The transform of the left hand")]
        public Transform leftHand;

        [Header("Right side")]

        [Tooltip("The transform of the right shoulder in the gesture")]
        public Transform rightShoulder;

        [Tooltip("The transform of the right hand")]
        public Transform rightHand;

        /// <summary>
        /// Retrieves a best-guess on which way the player's body is currently facing.
        /// 
        /// This best-guess is the forward-vector of the camera on the horizontal plane.
        /// </summary>
        public Vector3 BodyDirectionGuess {
            get {
                Vector3 direction = Vector3.ProjectOnPlane(head.forward, Vector3.up);
                if (Vector3.Dot(head.up, Vector3.up) < 0) {
                    // Player's head is rotated backwards; body direction needs to be inverted to preserve
                    direction = -direction;
                }
                return direction;
            }
        }
    }
}