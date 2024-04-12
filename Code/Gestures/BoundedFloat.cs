namespace TempID.VR.Skeleton.Gestures {
    /// <summary>
    /// BoundedFloat is used to define the minimum and maximum permissible
    /// values for a float.
    /// </summary>
    [System.Serializable]
    public struct BoundedFloat {
        /// <summary>
        /// The minimum value
        /// </summary>
        public float min;

        /// <summary>
        /// The maximum value
        /// </summary>
        public float max;
    }
}