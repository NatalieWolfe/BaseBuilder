
using UnityEngine; // For Debug.Log
using System;
using System.Collections.Generic;

[System.Serializable]
public class Board {
    public enum TileType { Edge = 0, Grass };

    public class Tile {
        public Board board {get; private set;}
        public TileType type {get; set;} // TODO A type setter function?
        public IntVector2 position {get; private set;}

        private LargeItem largeItem;
        private List<SmallItem> smallItems;

        public Tile(Board board, TileType type, IntVector2 position) {
            this.board = board;
            this.type = type;
            this.position = position;
            this.largeItem = null;
            this.smallItems = new List<SmallItem>();
        }

        public bool HasLargeItem() {
            return largeItem != null;
        }

        public LargeItem GetLargeItem() {
            return largeItem;
        }

        public void SetLargeItem(LargeItem item) {
            if (HasLargeItem()) {
                Debug.LogError(this + " already has a large item.");
                return;
            }
            largeItem = item;
            SendTileEvent(Events.TileEventType.LargeItemAdded, item);
        }

        public void RemoveLargeItem() {
            if (HasSmallItems()) {
                Debug.Log(this + " dropping small items on floor.");
            }
            LargeItem item = this.largeItem;
            this.largeItem = null;
            SendTileEvent(Events.TileEventType.LargeItemRemoved, item);
        }

        public bool HasSmallItems() {
            return smallItems.Count > 0;
        }

        public IEnumerable<SmallItem> GetSmallItems() {
            return smallItems;
        }

        public void AddSmallItem(SmallItem item) {
            if (HasLargeItem() && smallItems.Count >= largeItem.SurfaceCapacity){
                Debug.LogError(this + " can not take any more small items.");
                return;
            }
            smallItems.Add(item);
            SendTileEvent(Events.TileEventType.SmallItemAdded, item);
        }

        public ulong GetHinderance() {
            ulong hinderance = type == TileType.Edge ? AStarResolver.MAX_COST : 1ul; // TODO: Add per-tile-type hinderances.
            if (HasLargeItem()) {
                hinderance += largeItem.GetHinderance();
            }
            hinderance += (ulong)(((float)smallItems.Count) * board.smallItemHinderance);
            return hinderance;
        }

        public IEnumerable<Tile> GetNeighbors() {
            foreach (IntVector2 pos in position.GetNeighbors()) {
                if (board.IsOnBoard(pos)) {
                    yield return board.GetTile(pos);
                }
            }
        }

        public override string ToString() {
            return "Tile" + position;
        }

        private void SendTileEvent(Events.TileEventType eventType) {
            SendTileEvent(new Events.TileEvent(eventType, this));
        }

        private void SendTileEvent(Events.TileEventType eventType, LargeItem item) {
            SendTileEvent(new Events.TileEvent(eventType, this, item));
        }

        private void SendTileEvent(Events.TileEventType eventType, SmallItem item) {
            SendTileEvent(new Events.TileEvent(eventType, this, item));
        }

        private void SendTileEvent(Events.TileEvent e) {
            board.OnTileEvent(e);
        }
    }

    // TODO: Make boards infinite in dimension. Simplex or Perlin?
    // TODO: Use a 2-dimensional sparse array for storing tile contents.
    // TODO: Decide if sparse array is actually correct approach. Maybe flatten
    //       large structures into a single element instead? Keep items in a
    //       simple 2D array and just load a few large blocks at a time?
    public float smallItemHinderance;
    public int width;
    public int height;
    public Tile[,] tiles;

    private Action<Events.TileEvent> onTileEvent;
    private Dictionary<LargeItem, Tile> largeItemMap = new Dictionary<LargeItem, Tile>();

    public Board(int _width, int _height) {
        width = _width;
        height = _height;
        tiles = new Tile[width, height];

        // TODO: Dynamically generate tiles here.
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                tiles[x, y] = new Tile(this, TileType.Grass, new IntVector2(x, y));
            }
        }
    }

    public bool IsOnBoard(IntVector2 pos) {
        return IsOnBoard(pos.x, pos.y);
    }

    public bool IsOnBoard(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public Tile GetTile(IntVector2 pos) {
        return GetTile(pos.x, pos.y);
    }

    public Tile GetTile(int x, int y) {
        if (!IsOnBoard(x, y)) {
            return null;
        }

        return tiles[x, y];
    }

    public TileType GetTileType(IntVector2 pos) {
        if (!IsOnBoard(pos)) {
            return TileType.Edge;
        }
        return tiles[pos.x, pos.y].type;
    }

    public void Update(float deltaTime) {
        foreach (LargeItem item in largeItemMap.Keys) {
            item.Update(deltaTime);
        }
    }

    /// Sends out an event that a tile has been changed in some way.
    ///
    /// @param e - An event object containing the information about the changes.
    public void OnTileEvent(Events.TileEvent e) {
        // TODO: Update pathfinding grid when large items are added or removed.

        // If a large item was just added to the world, make sure it is in our
        // map of items. Likewise, if one was just removed, make sure it is _not_
        // in our map.
        if (e.tileEventType == Events.TileEventType.LargeItemAdded) {
            largeItemMap.Add(e.largeItem, e.tile);
        }
        else if (e.tileEventType == Events.TileEventType.LargeItemRemoved) {
            largeItemMap.Remove(e.largeItem);
        }

        // Finally, update any listeners.
        if (onTileEvent != null) {
            onTileEvent(e);
        }
    }

    public void RegisterOnTileEvent(Action<Events.TileEvent> action) {
        onTileEvent += action;
    }

    public void UnregisterOnTileEvent(Action<Events.TileEvent> action) {
        onTileEvent -= action;
    }
}
