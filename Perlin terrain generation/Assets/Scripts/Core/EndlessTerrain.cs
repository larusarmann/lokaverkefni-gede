using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public Transform player;
    public Material terrainMaterial;

    public float heightMultiplier = 5f;
    public int viewDistance = 2;

    public NoiseSettings noiseSettings = new NoiseSettings();

    private Dictionary<Vector2, TerrainChunk> chunks = new Dictionary<Vector2, TerrainChunk>();
    private Vector2 playerOldChunkCoord;

    private void Start()
    {
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
                        terrainMaterial
                    );

                    chunks.Add(viewedChunkCoord, newChunk);
                }
            }
        }
    }
}