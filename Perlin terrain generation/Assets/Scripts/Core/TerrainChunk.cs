using UnityEngine;

public class TerrainChunk
{
    private GameObject meshObject;
    private Vector2 coord;

    public TerrainChunk(Vector2 coord, NoiseSettings settings, float heightMultiplier, Transform parent, Material material)
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

        // Generate the texture!
        Texture2D texture = TextureGenerator.TextureFromHeightMap(noiseMap);

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        // Apply the material and our new texture
        Material chunkMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        chunkMaterial.mainTexture = texture; // This makes the colors actually show up!
        meshRenderer.material = chunkMaterial;
    }

    public void SetVisible(bool visible)
    {
        if (meshObject != null)
        {
            meshObject.SetActive(visible);
        }
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