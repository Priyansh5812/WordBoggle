using UnityEngine;


public static class Util
{
    public static Color GetComplementary(Color originalColor)
    {
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);
        h = (h + 0.5f) % 1f; // 180° hue shift
        Color complement = Color.HSVToRGB(h, s, v);
        return complement;
    }
}
