using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static utilities class for floats.
/// </summary>
/// <details>
/// In the original project, this is defined elsewhere to encourage reusability.
/// It is placed under this location purely to make understanding the demo 
/// project easier.
/// </details>
namespace TempID.VR.Skeleton.Gestures.Utilities {
    public static partial class FloatUtilities {
        /// <summary>
        /// Wraps the input degrees, bounds (-oo, oo), such that it is a 
        /// value (-180, 180]
        /// </summary>
        /// <param name="degrees">The degrees value to wrap</param>
        /// <returns>
        /// A value between (-180, 180] corresponding to the input value in 
        /// degrees
        /// </returns>
        public static float WrapRotation(float degrees) {
            float fullRotation = 360f;
            int totalWraps = (int)((Mathf.Sign(degrees) * (fullRotation / 2) + degrees) / fullRotation);
            return degrees - fullRotation * totalWraps;
        }
    }
}