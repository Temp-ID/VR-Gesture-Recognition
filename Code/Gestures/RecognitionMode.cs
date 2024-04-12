namespace TempID.VR.Skeleton.Gestures {
    /// <summary>
    /// Used to help identify the way recognition of gesture states are handled.
    /// </summary>
    public enum RecognitionMode
    {
        /// <summary>
        /// Inside RecognitionMode means values must fit inside bounded values
        /// to be considered valid.
        ///
        /// I.e. "allow everything within [min, max]"
        /// </summary>
        Inside,
        /// <summary>
        /// Outside RecognitionMode means values must be outside bounded values
        /// to be considered valid.
        ///
        /// I.e. "allow everything that is not within [min, max]"
        /// </summary>
        Outside
    }
}