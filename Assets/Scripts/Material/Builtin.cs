using UnityEngine;

namespace Material {
    public static class Builtin {
        public static readonly int Struix =
            MaterialRegistry.Instance.RegisterMaterial (new Material (new Color32 (59, 72, 94, 255), 1.0f));

        public static readonly int Pasto =
            MaterialRegistry.Instance.RegisterMaterial (new Material (new Color32 (132, 180, 155, 255), 0.3f));

        public static readonly int Nilum =
            MaterialRegistry.Instance.RegisterMaterial (new Material (new Color32 (255, 255, 255, 0), 0f));
    }
}