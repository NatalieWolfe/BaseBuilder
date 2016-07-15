
﻿using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Worker {
    public float speed = 4; // TODO: Make speed configurable.
    public IntVector2 position = IntVector2.zero;
    public IntVector2 movingIntoPosition = IntVector2.zero;
    public float moveCompletion = 0;
    public bool isWorking = true;

    // TODO: Emit JobAssigned event when job is set.
    public JobQueue.Job job;

    // TODO: Add worker inventory.
    private WorkersUnion union;
    private Queue<IntVector2> path;

    public Worker(WorkersUnion union) {
        this.union = union;
    }

    public void Update(float deltaTime) {
        if (!isWorking) {
            // TODO: Add non-work jobs like eating, sleeping, or recreation.
            return;
        }

        if (path != null && path.Count > 0) {
            UpdateMovement(deltaTime);
        }
        else if (job == null) {
            // If we don't have a job, ask our union to find us one.
            job = union.FindJob(this);
        }
        else if (!position.IsNextTo(job.position)) {
            BuildPathNextTo(job.position);

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
        else {
            UpdateWork(deltaTime);
        }
    }

    public IEnumerable<IntVector2> GetPath() {
        return path;
    }

    private void BuildPathNextTo(IntVector2 position) {
        // If we're standing on the target, find the neighbor with the lowest
        // movement cost and move into it.
        if (this.position == position) {
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
        }
        else {
            BuildPathTo(position);
            RemoveLastPathItem();
        }
    }

    private void BuildPathTo(IntVector2 position) {
        // We aren't standing in the target, so find a path to it.
        AStarResolver resolver = new AStarResolver(BoardManager.Board);
        path = resolver.FindPath(this.position, position);
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

    private void UpdateMovement(float deltaTime) {
        if (position == movingIntoPosition) {
            if (path == null || path.Count == 0) {
                Debug.LogError("Updating movement, but no path to follow.");
                return;
            }

            movingIntoPosition = path.Dequeue();
            moveCompletion = 0f;
        }

        moveCompletion += speed * deltaTime;
        if (moveCompletion >= 1f) {
            position = movingIntoPosition;
            moveCompletion = 0f;
        }
    }

    private void UpdateWork(float deltaTime) {
        // TODO: Redo the way job updating works.
        if (job.Update()) {
            job = null;
        }
    }
}
