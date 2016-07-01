
using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class IntVector2 {
    public static IntVector2 zero {get { return new IntVector2(0, 0); }}
    public static IntVector2 one {get { return new IntVector2(1, 1); }}

    public int x;
    public int y;

    public IntVector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public override string ToString() {
        return String.Format("({0}, {1})", x, y);
    }

    public override bool Equals(System.Object other) {
        if (other == null) {
            return false;
        }

        IntVector2 b = other as IntVector2;
        if (((System.Object)other) == null) {
            return false;
        }

        return x == b.x && y == b.y;
    }

    public override int GetHashCode() {
        return x ^ y;
    }

    public static bool operator==(IntVector2 a, IntVector2 b) {
        if (System.Object.ReferenceEquals(a, b)) {
            return true;
        }
        if ((object)a == null || (object)b == null) {
            return false;
        }

        return a.x == b.x && a.y == b.y;
    }

    public static bool operator!=(IntVector2 a, IntVector2 b) {
        return !(a == b);
    }
}
