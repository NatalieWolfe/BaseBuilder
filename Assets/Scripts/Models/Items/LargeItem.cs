
using System;

public class LargeItem {
    // TODO: Per-item surface capacities.
    public int SurfaceCapacity {get { return 0; }}

    public IntVector2 position;

    public ulong GetHinderance() {
        return Int32.MaxValue; // TODO: Per-item hinderances.
    }
}
