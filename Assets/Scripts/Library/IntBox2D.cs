
using System;
using System.Collections.Generic;

public class IntBox2D {
    public int top {get; private set;}
    public int right {get; private set;}
    public int bottom {get; private set;}
    public int left {get; private set;}

    public IntBox2D(IntVector2 a, IntVector2 b) {
        left    = Math.Min(a.x, b.x);
        right   = Math.Max(a.x, b.x);
        bottom  = Math.Min(a.y, b.y);
        top     = Math.Max(a.y, b.y);
    }

    public IEnumerable<IntVector2> Positions() {
        IntVector2 pos = IntVector2.zero;
        for (pos.x = left; pos.x <= right; ++pos.x) {
            for (pos.y = bottom; pos.y <= top; ++pos.y) {
                yield return pos;
            }
        }
    }
}
