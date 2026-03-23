using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(NoiseSettings settings, float offsetX, float offsetY)
    {
        float[,] noiseMap = new float[settings.width, settings.height];

        if (settings.scale <= 0)
        {
            settings.scale = 0.0001f;
        }

        for (int y = 0; y < settings.height; y++)
        {
            for (int x = 0; x < settings.width; x++)
            {
                float sampleX = (x + offsetX + settings.offsetX) / settings.scale;
                float sampleY = (y + offsetY + settings.offsetY) / settings.scale;

                float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = noiseValue;
            }
        }

        return noiseMap;
    }
}