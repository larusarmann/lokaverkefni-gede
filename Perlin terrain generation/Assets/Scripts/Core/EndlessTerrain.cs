using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public Transform player;
    public Material terrainMaterial;

    public float heightMultiplier = 5f;
    public int viewDistance = 2;

    public NoiseSettings noiseSettings = new NoiseSettings();

    [Header("Object Spawning")]
    public AssetSettings treeSettings;
    public AssetSettings stoneSettings;

    private Dictionary<Vector2, TerrainChunk> chunks = new Dictionary<Vector2, TerrainChunk>();
    private Vector2 playerOldChunkCoord;

    private void Start()
    {
        if (noiseSettings.useRandomSeed)
        {
            noiseSettings.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        UpdateChunks();
    }

    private void Update()
    {
        Vector2 currentChunkCoord = GetPlayerChunkCoord();

        if (currentChunkCoord != playerOldChunkCoord)
        {
            playerOldChunkCoord = currentChunkCoord;
            UpdateChunks();
        }
    }

    private Vector2 GetPlayerChunkCoord()
    {
        int currentChunkX = Mathf.FloorToInt(player.position.x / (noiseSettings.width - 1));
        int currentChunkY = Mathf.FloorToInt(player.position.z / (noiseSettings.height - 1));

        return new Vector2(currentChunkX, currentChunkY);
    }

    private void UpdateChunks()
    {
        Vector2 currentChunkCoord = GetPlayerChunkCoord();

        for (int yOffset = -viewDistance; yOffset <= viewDistance; yOffset++)
        {
            for (int xOffset = -viewDistance; xOffset <= viewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(
                    currentChunkCoord.x + xOffset,
                    currentChunkCoord.y + yOffset
                );

                if (!chunks.ContainsKey(viewedChunkCoord))
                {
                    TerrainChunk newChunk = new TerrainChunk(
                        viewedChunkCoord,
                        noiseSettings,
                        heightMultiplier,
                        transform,
                        terrainMaterial,
                        treeSettings,   // Passed to chunk
                        stoneSettings   // Passed to chunk
                    );

                    chunks.Add(viewedChunkCoord, newChunk);
                }
            }
        }
    }
}