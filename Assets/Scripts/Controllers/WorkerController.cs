
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class WorkerController : MonoBehaviour {
    public float speed;

    private IntVector2 gridPosition;
    private JobQueue.Job job;
    private Queue<IntVector2> path;
    private bool isWorking;

    // TODO: Add worker inventory.

    void Start() {
        job = null;
        path = new Queue<IntVector2>();
        isWorking = true;
    }

	void Update () {
        // TODO: Implement A* pathfinding.
        // TODO: Access UnionManager for tasks.
        // TODO: Check worker inventory against job reqs.

        // Update our current grid position.
        gridPosition = BoardManager.WorldToGridPoint(transform.position);

        if (!isWorking) {
            // TODO: Add non-work jobs like eating, sleeping, or recreation.
            return;
        }

        // If we don't currently have a job, look for one.
        if (job == null) {
            if (UnionManager.Jobs.Count == 0) {
                // TODO:    Move to some central meeting area or frequently used
                //          stockpile to be ready for a new job.
                return;
            }
            job = UnionManager.Jobs.ClaimJob();
        }

        if ((path == null || path.Count == 0) && !gridPosition.IsNextTo(job.position)) {
            BuildPathTo(job.position);

            // The path constructed above will go all the way to the tile
            // provided, but for jobs we want to go _next_ to the tile, so
            // we must remove the last item in the queue.
            RemoveLastPathItem();

            if (path == null) {
                // TODO:    Determine if the failure to path was because the
                //          worker is boxed in or if the job is boxed in.
                Debug.LogError("Failed to build path to " + job.position);
                Debug.LogError("Dropping job on the ground.");
                job = null;
                isWorking = false;
                return;
            }
        }

        if (path.Count > 0) {
            Move();
        }
        else {
            WorkOnJob();
        }
	}

    private void BuildPathTo(IntVector2 position) {
        // If we're standing on the target, find the neighbor with the lowest
        // movement cost and move into it.
        if (position == gridPosition) {
            Board.Tile tile = BoardManager.Board.GetTile(position);
            Board.Tile moveTo = null;
            foreach (Board.Tile neighbor in tile.GetNeighbors()) {
                if (
                    moveTo == null ||
                    moveTo.GetHinderance() > neighbor.GetHinderance()
                ) {
                    moveTo = neighbor;
                }
            }

            if (moveTo.GetHinderance() < AStarResolver.MAX_COST) {
                path = new Queue<IntVector2>();
                path.Enqueue(moveTo.position);
            }
            return;
        }

        // We aren't standing in the target, so find a path to it.
        AStarResolver resolver = new AStarResolver(BoardManager.Board);
        path = resolver.FindPath(gridPosition, position);
    }

    private int Clamp(int a, int min, int max) {
        return Math.Min(max, Math.Max(min, a));
    }

    private void Move() {
        if (path.Count == 0) {
            return;
        }

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
        if (job != null && job.Update()) {
            job = null;
        }
    }

    private void RemoveLastPathItem() {
        if (path == null || path.Count < 2) {
            return;
        }

        IntVector2 first = path.Peek();
        IntVector2 current = path.Dequeue();
        do {
            path.Enqueue(current);
            current = path.Dequeue();
        } while (path.Peek() != first);
    }
}
