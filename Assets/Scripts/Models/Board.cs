using UnityEngine;
using System.Collections;

public class Board {
    public enum TileType { Edge = 0, Grass };

    // TODO: Make boards infinite in dimension. Simplex or Perlin?
    // TODO: Use a 2-dimensional sparse array for storing tile contents.
    // TODO: Decide if sparse array is actually correct approach. Maybe flatten
    //       large structures into a single element instead? Keep items in a
    //       simple 2D array and just load a few large blocks at a time?
    public int width;
    public int height;
    public TileType[,] tiles;

    public Board(int _width, int _height) {
        width = _width;
        height = _height;
        tiles = new TileType[width, height];

        // TODO: Dynamically generate tiles here.
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                tiles[x, y] = TileType.Grass;
            }
        }
    }

    public TileType GetTileType(IntVector2 pos) {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height) {
            return TileType.Edge;
        }
        return tiles[pos.x, pos.y];
    }

    public IntVector2 WorldToGridPoint(Vector3 pos) {
        // Grid origin (0, 0) is world origin (0, 0, 0). Thus, moving from
        // world positions to grid positions is as simple as taking the
        // floor of the world pos divided by the tile size.
        return new IntVector2(
            Mathf.FloorToInt(pos.x),
            Mathf.FloorToInt(pos.y)
        );
    }

    public Vector3 GridToWorldPoint(IntVector2 pos) {
        // Like world-to-grid, we can rely on the matching origins to
        // simplify converting between grid and world coordinates.
        return new Vector3(
            ((float)pos.x),
            ((float)pos.y),
            0f
        );
    }
}
