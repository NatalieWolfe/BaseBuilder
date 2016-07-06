using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {
    public static BoardManager instance;
    public static Board Board {get { return instance.board; }}

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

    public Board board;

    private bool boardUpdated;

	void Start() {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        // TODO: Make board generation dynamic.
        board = new Board(50, 50);
        boardUpdated = false;
	}

    void Update() {
        if (boardUpdated) {
            boardUpdated = false;
            TileManager.instance.Redraw();
        }
    }

    public void BoardUpdated() {
        boardUpdated = true;
    }
}
