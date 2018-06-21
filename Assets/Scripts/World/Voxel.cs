using Material;

namespace World {
    public struct Voxel {
        public static readonly Voxel Void = new Voxel(0f, Material.Builtin.Nilum);
        public readonly float Density;
        public readonly int MaterialId;

        public Voxel (float density, int materialId) {
            Density = density;
            MaterialId = materialId;
        }

        public Voxel setDensity (float newDensity) {
            return new Voxel (newDensity, MaterialId);
        }
    }
}