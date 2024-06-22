namespace Pandora.MeshGradient
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class RandomColorGenerator
    {
        public static Color GenerateRandomColor()
        {
            var hue = Random.Range(0f, 1f); // Hue from 0 to 1
            var saturation = Random.Range(0.5f, 1f); // Saturation from 0.5 to 1 for vivid colors
            var value = Random.Range(0.7f, 1f); // Value from 0.7 to 1 to avoid dark colors

            return Color.HSVToRGB(hue, saturation, value);
        }


        public static List<Color> GenerateColorPalette(int amount)
        {
            var colorPalette = new List<Color>();
            if (amount <= 0) return colorPalette;

            var baseHue = Random.Range(0f, 1f); // Base hue for the palette
            var hueIncrement = 0.5f / amount; // Increment hue to ensure a variety of colors in the same palette

            for (var i = 0; i < amount; i++)
            {
                var hue = (baseHue + (i * hueIncrement)) % 1f; // Ensure hue stays within [0, 1]
                var saturation = Random.Range(0.5f, 1f); // Vivid colors
                var value = Random.Range(0.8f, 1f); // Avoid dark colors

                var newColor = Color.HSVToRGB(hue, saturation, value);
                colorPalette.Add(newColor);
            }

            return colorPalette;
        }
    }
}