using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {
    public static BoardManager instance;
    public static Board Board {get { return instance.board; }}

    public static IntVector2 WorldToGridPoint(Vector3 pos) {
        return instance.board.WorldToGridPoint(pos);
    }

    public static Vector3 GridToWorldPoint(IntVector2 pos) {
        return instance.board.GridToWorldPoint(pos);
    }

    public static Vector3 GridToWorldPoint(int x, int y) {
        return GridToWorldPoint(new IntVector2(x, y));
    }

    public static IntVector2 ScreenToGridPoint(Vector3 pos) {
        return instance.board.WorldToGridPoint(Camera.main.ScreenToWorldPoint(pos));
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
