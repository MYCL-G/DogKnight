using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    static float doThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var v3ToTarget = target.position - transform.position;
        v3ToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, v3ToTarget);
        return dot >= doThreshold;
    }
}
