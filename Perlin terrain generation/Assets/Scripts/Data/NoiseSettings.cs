using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public int width = 100;
    public int height = 100;
    public float scale = 20f;

    // --- New Settings for Detail Layers ---
    public int octaves = 4;              // Number of layers (e.g., 1=blobby, 4-6=detailed)
    [Range(0, 1)]
    public float persistence = 0.5f;     // How much each layer's height decreases
    public float lacunarity = 2f;        // How much each layer's detail increases
    public int seed;                     // Random seed for the world
    // --------------------------------------

    public float offsetX = 0f;
    public float offsetY = 0f;
}