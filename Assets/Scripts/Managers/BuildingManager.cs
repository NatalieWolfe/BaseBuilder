
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour {

    public GameObject cursorPrefab;
    public Board.TileType constructType; // TODO: Replace with "Tool" class.

    private PrefabPool cursorPool;
    private List<GameObject> cursors;
    private IntDragger2 cursorDrag;

    private JobQueue.Job jobProto; // TODO: Remove after adding "Tool" class.

	void Start() {
        cursors = new List<GameObject>();
        cursorPool = new PrefabPool(cursorPrefab);
        cursorPool.SetParent(transform);
        cursorDrag = new IntDragger2(KeyCode.Mouse0);

        // TODO: Remove this after adding "Tool" class.
        jobProto = UnionManager.Jobs.MakeProtoJob();
	}

	void Update() {
        UpdateCursorDrag();
	}

    private void UpdateCursorDrag() {
        cursorDrag.Update();

        // Mouse is down, update cursor.
        if (cursorDrag.dragging) {
            // Release our currently displayed cursors.
            ReleaseCursors();

            foreach (IntVector2 pos in jobProto.ValidPositions(cursorDrag.box)) {
                cursors.Add(cursorPool.Acquire(BoardManager.GridToWorldPoint(pos)));
            }
        }

        // Mouse went up, update tiles.
        if (cursorDrag.up) {
            // No need to display the cursors anymore, release them.
            ReleaseCursors();

            foreach (IntVector2 pos in jobProto.ValidPositions(cursorDrag.box)) {
                UnionManager.Jobs.AddJob(jobProto, pos);
            }
        }
    }

    private void ReleaseCursors() {
        foreach (GameObject cursor in cursors) {
            cursorPool.Release(cursor);
        }
        cursors.Clear();
    }
}
