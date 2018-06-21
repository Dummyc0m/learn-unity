using UnityEngine;

namespace Client.Render {
    public struct SurfaceNetCube {
        public static readonly SurfaceNetCube Zero = new SurfaceNetCube () {
            Position = Vector3.zero,
            CornerMask = 0,
            OnSurface = false
        };

        public Vector3 Position;
        public int CornerMask;
        public bool OnSurface;
        public Color32 color;
    }
}