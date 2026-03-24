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

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float rawHeight = heightMap[x, y];
                float currentHeight = rawHeight * heightMultiplier;

                vertices[vertexIndex] = new Vector3(x, currentHeight, y);
                // UVS are required so the texture knows how to wrap around the mesh!
                uvs[vertexIndex] = new Vector2((float)x / width, (float)y / height);

                if (x < width - 1 && y < height - 1)
                {
                    // FIXED WINDING ORDER (Clockwise) so lighting calculates correctly
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + width;
                    triangles[triangleIndex + 2] = vertexIndex + width + 1;

                    triangles[triangleIndex + 3] = vertexIndex;
                    triangles[triangleIndex + 4] = vertexIndex + width + 1;
                    triangles[triangleIndex + 5] = vertexIndex + 1;

                    triangleIndex += 6;
                }

                vertexIndex++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals(); // With the fixed triangles, normals will point UP now!

        return mesh;
    }
}