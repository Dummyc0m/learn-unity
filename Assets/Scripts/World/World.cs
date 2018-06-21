using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Material;
using Util;
using WorldGen;
using static Util.Contracts;

namespace World {
    public class World {
        private readonly IDictionary<TilePos, Tile> _loadedTiles;
        private readonly TileGenerator _generator;

        public World (WorldOptions options) {
            _loadedTiles = new Dictionary<TilePos, Tile> ();
            _generator = new TileGenerator (options);
        }

        [CanBeNull]
        public Tile GetTileAt (TilePos pos) {
            return _loadedTiles[pos];
        }

        public Voxel? GetVoxelAt (VoxelPos pos) {
            var tilePos = pos.ToTilePos ();
            return GetTileAt (tilePos)?.GetVoxelAt (pos);
        }

        public bool IsTileLoaded (TilePos pos) {
            return _loadedTiles.ContainsKey (pos);
        }

        public bool IsVoxelLoaded (VoxelPos pos) {
            return IsTileLoaded (pos.ToTilePos ());
        }

        // todo make private?
        public Tile LoadTile (TilePos pos) {
            Requires (!IsTileLoaded (pos));
            
            var tile = _generator.GenerateTile (pos, _generator.GenerateNoiseMap (pos));
            _loadedTiles[pos] = tile;
            
            Ensures (IsTileLoaded (pos));
            return tile;
        }
    }
}