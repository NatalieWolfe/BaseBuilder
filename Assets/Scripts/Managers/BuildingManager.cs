
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour {

    public GameObject cursorPrefab;
    public Board.TileType constructType;

    private PrefabPool cursorPool;
    private List<GameObject> cursors;
    private IntDragger2 cursorDrag;

	void Start() {
        cursors = new List<GameObject>();
        cursorPool = new PrefabPool(cursorPrefab);
        cursorPool.SetParent(transform);
        cursorDrag = new IntDragger2(KeyCode.Mouse0);
	}

	void Update() {
        UpdateCursorDrag();
	}

    private void UpdateCursorDrag() {
        cursorDrag.Update();
        Board board = BoardManager.Board;

        // Mouse is down, update cursor.
        if (cursorDrag.dragging) {
            // Release our currently displayed cursors.
            ReleaseCursors();

            foreach (IntVector2 pos in cursorDrag.box.Positions()) {
                cursors.Add(cursorPool.Acquire(board.GridToWorldPoint(pos)));
            }
        }

        // Mouse went up, update tiles.
        if (cursorDrag.up) {
            // No need to display the cursors anymore, release them.
            ReleaseCursors();

            foreach (IntVector2 pos in cursorDrag.box.Positions()) {
                board.SetTileType(pos, constructType);
            }

            BoardManager.instance.BoardUpdated();
        }
    }

    private void ReleaseCursors() {
        foreach (GameObject cursor in cursors) {
            cursorPool.Release(cursor);
        }
        cursors.Clear();
    }
}
