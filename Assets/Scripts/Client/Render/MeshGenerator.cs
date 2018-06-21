using Client.World;
using Util;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Client.Render {
    public class MeshGenerator {
        private MeshGenerator () { }

        public SurfaceNetCube[,,] GenerateSurfaceNetCubes (BufferedWorld world, TilePos pos, LevelOfDetail lod) {
//            var snNets = new SurfaceNetCube[14, 14, 14];
            var lodFactor = 16 / lod.Skip;
            var snNets = new SurfaceNetCube[lodFactor, lodFactor, lodFactor];
            var tile = world.GetTileAt (pos);
            Contracts.Asserts (tile != null);
            for (var x = tile.StartPos.X; x < tile.EndPos.X; x += lod.Skip) {
                for (var y = tile.StartPos.Y; y < tile.EndPos.Y; x += lod.Skip) {
                    for (var z = tile.StartPos.Z; z < tile.EndPos.Z; x += lod.Skip) {
                        var sample = new float[8];
                        var voxelPos = new VoxelPos (x, y, z);

                        for (var i = 0; i < 8; i++) {
                            var offset = SurfaceNetUtil.GetVoxelOffset (i).Multiply (lod.Skip);
                            sample[i] = world.GetVoxelAt (offset.Add (x, y, z)).Density;
                        }

                        var snNet = SurfaceNetUtil.CalculateSurfaceNetCube (sample, world.GetVoxelAt (voxelPos));
                        snNet.Position = new Vector3 (snNet.Position.x + x, snNet.Position.y + y,
                            snNet.Position.z + z);
                        snNets[x / lod.Skip, y / lod.Skip, z / lod.Skip] = snNet;
                    }
                }
            }

//            foreach (var voxelPos in tile) {
////                if (voxelPos.X <= 0 || voxelPos.Y <= 0 || voxelPos.Z <= 0) {
////                    continue;
////                }
////
////                if (voxelPos.X >= 15 || voxelPos.Y >= 15 || voxelPos.Z >= 15) {
////                    continue;
////                }
//
//                var sample = new float[8];
//
//                for (var i = 0; i < 8; i++) {
//                    var offset = SurfaceNetUtil.GetVoxelCornerOffset (i);
//                    sample[i] = world.GetVoxelAt (voxelPos.Add ((int) offset.x, (int) offset.y, (int) offset.z))
//                        .Density;
//                }
//
//                var snNet = SurfaceNetUtil.CalculateSurfaceNetCube (sample, world.GetVoxelAt (voxelPos));
//                snNet.Position = new Vector3 (snNet.Position.x + voxelPos.X, snNet.Position.y + voxelPos.Y,
//                    snNet.Position.z + voxelPos.Z);
//                snNets[voxelPos.X, voxelPos.Y, voxelPos.Z] = snNet;
//            }

            return snNets;
        }

        public MeshBuf GenerateMesh (SurfaceNetCube[,,] snNets) {
            return SurfaceNetUtil.ComputeMesh (snNets);
        }

        public static readonly MeshGenerator Instance = new MeshGenerator ();

        public class LevelOfDetail {
            public static readonly LevelOfDetail Lowest = new LevelOfDetail (8);
            public static readonly LevelOfDetail Low = new LevelOfDetail (4);
            public static readonly LevelOfDetail Medium = new LevelOfDetail (2);
            public static readonly LevelOfDetail High = new LevelOfDetail (1);

            public readonly int Skip;

            private LevelOfDetail (int skip) {
                this.Skip = skip;
            }
        }
    }
}