using Material;
using Util;
using UnityEngine;
using World;

namespace WorldGen {
    public class TileGenerator {
        private readonly WorldOptions _options;

        public TileGenerator (WorldOptions options) {
            this._options = options;
        }

        public float[,] GenerateNoiseMap (TilePos pos) {
            var noiseMap = Noise.GenerateNoiseMap (16, 16, _options.Seed, _options.NoiseScale, _options.Octaves,
                _options.Persistance, _options.Lacunarity,
                pos.ToVector2 ());
            for (var x = 0; x < 16; x++) {
                for (var z = 0; z < 16; z++) {
                    noiseMap[x, z] = _options.HeightCurve.Evaluate (noiseMap[x, z]) * _options.MaxHeight;
                }
            }

            return noiseMap;
        }

        public Tile GenerateTile (TilePos pos, float[,] noiseMap) {
            var startPos = pos.ToVoxelPos ();
            var tile = new Tile (pos);
            if (pos.Y << 4 >= _options.MaxHeight) {
                return tile;
            }

            // TODO debug
            foreach (var voxelPos in tile) {
                var noiseValue = noiseMap[voxelPos.X - startPos.X, voxelPos.Z - startPos.Z];
                if (noiseValue >= voxelPos.Y && noiseValue < voxelPos.Y + 1) {
                    tile.SetVoxelAt (voxelPos,
                        voxelPos.Y > 3
                            ? new Voxel (noiseValue - voxelPos.Y, Builtin.Pasto)
                            : new Voxel (noiseValue - voxelPos.Y, Builtin.Struix));
                } else if (noiseValue < voxelPos.Y) {
                    tile.SetVoxelAt (voxelPos, new Voxel (0.0f, Builtin.Pasto));
                } else {
                    tile.SetVoxelAt (voxelPos, new Voxel (1.0f, Builtin.Struix));
                }
            }

            return tile;
        }
    }
}