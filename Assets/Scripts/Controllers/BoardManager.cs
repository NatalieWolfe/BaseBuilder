using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {
    public static BoardManager instance;
    public static Board Board {get { return instance.board; }}

    public Board board;

    public static IntVector2 WorldToGridPoint(Vector3 pos) {
        return instance.board.WorldToGridPoint(pos);
    }

    public static Vector3 GridToWorldPoint(IntVector2 pos) {
        return instance.board.GridToWorldPoint(pos);
    }

    public static IntVector2 ScreenToGridPoint(Vector3 pos) {
        return instance.board.WorldToGridPoint(Camera.main.ScreenToWorldPoint(pos));
    }

	void Start () {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        // TODO: Make board generation dynamic.
        board = new Board(5, 5);
	}
}
