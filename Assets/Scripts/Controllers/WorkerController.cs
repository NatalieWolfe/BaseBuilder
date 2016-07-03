
﻿using UnityEngine;
using System;
using System.Collections;

public class WorkerController : MonoBehaviour {
    public float speed;

    private IntVector2 gridPosition;
    private JobQueue.Job job;

    // TODO: Add worker inventory.

    void Start() {
        job = null;
    }

	void Update () {
        // TODO: Implement A* pathfinding.
        // TODO: Access WorkerManager for tasks.
        // TODO: Check worker inventory against job reqs.

        // Update our current grid position.
        gridPosition = BoardManager.WorldToGridPoint(transform.position);

        // If we don't currently have a job, look for one.
        if (job == null) {
            if (WorkerManager.Jobs.Count == 0) {
                // TODO:    Move to some central meeting area or frequently used
                //          stockpile to be ready for a new job.
                return;
            }
            job = WorkerManager.Jobs.ClaimJob();
        }

        if (!gridPosition.IsNextTo(job.position)) {
            MoveNextTo(job.position);
        }
        else {
            WorkOnJob();
        }
	}

    private void MoveNextTo(IntVector2 position) {
        float dist = speed * Time.deltaTime;
        Vector3 targPosition = BoardManager.GridToWorldPoint(position - new IntVector2(1, 0));
        Vector3 movement = new Vector3(
            Mathf.Clamp(targPosition.x - transform.position.x, -dist, dist),
            Mathf.Clamp(targPosition.y - transform.position.y, -dist, dist),
            0
        );

        transform.position += movement;
    }

    private void WorkOnJob() {
        if (job.Update()) {
            job = null;
            BoardManager.instance.BoardUpdated();
        }
    }
}
