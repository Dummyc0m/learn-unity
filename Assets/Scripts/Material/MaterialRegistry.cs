using System.Collections.Generic;

namespace Material {
    public class MaterialRegistry {
        private int _materialId;
        private readonly IDictionary<int, Material> _materialMap;

        private MaterialRegistry () {
            _materialMap = new Dictionary<int, Material> ();
        }

        public int RegisterMaterial (Material mat) {
            int matId;
            lock (this) {
                matId = _materialId++;
            }

            _materialMap[matId] = mat;
            return matId;
        }

        public Material GetMaterial (int id) {
            return _materialMap[id];
        }

        public static readonly MaterialRegistry Instance = new MaterialRegistry ();
    }
}