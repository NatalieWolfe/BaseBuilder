
using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class IntVector2 {
    public static readonly IntVector2 zero = new IntVector2(0, 0);

    public int x;
    public int y;

    public IntVector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public override string ToString() {
        return String.Format("({0}, {1})", x, y);
    }
}
