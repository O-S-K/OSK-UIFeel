#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
public class RandomRangeAttribute : PropertyAttribute
{
    public float Min { get; }
    public float Max { get; }

    public RandomRangeAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }
}
#endif
