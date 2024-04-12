using UnityEngine;
using UnityEditor;

namespace TempID.VR.Skeleton {
    /// <summary>
    /// Enables an object to follow the player's skeleton using the body 
    /// direction as the rotation and the HMD (head) as the position.
    /// </summary>
    /// <details>
    /// For the purposes of gesture recognition, this is used to estimate
    /// shoulder positions of the player. From deriving shoulder positions,
    /// we are able to determine the direction of players hands.
    /// 
    /// This class has custom Unity editor functions that will make the input
    /// object follow the input skeleton during scene-time loading.
    /// </details>
    /// <seealso cref="FollowSkeletonBodyEditor"/>
    public class FollowSkeletonBody : MonoBehaviour {
        [Tooltip("The skeleton to follow")]
        public VRSkeleton skeleton;

        [Tooltip("The offset of the following point")]
        public Vector3 offset;
        [Tooltip("The distance to follow at")]
        public float distance;
        [Tooltip("The angle to follow at")]
        public Vector3 angle;

        /// <summary>
        /// Updates the following position
        /// </summary>
        public void Update() {
            transform.position = GetFollowPosition();
        }

        /// <summary>
        /// Retrieves the world-space coordinates for following the skeleton
        /// </summary>
        /// <returns></returns>
        internal Vector3 GetFollowPosition() {
            Vector3 baseFollowPosition = skeleton.head.position + offset;

            Vector3 baseDir = skeleton.BodyDirectionGuess;
            Vector3 directionToDestination = Quaternion.Euler(angle.x, angle.y, angle.z) * baseDir;

            return baseFollowPosition + directionToDestination * distance;

        }
    }

    /// <summary>
    /// A custom editor for FollowSkeletonBody that will cause any object
    /// with this script to reposition itself during scene-time to match
    /// the values defined by the FollowSkeletonBody component.
    /// </summary>
    [CustomEditor(typeof(FollowSkeletonBody)), CanEditMultipleObjects]
    public class FollowSkeletonBodyEditor : Editor {
        /// <summary>
        /// Update the following position in-editor
        /// </summary>
        protected void OnSceneGUI() {
            FollowSkeletonBody fp = target as FollowSkeletonBody;
            if (fp.skeleton == null) {
                // Cannot follow nothing; value not yet set.
                return;
            }
            fp.transform.position = fp.GetFollowPosition();
        }
    }
}