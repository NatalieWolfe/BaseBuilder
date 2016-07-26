
using System;
using System.Collections.Generic;

public class IntBox2D {
    public int top      {get; private set;}
    public int right    {get; private set;}
    public int bottom   {get; private set;}
    public int left     {get; private set;}

    public int height   {get { return top - bottom; }}
    public int width    {get { return right - left; }}

    public IntBox2D(IntVector2 a, IntVector2 b) {
        left    = Math.Min(a.x, b.x);
        right   = Math.Max(a.x, b.x);
        bottom  = Math.Min(a.y, b.y);
        top     = Math.Max(a.y, b.y);
    }

    public bool Contains(IntVector2 pos) {
        return pos.y >= bottom && pos.y <= top && pos.x >= left && pos.x <= right;
    }

    public IEnumerable<IntVector2> Positions() {
        for (int x = left; x <= right; ++x) {
            for (int y = bottom; y <= top; ++y) {
                yield return new IntVector2(x, y);
            }
        }
    }

    public IEnumerable<IntVector2> Border() {
        // Make a clock-wise traversal around the edges of the box, starting
        // across the top.
        for (int x = left; x <= right; ++x) {
            yield return new IntVector2(x, top);
        }

        // Then down the right side.
        for (int y = top - 1; y > bottom; --y) {
            yield return new IntVector2(right, y);
        }

        // Then back along the bottom.
        for (int x = right; x >= left; --x) {
            yield return new IntVector2(x, bottom);
        }

        // And finally up the left side.
        for (int y = bottom + 1; y < top; ++y) {
            yield return new IntVector2(left, y);
        }
    }
}
