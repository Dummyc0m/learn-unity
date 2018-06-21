using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Util;
using World;
using static Util.Contracts;

namespace Client.World {
    public class BufferedWorld {
        private readonly IDictionary<TilePos, BufferedTile> _loadedTiles;
        private Client.Render.MeshBuf[] _bufferedMesh;

        // TODO consider the case where tiles are updated
        // TODO OnSet Voxel, need to notify nearby 8 voxels to recalculate ssnCube
        public Client.Render.MeshBuf[] Mesh {
            get {
                return _bufferedMesh ??
                       (_bufferedMesh = _loadedTiles.Values.Select (e => e.GenerateMesh (this)).ToArray ());
            }
        }

        public BufferedWorld () {
            _loadedTiles = new Dictionary<TilePos, BufferedTile> ();
        }

        [CanBeNull]
        public BufferedTile GetTileAt (TilePos pos) {
            return _loadedTiles[pos];
        }

        public Voxel GetVoxelAt (VoxelPos pos) {
            var tilePos = pos.ToTilePos ();
            return GetTileAt (tilePos)?.GetVoxelAt (pos) ?? Voxel.Void;
        }

        public bool IsTileLoaded (TilePos pos) {
            return _loadedTiles.ContainsKey (pos);
        }

        public bool IsVoxelLoaded (VoxelPos pos) {
            return IsTileLoaded (pos.ToTilePos ());
        }

        public void LoadTile (Tile tile) {
            Requires (tile != null);
            Requires (!IsTileLoaded (tile.Pos));

            lock (this) {
                _loadedTiles[tile.Pos] = new BufferedTile (tile);
                _bufferedMesh = null;
            }

            Ensures (IsTileLoaded (tile.Pos));
        }
    }
}