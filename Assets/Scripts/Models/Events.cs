
public static class Events {
    public enum EventType {
        TileEvent
    }

    public enum TileEventType {
        LargeItemAdded,
        LargeItemRemoved,
        SmallItemAdded,
        SmallItemRemoved,
        TypeChanged
    };

    public class Event {
        public EventType eventType;
    }

    public class TileEvent : Event {
        public TileEventType tileEventType;
        public Board.Tile tile;

        public LargeItem largeItem;
        public SmallItem smallItem;

        public TileEvent(TileEventType type, Board.Tile tile, LargeItem item) {
            this.eventType = EventType.TileEvent;
            this.tileEventType = type;
            this.tile = tile;
            this.largeItem = item;
            this.smallItem = null;
        }

        public TileEvent(TileEventType type, Board.Tile tile, SmallItem item) {
            this.eventType = EventType.TileEvent;
            this.tileEventType = type;
            this.tile = tile;
            this.largeItem = null;
            this.smallItem = item;
        }

        public TileEvent(TileEventType type, Board.Tile tile) {
            this.eventType = EventType.TileEvent;
            this.tileEventType = type;
            this.tile = tile;
            this.largeItem = null;
            this.smallItem = null;
        }
    }
}
