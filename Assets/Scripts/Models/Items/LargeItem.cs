
using System;

public class LargeItem {
    // TODO: Per-item surface capacities.
    public int SurfaceCapacity {get { return 0; }}

    public IntVector2 position;
    public string type {get; protected set;}

    public LargeItem(string type) {
        this.type = type;
    }

    protected LargeItem(LargeItem other) {
        this.type = other.type;
    }

    public virtual LargeItem Clone() {
        return new LargeItem(this);
    }

    public ulong GetHinderance() {
        return Int32.MaxValue; // TODO: Per-item hinderances.
    }

    public void Update(float deltaTime) {
        DoUpdate(deltaTime);
        // TODO: Add item components and update them here.
    }

    public virtual void DoUpdate(float deltaTime) {
        // Default update is to do nothing.
    }

    protected void SendItemEvent(Events.ItemEventType type) {
        SendItemEvent(new Events.ItemEvent(type, this));
    }

    protected void SendItemEvent(Events.ItemEventType type, object data) {
        Events.ItemEvent e = new Events.ItemEvent(type, this);
        e.data = data;
        SendItemEvent(e);
    }

    protected void SendItemEvent(Events.ItemEvent e) {
        ItemDatabase.instance.OnItemEvent(e);
    }
}
