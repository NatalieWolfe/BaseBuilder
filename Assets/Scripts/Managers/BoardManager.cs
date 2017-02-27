
﻿using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
    public static Board Board {get { return GameManager.Game.board; }}

    public static IntVector2 WorldToGridPoint(Vector3 pos) {
        // Grid origin (0, 0) is world origin (0, 0, 0). Thus, moving from
        // world positions to grid positions is as simple as taking the
        // floor of the world pos divided by the tile size.
        return new IntVector2(
            Mathf.FloorToInt(pos.x + 0.5f),
            Mathf.FloorToInt(pos.y + 0.5f)
        );
    }

    public static Vector3 GridToWorldPoint(IntVector2 pos) {
        return GridToWorldPoint(pos.x, pos.y);
    }

    public static Vector3 GridToWorldPoint(int x, int y) {
        // Like world-to-grid, we can rely on the matching origins to
        // simplify converting between grid and world coordinates.
        return new Vector3((float)x, (float)y, 0f);
    }

    public static IntVector2 ScreenToGridPoint(Vector3 pos) {
        return WorldToGridPoint(Camera.main.ScreenToWorldPoint(pos));
    }

    public static IEnumerable<Board.Tile> SelectionToTiles(
        IEnumerable<IntVector2> selection
    ) {
        foreach (IntVector2 position in selection) {
            yield return BoardManager.Board.GetTile(position);
        }
    }
}
