using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;


[CreateAssetMenu(fileName = "ComplementaryColors", menuName = "Scriptable Objects/ComplementaryColors")]
public class ColorConfig : ScriptableObject
{
    [SerializedDictionary("MainColor", "Complementary")]
    public SerializedDictionary<Color, Color> ColorDict;

    public Color GetComplementary(Color color)
    {
        if (ColorDict.ContainsKey(color))
            return ColorDict[color];
        else
        { 
            return Util.GetComplementary(color);
        }
    }

    public Color GetRandomColor()
    {
        int i = Random.Range(0, ColorDict.Count);
        foreach (var color in ColorDict)
        {   
            i--;
            if (i <= 0)
                return color.Key;
        }

        // Control won't come till here...
        return Color.black;
    }

    [ContextMenu("Refresh")]
    public void CalculateComplementary()
    {
        List<Color> colors = ColorDict.Keys.ToList();
        foreach (var c in colors)
        {
            ColorDict[c] = Util.GetComplementary(c);
        }

    }
}
