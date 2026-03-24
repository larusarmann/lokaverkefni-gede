using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = heightMap[x, y];

                // --- Your awesome color logic ---
                if (currentHeight < 0.2f)
                {
                    colorMap[y * width + x] = new Color(0.1f, 0.3f, 0.6f); // Deep Water Blue
                }
                else if (currentHeight < 0.4f)
                {
                    colorMap[y * width + x] = new Color(0.9f, 0.8f, 0.6f); // Sand Yellow
                }
                else if (currentHeight < 0.7f)
                {
                    colorMap[y * width + x] = new Color(0.2f, 0.6f, 0.2f); // Grass Green
                }
                else
                {
                    colorMap[y * width + x] = Color.white; // Snow Cap
                }
            }
        }

        Texture2D texture = new Texture2D(width, height);
        // Point filter keeps the color edges crisp instead of blurry!
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }
}