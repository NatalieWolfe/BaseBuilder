
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour {

    public GameObject cursorPrefab;
    public Board.TileType constructType;

    private PrefabPool cursorPool;
    private List<GameObject> cursors;
    private IntVector2 mouseDownPos = IntVector2.zero;
    private IntVector2 mouseLastPos = IntVector2.zero;

	void Start() {
        cursors = new List<GameObject>();
        cursorPool = new PrefabPool(cursorPrefab);
        cursorPool.SetParent(transform);
	}

	void Update() {
        IntVector2 mousePos = InputManager.MouseGridPosition;
        CursorDrag(mousePos);
        mouseLastPos = mousePos;
	}

    private void CursorDrag(IntVector2 mousePos) {
        bool mouseMoved = mousePos != mouseLastPos;

        // New drag started.
        if (InputManager.MouseLeftDown) {
            mouseDownPos = mousePos;
        }

        Board board = BoardManager.Board;
        IntBox2D cursorBox = new IntBox2D(mousePos, mouseDownPos);

        // Mouse is down, update cursor.
        if (InputManager.MouseLeftPressed && mouseMoved) {
            // Release our currently displayed cursors.
            ReleaseCursors();

            foreach (IntVector2 pos in cursorBox.Positions()) {
                cursors.Add(cursorPool.Acquire(board.GridToWorldPoint(pos)));
            }
        }

        // Mouse went up, update tiles.
        if (InputManager.MouseLeftUp) {
            // No need to display the cursors anymore, release them.
            ReleaseCursors();

            foreach (IntVector2 pos in cursorBox.Positions()) {
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
