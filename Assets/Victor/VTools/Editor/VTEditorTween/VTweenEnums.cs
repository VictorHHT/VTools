using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.EditorTween
{
    public enum TweenMethod { TweenFunction, CubicBezier };
    public enum PlayDirection { Forward, Backward };
    public enum PlayStyle { Normal, PingPong };
    public enum RotateMode
    {
        /// <summary>
        /// Faster way that don't rotate beyond 360°, along shorter arc
        /// </summary>
        Fast,
        /// <summary>
        /// Slower way that don't rotate beyond 360°, along longer arc
        /// </summary>
        Slow
    }
}
