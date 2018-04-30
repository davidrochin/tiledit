using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {

    public class Math {

        public static int RoundDown(float f) {
            if (f > 0f) {
                return (int)f;
            } else {
                return (int)(f - 1f);
            }
        }

        public static int RoundNearest(float f) {
            float decimals = f % 1f;
            if (decimals <= 0.5f) {
                return (int)f;
            } else {
                return (int)(f + 1);
            }
        }

        public static float GetDecimal(float f) {
            return f % 1f;
        }

        public static Vector3 MiddleBetween(Vector3 a, Vector3 b) {
            Vector3 dir = b - a;
            return a + dir * (Vector3.Distance(a, b) * 0.5f);
        }
    }

}

