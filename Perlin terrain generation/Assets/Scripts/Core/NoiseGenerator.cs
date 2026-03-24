using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(NoiseSettings settings, float worldX, float worldZ)
    {
        float[,] noiseMap = new float[settings.width, settings.height];

        // Use the seed to create random offsets for each octave
        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        for (int i = 0; i < settings.octaves; i++) {
            float offX = prng.Next(-100000, 100000) + settings.offsetX;
            float offZ = prng.Next(-100000, 100000) + settings.offsetY;
            octaveOffsets[i] = new Vector2(offX, offZ);
        }

        if (settings.scale <= 0) settings.scale = 0.0001f;

        // Keep track of the max possible height to normalize values later
        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < settings.octaves; i++) {
            maxPossibleHeight += amplitude;
            amplitude *= settings.persistence;
        }

        for (int y = 0; y < settings.height; y++)
        {
            for (int x = 0; x < settings.width; x++)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++)
                {
                    // Calculate sample coordinates using world position + octave offsets
                    float sampleX = (x + worldX + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y + worldZ + octaveOffsets[i].y) / settings.scale * frequency;

                    // Range -1 to 1 allows octaves to subtract height for more variety
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistence;
                    frequency *= settings.lacunarity;
                }

                // Normalize the value back to a 0-1 range
                float normalizedHeight = (noiseHeight + 1) / (2f * maxPossibleHeight / 1.75f);
                noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, 1);
            }
        }

        return noiseMap;
    }
}