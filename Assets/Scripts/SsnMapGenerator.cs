using Client.Render;
using Util;
using UnityEngine;
using World;

public class SsnMapGenerator : MonoBehaviour {
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range (0, 1)] public float persistance;
    public float lacunarity;

    public float heightScale;
    public int maxHeight;
    public AnimationCurve meshHeightCurve;

    public int seed;

    public bool AutoUpdate;

    public void GenerateMap () {
        var world = new World.World (new WorldOptions (noiseScale, octaves, persistance, lacunarity, maxHeight,
            meshHeightCurve,
            seed));

        var clientWorld = new Client.World.BufferedWorld ();
        var pos = new TilePos (0, 0, 0);

        // todo generate some sample chunks
        for (var i = 0; i < 8; i++) {
            var voxelOffset = SurfaceNetUtil.GetVoxelOffset (i);
            var tilePos = new TilePos(voxelOffset.X, voxelOffset.Y, voxelOffset.Z);
            var tile = world.LoadTile (tilePos);
            clientWorld.LoadTile (tile);
        }

        var meshes = clientWorld.Mesh;

        var display = FindObjectOfType<MapDisplay> ();
        display.DrawMesh (mesh);
    }

    private void OnValidate () {
        if (mapWidth < 1) {
            mapWidth = 1;
        }

        if (mapHeight < 1) {
            mapHeight = 1;
        }

        if (lacunarity < 1) {
            lacunarity = 1;
        }

        if (octaves < 0) {
            octaves = 0;
        }

        if (heightScale < 0) {
            heightScale = 0;
        }
    }
}