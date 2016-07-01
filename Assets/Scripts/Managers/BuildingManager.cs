
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

        int xMin = Math.Min(mouseDownPos.x, mousePos.x);
        int xMax = Math.Max(mouseDownPos.x, mousePos.x);
        int yMin = Math.Min(mouseDownPos.y, mousePos.y);
        int yMax = Math.Max(mouseDownPos.y, mousePos.y);

        // Mouse is down, update cursor.
        if (InputManager.MouseLeftPressed && mouseMoved) {
            // Release our currently displayed cursors.
            ReleaseCursors();

            IntVector2 pos = IntVector2.zero;
            for (pos.x = xMin; pos.x <= xMax; ++pos.x) {
                for (pos.y = yMin; pos.y <= yMax; ++pos.y) {
                    cursors.Add(
                        cursorPool.Acquire(BoardManager.GridToWorldPoint(pos))
                    );
                }
            }
        }

        // Mouse went up, update tiles.
        if (InputManager.MouseLeftUp) {
            // No need to display the cursors anymore, release them.
            ReleaseCursors();

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

    private void ReleaseCursors() {
        foreach (GameObject cursor in cursors) {
            cursorPool.Release(cursor);
        }
        cursors.Clear();
    }
}
