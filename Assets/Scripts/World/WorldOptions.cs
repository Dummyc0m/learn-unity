using UnityEngine;

namespace World {
    public struct WorldOptions {
        public readonly float NoiseScale;

        public readonly int Octaves;
        public readonly float Persistance;
        public readonly float Lacunarity;

        public readonly int MaxHeight;
        public readonly AnimationCurve HeightCurve;

        public readonly int Seed;

        public WorldOptions(float noiseScale, int octaves, float persistance, float lacunarity, int maxHeight, AnimationCurve heightCurve, int seed) {
            NoiseScale = noiseScale;
            Octaves = octaves;
            Persistance = persistance;
            Lacunarity = lacunarity;
            MaxHeight = maxHeight;
            HeightCurve = heightCurve;
            Seed = seed;
        }
    }
}