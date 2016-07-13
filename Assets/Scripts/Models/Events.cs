
public static class Events {
    public enum EventType {
        ItemEvent,
        TileEvent
    }

    public class Event {
        public EventType eventType {get; private set;}
        public object data;

        protected Event(EventType type) {
            eventType = type;
        }
    }

    // ---------------------------------------------------------------------- //

    public enum ItemEventType {
        Created,
        Damaged,
        Destroyed,

        Custom
    };

    public class ItemEvent : Event {
        public ItemEventType itemEventType;

        public LargeItem largeItem;
        public SmallItem smallItem;

        public ItemEvent(ItemEventType type, LargeItem item):
            base(EventType.ItemEvent)
        {
            this.itemEventType = type;
            this.largeItem = item;
            this.smallItem = null;
        }


        public ItemEvent(ItemEventType type, SmallItem item):
            base(EventType.ItemEvent)
        {
            this.itemEventType = type;
            this.largeItem = null;
            this.smallItem = item;
        }
    }

    // ---------------------------------------------------------------------- //

    public enum TileEventType {
        LargeItemAdded,
        LargeItemRemoved,
        SmallItemAdded,
        SmallItemRemoved,
        TypeChanged,

        Custom
    };

    public class TileEvent : Event {
        public TileEventType tileEventType;
        public Board.Tile tile;

        public LargeItem largeItem;
        public SmallItem smallItem;

        public TileEvent(TileEventType type, Board.Tile tile, LargeItem item):
            base(EventType.TileEvent)
        {
            this.tileEventType = type;
            this.tile = tile;
            this.largeItem = item;
            this.smallItem = null;
        }

        public TileEvent(TileEventType type, Board.Tile tile, SmallItem item):
            base(EventType.TileEvent)
        {
            this.tileEventType = type;
            this.tile = tile;
            this.largeItem = null;
            this.smallItem = item;
        }

        public TileEvent(TileEventType type, Board.Tile tile):
            base(EventType.TileEvent)
        {
            this.tileEventType = type;
            this.tile = tile;
            this.largeItem = null;
            this.smallItem = null;
        }
    }

    // ---------------------------------------------------------------------- //
}
