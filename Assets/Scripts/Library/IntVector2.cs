
using System;
using System.Collections.Generic;

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

    public bool IsNextTo(IntVector2 pos) {
        return (Math.Abs(x - pos.x) + Math.Abs(y - pos.y)) == 1;
    }

    public bool IsBetween(IntVector2 a, IntVector2 b) {
        return (new IntBox2D(a, b)).Contains(this);
    }

    public bool IsAbove(IntVector2 pos) {
        return y > pos.y;
    }

    public bool IsBelow(IntVector2 pos) {
        return y < pos.y;
    }

    public bool IsRightOf(IntVector2 pos) {
        return x > pos.x;
    }

    public bool IsLeftOf(IntVector2 pos) {
        return x < pos.x;
    }

    public IEnumerable<IntVector2> GetNeighbors() {
        yield return new IntVector2(x, y + 1);  // North
        yield return new IntVector2(x + 1, y);  // East
        yield return new IntVector2(x, y - 1);  // South
        yield return new IntVector2(x - 1, y);  // West
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

    public static IntVector2 operator-(IntVector2 a, IntVector2 b) {
        return new IntVector2(
            a.x - b.x,
            a.y - b.y
        );
    }
}
