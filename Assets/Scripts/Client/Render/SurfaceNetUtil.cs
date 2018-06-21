using System;
using System.Collections.Generic;
using Material;
using UnityEngine;
using Util;
using World;
using Material = Material.Material;

namespace Client.Render {
    public static class SurfaceNetUtil {
        private static readonly int[] CubeEdgesTable = GenerateCubeEdgesTable ();

        private static readonly int[] IntersectionTable = GenerateIntersectionTable (CubeEdgesTable);

        private static int[] _buffer = new int[4096];

        /**
         *  y         z
	     *  ^        /     
	     *  |
	     *    6----7
	     *   /|   /|
	     *  4----5 |
	     *  | 2--|-3
	     *  |/   |/
	     *  0----1   --> x
	     * 
         */
        private static readonly Vector3[] VoxelCornerOffsets = new Vector3[8] {
            new Vector3 (0, 0, 0), // 0
            new Vector3 (1, 0, 0), // 1
            new Vector3 (0, 1, 0), // 2
            new Vector3 (1, 1, 0), // 3
            new Vector3 (0, 0, 1), // 4
            new Vector3 (1, 0, 1), // 5
            new Vector3 (0, 1, 1), // 6
            new Vector3 (1, 1, 1) // 7
        };
        
        private static readonly VoxelPos[] VoxelOffsets = {
            new VoxelPos (0, 0, 0), // 0
            new VoxelPos (1, 0, 0), // 1
            new VoxelPos (0, 1, 0), // 2
            new VoxelPos (1, 1, 0), // 3
            new VoxelPos (0, 0, 1), // 4
            new VoxelPos (1, 0, 1), // 5
            new VoxelPos (0, 1, 1), // 6
            new VoxelPos (1, 1, 1) // 7
        };

        public static Vector3 GetVoxelCornerOffsetVec (int index) {
            return VoxelCornerOffsets[index];
        }

        public static VoxelPos GetVoxelOffset (int index) {
            return VoxelOffsets[index];
        }

        /**
         * returns uint[24]
         */
        private static int[] GenerateCubeEdgesTable () {
            var cubeEdges = new int[24];
            var k = 0;
            for (var i = 0; i < 8; ++i) {
                for (var j = 1; j <= 4; j = j << 1) {
                    var p = i ^ j;
                    if (i > p) continue;
                    cubeEdges[k++] = i;
                    cubeEdges[k++] = p;
                }
            }

            return cubeEdges;
        }

        /**
         * cubeEdges is not mutated
         * returns uint[256]
         */
        private static int[] GenerateIntersectionTable (IList<int> cubeEdges) {
            var edgeTable = new int[256];
            for (var i = 0; i < 256; ++i) {
                var em = 0;
                for (var j = 0; j < 24; j += 2) {
                    var a = Convert.ToBoolean (i & (1 << cubeEdges[j]));
                    var b = Convert.ToBoolean (i & (1 << cubeEdges[j + 1]));
                    em |= a != b ? (1 << (j >> 1)) : 0;
                }

                edgeTable[i] = em;
            }

            return edgeTable;
        }

        /**
         * sample: float[8]
         * 
         */
        public static SurfaceNetCube CalculateSurfaceNetCube (IList<float> sample, Voxel voxel) {
            // create corner mask
            var cornerMask = 0;
            for (var i = 0; i < 8; i++) {
                cornerMask |= ((sample[i] > 0) ? (1 << i) : 0);
            }

            // interior cube
            if (cornerMask == 0 || cornerMask == 0xff) {
                return SurfaceNetCube.Zero;
            }

            var edgeMask = IntersectionTable[cornerMask];
            var edgeCrossings = 0;
            var vertPos = Vector3.zero;

            for (var i = 0; i < 12; ++i) {
                //Use edge mask to check if it is crossed
                if (!((edgeMask & (1 << i)) > 0)) {
                    continue;
                }

                //If it did, increment number of edge crossings
                edgeCrossings++;

                //Now find the point of intersection
                var e0 = CubeEdgesTable[i << 1];
                var e1 = CubeEdgesTable[(i << 1) + 1];
                var g0 = sample[e0];
                var g1 = sample[e1];
                var t = (-g0) / (g1 - g0);

                vertPos += Vector3.Lerp (VoxelCornerOffsets[e0], VoxelCornerOffsets[e1], t);
            }

            vertPos /= edgeCrossings;
            return new SurfaceNetCube () {
                Position = vertPos,
                CornerMask = cornerMask,
                OnSurface = true,
                color = MaterialRegistry.Instance.GetMaterial (voxel.MaterialId).MatColor
            };
        }

        private static void AddQuad (int a, int b, int c, int d, IReadOnlyList<Vector3> source, ICollection<int> target,
                                     IReadOnlyList<Color32> colors, ICollection<Vector3> duplicatedSource,
                                     ICollection<Color32> duplicatedColors) {
            var currentCount = duplicatedSource.Count;
            var vecA = source[a];
            var vecB = source[b];
            var vecC = source[c];
            var vecD = source[d];
            var color = colors[a];
            foreach (var i in new[] {a, b, c, d}) {
                if (colors[i].a >= 122) {
                    color = colors[i];
                }
            }

            if ((vecA - vecC).magnitude >= (vecB - vecD).magnitude) {
                duplicatedSource.Add (vecA);
                target.Add (currentCount++);
                duplicatedSource.Add (vecB);
                target.Add (currentCount++);
                duplicatedSource.Add (vecD);
                target.Add (currentCount++);

                duplicatedSource.Add (vecD);
                target.Add (currentCount++);
                duplicatedSource.Add (vecB);
                target.Add (currentCount++);
                duplicatedSource.Add (vecC);
                target.Add (currentCount);
            } else {
                duplicatedSource.Add (vecA);
                target.Add (currentCount++);
                duplicatedSource.Add (vecB);
                target.Add (currentCount++);
                duplicatedSource.Add (vecC);
                target.Add (currentCount++);

                duplicatedSource.Add (vecA);
                target.Add (currentCount++);
                duplicatedSource.Add (vecC);
                target.Add (currentCount++);
                duplicatedSource.Add (vecD);
                target.Add (currentCount);
            }

            for (var i = 0; i < 6; i++) {
                duplicatedColors.Add (color);
            }
        }

        /**
         * returns a mesh!
         */
        public static MeshBuf ComputeMesh (SurfaceNetCube[,,] voxels) {
            var vertices = new List<Vector3> ();
            var colors = new List<Color32> ();
            var triangles = new List<int> ();
            var duplicatedVertices = new List<Vector3> ();
            var duplicatedColors = new List<Color32> ();
            // get the width, height, and depth of the sample space for our nested for loops
            var width = voxels.GetUpperBound (0) + 1;
            var height = voxels.GetUpperBound (1) + 1;
            var depth = voxels.GetUpperBound (2) + 1;

            var n = 0;
            var pos = new int[3];
            var r = new int[] {1, width + 1, (width + 1) * (height + 1)};
//            var grid = new float[8];
            var bufferNumber = 1;

            // resize the buffer if it's not big enough
            if (r[2] * 2 > _buffer.Length)
                _buffer = new int[r[2] * 2];

            for (pos[2] = 0; pos[2] < depth - 1; pos[2]++, bufferNumber ^= 1, r[2] = -r[2]) {
                var bufferIndex = 1 + (width + 1) * (1 + bufferNumber * (height + 1));

                for (pos[1] = 0; pos[1] < height - 1; pos[1]++, n++, bufferIndex += 2) {
                    for (pos[0] = 0; pos[0] < width - 1; pos[0]++, n++, bufferIndex++) {
                        // get the corner mask we calculated earlier
//                        var voxel = voxels[pos[0], pos[1], pos[2]];
                        var voxel = voxels[pos[0], pos[1], pos[2]];

                        // Early Termination Check
                        if (!voxel.OnSurface) {
                            continue;
                        }

                        var cornerMask = voxel.CornerMask;
                        // get edge mask
                        var edgeMask = IntersectionTable[cornerMask];

                        var vertex = voxel.Position;

                        //Add Vertex to Buffer, Store Pointer to Vertex Index
                        _buffer[bufferIndex] = vertices.Count;
                        vertices.Add (vertex);
                        colors.Add (voxel.color);
//                        colors.Add(MaterialRegistry.Instance.GetMaterial(Builtin.Struix).MatColor);

                        //Add Faces (Loop Over 3 Base Components)
                        for (var i = 0; i < 3; i++) {
                            //First 3 Entries Indicate Crossings on Edge
                            if (!Convert.ToBoolean (edgeMask & (1 << i))) {
                                continue;
                            }

                            //i - Axes, iu, iv - Ortho Axes
                            var iu = (i + 1) % 3;
                            var iv = (i + 2) % 3;

                            //Skip if on Boundary
                            if (pos[iu] == 0 || pos[iv] == 0)
                                continue;

                            //Otherwise, Look Up Adjacent Edges in Buffer
                            var du = r[iu];
                            var dv = r[iv];

                            //Flip Orientation Depending on Corner Sign
                            if (Convert.ToBoolean (cornerMask & 1)) {
                                AddQuad (_buffer[bufferIndex], _buffer[bufferIndex - du],
                                    _buffer[bufferIndex - dv - du],
                                    _buffer[bufferIndex - dv], vertices, triangles, colors, duplicatedVertices,
                                    duplicatedColors);
                            } else {
                                AddQuad (_buffer[bufferIndex], _buffer[bufferIndex - dv],
                                    _buffer[bufferIndex - dv - du],
                                    _buffer[bufferIndex - du], vertices, triangles, colors, duplicatedVertices,
                                    duplicatedColors);
                            }
                        }
                    }
                }
            }

            return new MeshBuf(duplicatedVertices.ToArray (), triangles.ToArray (), duplicatedColors.ToArray ());
        }
    }
}