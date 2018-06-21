using UnityEngine;
using System.Collections;

public static class MeshGenerator {
    public static MeshData GenerateTerrainMesh (float[,] heightMap, float heightScale, AnimationCurve meshHeightCurve) {
        int width = heightMap.GetLength (0);
        int height = heightMap.GetLength (1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData (width, height, false);
        int vertexIndex = 0;

        for (int z = 0; z < height - 1; z++) {
            for (int x = 0; x < width; x++) {
                meshData.vertices[vertexIndex] = new Vector3 (topLeftX + x,
                    meshHeightCurve.Evaluate (heightMap[x, z]) * heightScale, topLeftZ - z);
                meshData.uvs[vertexIndex] = new Vector2 (x / (float) width, z / (float) height);

                if (x < width - 1 && z < height - 1) {
                    meshData.AddTriangle (vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle (vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public static MeshData GenerateTerrainMeshColored (float[,] heightMap, float heightScale,
                                                       AnimationCurve meshHeightCurve, TerrainType[] regions) {
        int width = heightMap.GetLength (0);
        int height = heightMap.GetLength (1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData (width, height, true);
        int vertexCount = 0;

        for (int z = 0; z < height - 1; z++) {
            for (int x = 0; x < width - 1; x++) {
                // add 6 vertices and 2 triangles

                meshData.AddTriangle (vertexCount, vertexCount + 1, vertexCount + 2);
                float height1 = meshHeightCurve.Evaluate (heightMap[x, z]);
                float height2 = meshHeightCurve.Evaluate (heightMap[x + 1, z + 1]);
                float height3 = meshHeightCurve.Evaluate (heightMap[x, z + 1]);
                float averageHeight = (height1 + height2 + height3) / 3;
                Color32 regionColor = GetRegionColor (regions, averageHeight);
                meshData.color32s[vertexCount] = regionColor;
                meshData.vertices[vertexCount++] = new Vector3 (topLeftX + x, height1 * heightScale, topLeftZ - z);
                meshData.color32s[vertexCount] = regionColor;
                meshData.vertices[vertexCount++] =
                    new Vector3 (topLeftX + x + 1, height2 * heightScale, topLeftZ - (z + 1));
                meshData.color32s[vertexCount] = regionColor;
                meshData.vertices[vertexCount++] =
                    new Vector3 (topLeftX + x, height3 * heightScale, topLeftZ - (z + 1));

                height1 = meshHeightCurve.Evaluate (heightMap[x + 1, z + 1]);
                height2 = meshHeightCurve.Evaluate (heightMap[x, z]);
                height3 = meshHeightCurve.Evaluate (heightMap[x + 1, z]);
                meshData.AddTriangle (vertexCount, vertexCount + 1, vertexCount + 2);
                averageHeight = (height1 + height2 + height3) / 3;
                regionColor = GetRegionColor (regions, averageHeight);
                meshData.color32s[vertexCount] = regionColor;
                meshData.vertices[vertexCount++] =
                    new Vector3 (topLeftX + x + 1, height1 * heightScale, topLeftZ - (z + 1));
                meshData.color32s[vertexCount] = regionColor;
                meshData.vertices[vertexCount++] = new Vector3 (topLeftX + x, height2 * heightScale, topLeftZ - z);
                meshData.color32s[vertexCount] = regionColor;
                meshData.vertices[vertexCount++] = new Vector3 (topLeftX + x + 1, height3 * heightScale, topLeftZ - z);
            }
        }

        return meshData;
    }

    private static Color32 GetRegionColor (TerrainType[] regions, float height) {
        for (int i = 0; i < regions.Length; i++) {
            if (height <= regions[i].height) {
                return regions[i].colour;
            }
        }

        return regions[regions.Length - 1].colour;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Color32[] color32s;

    int triangleIndex;

    public MeshData (int meshWidth, int meshHeight, bool colored) {
        if (colored) {
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
            vertices = new Vector3[triangles.Length];
            color32s = new Color32[triangles.Length];
        } else {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }
    }

    public void AddTriangle (int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh () {
        Mesh mesh = new Mesh ();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors32 = color32s;
        mesh.RecalculateNormals ();
        return mesh;
    }
}