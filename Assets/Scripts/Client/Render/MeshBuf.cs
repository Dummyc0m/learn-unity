using UnityEngine;

namespace Client.Render {
    public class MeshBuf {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;
        public readonly Color32[] Colors;
        
        public MeshBuf (Vector3[] vertices, int[] triangles, Color32[] colors) {
            Vertices = vertices;
            Triangles = triangles;
            Colors = colors;
        }

        public Mesh ToMesh () {
            var mesh = new Mesh {
                vertices = Vertices,
                triangles = Triangles,
                colors32 = Colors
            };
            mesh.RecalculateNormals ();
            return mesh;
        }
    }
}