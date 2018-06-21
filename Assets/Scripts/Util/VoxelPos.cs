using System;

namespace Util {
    public struct VoxelPos : IComparable<VoxelPos> {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public VoxelPos (int x, int y, int z) {
            X = x;
            Y = y;
            Z = z;
        }

        public VoxelPos SetX (int x) {
            return new VoxelPos (x, Y, Z);
        }

        public VoxelPos SetY (int y) {
            return new VoxelPos (X, y, Z);
        }

        public VoxelPos SetZ (int z) {
            return new VoxelPos (X, Y, z);
        }

        public VoxelPos Add (int x, int y, int z) {
            return new VoxelPos (X + x, Y + y, Z + z);
        }

        public VoxelPos Multiply (int f) {
            return new VoxelPos (X * f, Y * f, Z * f);
        }

        public TilePos ToTilePos () {
            return new TilePos (X >> 4, Y >> 4, Z >> 4);
        }

        public bool Equals (VoxelPos other) {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals (object obj) {
            if (ReferenceEquals (null, obj)) return false;
            return obj is VoxelPos && Equals ((VoxelPos) obj);
        }

        public override int GetHashCode () {
            unchecked {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }

        public int CompareTo (VoxelPos other) {
            var xComparison = X.CompareTo (other.X);
            if (xComparison != 0) return xComparison;
            var yComparison = Y.CompareTo (other.Y);
            if (yComparison != 0) return yComparison;
            return Z.CompareTo (other.Z);
        }

        public override string ToString () {
            return $"{nameof (X)}: {X}, {nameof (Y)}: {Y}, {nameof (Z)}: {Z}";
        }
    }
}