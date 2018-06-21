using System;
using UnityEngine;

namespace Util {
    public struct TilePos : IComparable<TilePos> {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        
        public TilePos(int x, int y, int z) {
            X = x;
            Y = y;
            Z = z;
        }

        public TilePos SetX(int x) {
            return new TilePos(x, Y, Z);
        }

        public TilePos SetY(int y) {
            return new TilePos(X, y, Z);
        }

        public TilePos SetZ(int z) {
            return new TilePos(X, Y, z);
        }

        public TilePos Add(int x, int y, int z) {
            return new TilePos(X + x, Y + y, Z + z);
        }

        public Vector2 ToVector2() {
            return new Vector2(X, Z);
        }

        public Vector3 ToVector3() {
            return new Vector3(X, Y, Z);
        }

        public VoxelPos ToVoxelPos() {
            return new VoxelPos(X << 4, Y << 4, Z << 4);
        }

        public bool Equals(TilePos other) {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TilePos && Equals((TilePos) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }

        public int CompareTo(TilePos other) {
            var xComparison = X.CompareTo(other.X);
            if (xComparison != 0) return xComparison;
            var yComparison = Y.CompareTo(other.Y);
            if (yComparison != 0) return yComparison;
            return Z.CompareTo(other.Z);
        }

        public override string ToString() {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
        }
    }
}