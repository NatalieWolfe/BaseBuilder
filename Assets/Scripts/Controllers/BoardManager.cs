using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {
    public static BoardManager instance;
    public static Board Board {get { return instance.board; }}

    public Board board;

	// Use this for initialization
	void Start () {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        // TODO: Make board generation dynamic.
        board = new Board(5, 5);
	}

	// Update is called once per frame
	void Update () {

	}
}
