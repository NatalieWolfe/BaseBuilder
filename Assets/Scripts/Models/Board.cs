
using UnityEngine; // For Debug.Log
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
        }

        public void RemoveLargeItem() {
            if (HasSmallItems()) {
                Debug.Log(this + " dropping small items on floor.");
            }
            this.largeItem = null;
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
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public Tile GetTile(IntVector2 pos) {
        if (!IsOnBoard(pos)) {
            return null;
        }

        return tiles[pos.x, pos.y];
    }

    public TileType GetTileType(IntVector2 pos) {
        if (!IsOnBoard(pos)) {
            return TileType.Edge;
        }
        return tiles[pos.x, pos.y].type;
    }

    public void SetTileType(IntVector2 pos, TileType type) {
        tiles[pos.x, pos.y].type = type;
    }
}
