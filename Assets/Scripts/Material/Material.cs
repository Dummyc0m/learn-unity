using UnityEngine;

namespace Material {
    public class Material {
        public Color32 MatColor { get; }
        public float Toughness { get; }

        public Material (Color32 matColor, float toughness) {
            MatColor = matColor;
            Toughness = toughness;
        }
    }
}