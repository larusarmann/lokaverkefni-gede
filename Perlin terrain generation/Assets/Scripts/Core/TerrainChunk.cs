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
        meshCollider.sharedMesh = mesh; // This allows the Raycast to "hit" the terrain
        meshRenderer.material = material;

        // Spawn trees and stones after the mesh and collider are ready
        SpawnObjects(treeSettings, noiseMap, settings, heightMultiplier, worldX, worldZ);
        SpawnObjects(stoneSettings, noiseMap, settings, heightMultiplier, worldX, worldZ);
    }

    void SpawnObjects(AssetSettings assetSet, float[,] noiseMap, NoiseSettings settings, float heightMultiplier, float worldX, float worldZ)
    {
        if (assetSet.prefabs == null || assetSet.prefabs.Length == 0) return;

        System.Random prng = new System.Random((int)(worldX + worldZ * 1000));

        for (int i = 0; i < assetSet.density; i++)
        {
            int x = prng.Next(0, settings.width);
            int y = prng.Next(0, settings.height);
            float heightValue = noiseMap[x, y];

            // This line ensures trees ONLY spawn above the water/beach level
            if (heightValue >= assetSet.minHeight && heightValue <= assetSet.maxHeight)
            {
                float posX = worldX + x;
                float posZ = worldZ + y;
                float posY = heightValue * heightMultiplier;

                GameObject prefab = assetSet.prefabs[prng.Next(0, assetSet.prefabs.Length)];
                
                // Initial spawn height
                Vector3 spawnPos = new Vector3(posX, posY + 20f, posZ);
                GameObject obj = Object.Instantiate(prefab, spawnPos, Quaternion.Euler(0, prng.Next(0, 360), 0));
                obj.transform.parent = meshObject.transform;

                // Apply size
                float randomScale = (float)(prng.NextDouble() * (assetSet.maxScale - assetSet.minScale) + assetSet.minScale);
                obj.transform.localScale = Vector3.one * randomScale;

                // "Shoot" the tree down so it sits perfectly on the sloped terrain
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