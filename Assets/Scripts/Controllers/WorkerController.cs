
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class WorkerController : MonoBehaviour {
    public float speed;

    private IntVector2 gridPosition;
    private JobQueue.Job job;
    private Queue<IntVector2> path;

    // TODO: Add worker inventory.

    void Start() {
        job = null;
        path = new Queue<IntVector2>();
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
            if (path.Count == 0) {
                BuildPathTo(job.position);
            }
            Move();
        }
        else {
            WorkOnJob();
        }
	}

    private void BuildPathTo(IntVector2 position) {
        if (gridPosition == position) {
            path.Enqueue(new IntVector2(gridPosition.x - 1, gridPosition.y));
            return;
        }

        IntVector2 next = gridPosition;
        while (next.x != position.x) {
            path.Enqueue(next);
            next = new IntVector2(next.x + Clamp(position.x - next.x, -1, 1), next.y);
        }

        while (next.y != position.y) {
            path.Enqueue(next);
            next = new IntVector2(next.x, next.y + Clamp(position.y - next.y, -1, 1));
        }
    }

    private int Clamp(int a, int min, int max) {
        return Math.Min(max, Math.Max(min, a));
    }

    private void Move() {
        IntVector2 next = path.Peek();
        Vector3 worldNext = BoardManager.GridToWorldPoint(next);
        worldNext.z = transform.position.z;
        if (transform.position == worldNext) {
            path.Dequeue();
            if (path.Count == 0) {
                return;
            }
            next = path.Peek();
        }
        MoveTo(next);
    }

    private void MoveTo(IntVector2 position) {
        float dist = speed * Time.deltaTime;
        Vector3 movement = new Vector3(
            Mathf.Clamp(position.x - transform.position.x, -dist, dist),
            Mathf.Clamp(position.y - transform.position.y, -dist, dist),
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
