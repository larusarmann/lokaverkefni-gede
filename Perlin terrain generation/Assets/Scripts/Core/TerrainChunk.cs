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
        
        // Sets the layer so the Player can collide with it
        meshObject.layer = LayerMask.NameToLayer("Default"); 

        // We only declare these variables ONCE
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshCollider meshCollider = meshObject.AddComponent<MeshCollider>();

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(settings, worldX, worldZ);
        Mesh mesh = MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier);

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        // Force physics to update immediately so the player doesn't fall through
        Physics.SyncTransforms();

        if (material != null)
        {
            meshRenderer.material = material;
        }
        else
        {
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }
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