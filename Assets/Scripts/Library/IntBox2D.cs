
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

    public IEnumerable<IntVector2> Positions() {
        for (int x = left; x <= right; ++x) {
            for (int y = bottom; y <= top; ++y) {
                yield return new IntVector2(x, y);
            }
        }
    }
}
