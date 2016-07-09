
using System;

public class LargeItem {
    // TODO: Per-item surface capacities.
    public int SurfaceCapacity {get { return 0; }}

    public IntVector2 position;
    public string name {get; protected set;}

    public ulong GetHinderance() {
        return Int32.MaxValue; // TODO: Per-item hinderances.
    }

    public virtual void Update(float deltaTime) {
        // Default update is to do nothing.
        // TODO: Add item components and update them here.
    }
}
