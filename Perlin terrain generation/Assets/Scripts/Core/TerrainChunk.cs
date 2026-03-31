using UnityEngine;

public class TerrainChunk
{
    private GameObject meshObject;
    private Vector2 coord;

    public TerrainChunk(Vector2 coord, NoiseSettings settings, float heightMultiplier, Transform parent, Material material, AssetSettings treeSettings, AssetSettings stoneSettings)
    {
        this.coord = coord;

        float worldX = coord.x * (settings.width - 1);
        float worldZ = coord.y * (settings.height - 1);

        meshObject = new GameObject("Terrain Chunk " + coord.x + "," + coord.y);
        meshObject.transform.position = new Vector3(worldX, 0, worldZ);
        meshObject.transform.parent = parent;

        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshCollider meshCollider = meshObject.AddComponent<MeshCollider>();

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(settings, worldX, worldZ);
        Mesh mesh = MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier);

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.material = material;

        // FIX: Added different offsets (0 and 500) so they don't spawn in the same spot
        SpawnObjects(treeSettings, noiseMap, settings, heightMultiplier, worldX, worldZ, 0);
        SpawnObjects(stoneSettings, noiseMap, settings, heightMultiplier, worldX, worldZ, 500);
    }

    void SpawnObjects(AssetSettings assetSet, float[,] noiseMap, NoiseSettings settings, float heightMultiplier, float worldX, float worldZ, int seedOffset)
    {
        if (assetSet.prefabs == null || assetSet.prefabs.Length == 0) return;

        // FIX: The seed now includes the offset so trees and stones pick different spots
        System.Random prng = new System.Random((int)(worldX + worldZ * 1000) + seedOffset);

        for (int i = 0; i < assetSet.density; i++)
        {
            int x = prng.Next(0, settings.width);
            int y = prng.Next(0, settings.height);
            float heightValue = noiseMap[x, y];

            if (heightValue >= assetSet.minHeight && heightValue <= assetSet.maxHeight)
            {
                float posX = worldX + x;
                float posZ = worldZ + y;
                float posY = heightValue * heightMultiplier;

                GameObject prefab = assetSet.prefabs[prng.Next(0, assetSet.prefabs.Length)];
                
                // Spawn high and raycast down
                Vector3 spawnPos = new Vector3(posX, posY + 20f, posZ);
                GameObject obj = Object.Instantiate(prefab, spawnPos, Quaternion.Euler(0, prng.Next(0, 360), 0));
                obj.transform.parent = meshObject.transform;

                // IMPROVED SCALE: Now uses the min/max from AssetSettings
                float randomScale = (float)(prng.NextDouble() * (assetSet.maxScale - assetSet.minScale) + assetSet.minScale);
                obj.transform.localScale = Vector3.one * randomScale;

                if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 50f))
                {
                    obj.transform.position = hit.point;
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        if (meshObject != null) meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject != null && meshObject.activeSelf;
    }

    public Vector2 GetCoord()
    {
        return coord;
    }
}