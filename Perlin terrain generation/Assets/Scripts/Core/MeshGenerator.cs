using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GenerateTerrainMesh(float[,] heightMap, float heightMultiplier)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        Vector2[] uvs = new Vector2[width * height];
        Color[] colors = new Color[width * height]; // This stores the color for each point

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float rawHeight = heightMap[x, y];
                float currentHeight = rawHeight * heightMultiplier;

                vertices[vertexIndex] = new Vector3(x, currentHeight, y);
                uvs[vertexIndex] = new Vector2((float)x / width, (float)y / height);

                // --- Color logic based on height (0 to 1) ---
                if (rawHeight < 0.2f) {
                    colors[vertexIndex] = new Color(0.1f, 0.3f, 0.6f); // Deep Water Blue
                } else if (rawHeight < 0.4f) {
                    colors[vertexIndex] = new Color(0.9f, 0.8f, 0.6f); // Sand Yellow
                } else if (rawHeight < 0.7f) {
                    colors[vertexIndex] = new Color(0.2f, 0.6f, 0.2f); // Grass Green
                } else {
                    colors[vertexIndex] = Color.white; // Snow Cap
                }

                if (x < width - 1 && y < height - 1)
                {
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + width + 1;
                    triangles[triangleIndex + 2] = vertexIndex + width;

                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + 1;
                    triangles[triangleIndex + 5] = vertexIndex + width + 1;

                    triangleIndex += 6;
                }

                vertexIndex++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors; // Apply the colors to the mesh
        mesh.RecalculateNormals();

        return mesh;
    }
}