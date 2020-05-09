using System.Collections.Generic;
using UnityEngine;
public class OtherUtil
{
    public const float PRECISION = 0.0001f;
    public static bool IsEqual(float a, float b)
    {
        return Mathf.Abs(a - b) < PRECISION;
    }

    public static void Rgba8888ToColor(ref Color color, int value)
    {
        color.r = ((value & 0xff000000) >> 24) / 255f;
        color.g = ((value & 0x00ff0000) >> 16) / 255f;
        color.b = ((value & 0x0000ff00) >> 8) / 255f;
        color.a = ((value & 0x000000ff)) / 255f;
    }
}
