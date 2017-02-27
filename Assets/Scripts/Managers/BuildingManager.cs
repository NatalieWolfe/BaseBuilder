
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour {
    public static BuildingManager instance {get; private set;}


    public GameObject cursorPrefab;
    public Tool tool;

    private PrefabPool cursorPool;
    private List<GameObject> cursors;
    private IntDragger2 cursorDrag;

    void Start() {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        cursors = new List<GameObject>();
        cursorPool = new PrefabPool(cursorPrefab);
        cursorPool.SetParent(transform);
        cursorDrag = new IntDragger2(KeyCode.Mouse0);

        // FIXME: This is only for debugging.
        LargeItem wall = GameManager.Game.itemDB.CreateLargeItem("Wall");
        BuildingManager.instance.tool = new ConstructTool(wall);
    }

    void Update() {
        // If there is no tool, simply clear our display, otherwise update the
        // cursor.
        if (tool == null) {
            ReleaseCursors();
        }
        else {
            UpdateCursorDrag();
        }
    }

    private void UpdateCursorDrag() {
        cursorDrag.Update();

        // Mouse is down, update cursor.
        if (cursorDrag.dragging) {
            // Release our currently displayed cursors.
            ReleaseCursors();

            foreach (IntVector2 pos in tool.GetSelectedPositions(cursorDrag)) {
                cursors.Add(cursorPool.Acquire(BoardManager.GridToWorldPoint(pos)));
            }
        }

        // Mouse went up, update tiles.
        if (cursorDrag.up) {
            // No need to display the cursors anymore, release them.
            ReleaseCursors();

            tool.DoAction(
                BoardManager.SelectionToTiles(tool.GetSelectedPositions(cursorDrag))
            );
        }
    }

    private void ReleaseCursors() {
        foreach (GameObject cursor in cursors) {
            cursorPool.Release(cursor);
        }
        cursors.Clear();
    }
}
