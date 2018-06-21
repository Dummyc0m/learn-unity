using System.Collections;
using System.Collections.Generic;
using Client.Render;
using Util;
using World;

namespace Client.World {
    public class BufferedTile : IEnumerable<VoxelPos> {
        private Tile _tile;
        private SurfaceNetCube[,,] _bufferedSurfaceNetCubes;
        private MeshBuf _bufferedMeshBuf;
        private Render.MeshGenerator.LevelOfDetail _levelOfDetail = Render.MeshGenerator.LevelOfDetail.Medium;

        public TilePos Pos => _tile.Pos;
        public VoxelPos StartPos => _tile.StartPos;
        public VoxelPos EndPos => _tile.EndPos;

        public Render.MeshGenerator.LevelOfDetail LevelOfDetail {
            get => _levelOfDetail;
            set {
                lock (this) {
                    _levelOfDetail = value;
                    _bufferedSurfaceNetCubes = null;
                    _bufferedMeshBuf = null;
                }
            }
        }

        public BufferedTile (Tile tile) {
            _tile = tile;
        }

        public Render.MeshBuf GenerateMesh (BufferedWorld world) {
            var buffered = _bufferedMeshBuf;
            if (buffered != null) return buffered;
            if (_bufferedSurfaceNetCubes == null) {
                _bufferedSurfaceNetCubes =
                    Render.MeshGenerator.Instance.GenerateSurfaceNetCubes (world, Pos, LevelOfDetail);
            }
            _bufferedMeshBuf = Render.MeshGenerator.Instance.GenerateMesh (_bufferedSurfaceNetCubes);
            return _bufferedMeshBuf;
        }

        public void UpdateVoxel () {
            // TODO recalculate part of snCube
        }

        public void SetVoxelAt (VoxelPos pos, Voxel voxel) {
            _tile.SetVoxelAt (pos, voxel);
        }

        public Voxel GetVoxelAt (VoxelPos pos) {
            return _tile.GetVoxelAt (pos);
        }

        public IEnumerator<VoxelPos> GetEnumerator () {
            return _tile.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return _tile.GetEnumerator ();
        }
    }
}