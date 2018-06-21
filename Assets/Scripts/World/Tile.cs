using System.Collections;
using System.Collections.Generic;
using Material;
using Util;

namespace World {
    public class Tile : IEnumerable<VoxelPos> {
        private Voxel[,,] DensityMap { get; }
        public TilePos Pos { get; }
        public VoxelPos StartPos { get; }
        public VoxelPos EndPos { get; }

        public Tile (TilePos pos) {
            DensityMap = new Voxel[16, 16, 16];
            Pos = pos;
            StartPos = pos.ToVoxelPos ();
            EndPos = pos.Add (1, 1, 1).ToVoxelPos ();
        }

        public IEnumerator<VoxelPos> GetEnumerator () {
            for (var x = StartPos.X; x < EndPos.X; x++) {
                for (var z = StartPos.Z; z < EndPos.Z; z++) {
                    for (var y = StartPos.Y; y < EndPos.Y; y++) {
                        yield return new VoxelPos (x, y, z);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator ();
        }

        public void SetVoxelAt (VoxelPos pos, Voxel voxel) {
            this.DensityMap[pos.X - StartPos.X, pos.Y - StartPos.Y, pos.Z - StartPos.Z] = voxel;
        }

        public Voxel GetVoxelAt (VoxelPos pos) {
            return this.DensityMap[pos.X - StartPos.X, pos.Y - StartPos.Y, pos.Z - StartPos.Z];
        }
    }
}