
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour {

    public GameObject cursorPrefab;
    public Board.TileType constructType;

    private List<GameObject> cursors;
    private IntVector2 mouseDownPos = IntVector2.zero;

	void Start() {
        cursors = new List<GameObject>();
	}

	void Update() {
        // TODO: Pool and re-use cursors.
	    foreach (GameObject obj in cursors) {
            Destroy(obj);
        }
        cursors.Clear();
        IntVector2 mousePos = InputManager.MouseGridPosition;

        // New drag started.
        if (InputManager.MouseLeftDown) {
            mouseDownPos = mousePos;
        }

        int xMin = Math.Min(mouseDownPos.x, mousePos.x);
        int xMax = Math.Max(mouseDownPos.x, mousePos.x);
        int yMin = Math.Min(mouseDownPos.y, mousePos.y);
        int yMax = Math.Max(mouseDownPos.y, mousePos.y);

        // Mouse is down, update cursor.
        if (InputManager.MouseLeftPressed) {
            for (int x = xMin; x <= xMax; ++x) {
                for (int y = yMin; y <= yMax; ++y) {
                    GameObject obj = Instantiate(cursorPrefab);
                    obj.transform.parent = transform;
                    obj.transform.position = BoardManager.GridToWorldPoint(x, y);
                    cursors.Add(obj);
                }
            }
        }

        // Mouse went up, update tiles.
        if (InputManager.MouseLeftUp) {
            Board board = BoardManager.Board;
            IntVector2 pos = IntVector2.zero;
            for (int x = xMin; x <= xMax; ++x) {
                pos.x = x;
                for (int y = yMin; y <= yMax; ++y) {
                    pos.y = y;
                    board.SetTileType(pos, constructType);
                }
            }
            BoardManager.instance.BoardUpdated();
        }
	}
}
