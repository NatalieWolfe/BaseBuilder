
public class SmallItem {
    public IntVector2 position;
    public string type {get; protected set;}

    public SmallItem(string type) {
        this.type = type;
    }

    protected SmallItem(SmallItem other) {
        this.type = other.type;
    }

    public virtual SmallItem Clone() {
        return new SmallItem(this);
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
