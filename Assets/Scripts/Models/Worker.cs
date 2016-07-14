
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

    public JobQueue.Job job;

    // TODO: Add worker inventory.
    private Queue<IntVector2> path;

    public void Update(float deltaTime) {
        if (!isWorking) {
            // TODO: Add non-work jobs like eating, sleeping, or recreation.
            return;
        }

        if (path != null && path.Count > 0) {
            UpdateMovement(deltaTime);
        }
        else if (job == null) {
            if (WorkersUnion.Jobs.Count == 0) {
                // TODO:    Move to some central meeting area or frequently used
                //          stockpile to be ready for a new job.
                // TODO: Take jobs away from other workers.
                return;
            }

            // TODO: Grab the job based on our position.
            // TODO: Grab the job based on our skill set.
            job = WorkersUnion.Jobs.ClaimJob();
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
            // UpdateWork();
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

    private void UpdateMovement(float deltaTime) {

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


    // private int Clamp(int a, int min, int max) {
    //     return Math.Min(max, Math.Max(min, a));
    // }
    //
    // private void Move() {
    //     if (path.Count == 0) {
    //         return;
    //     }
    //
    //     IntVector2 next = path.Peek();
    //     Vector3 worldNext = BoardManager.GridToWorldPoint(next);
    //     worldNext.z = transform.position.z;
    //     if (transform.position == worldNext) {
    //         path.Dequeue();
    //         if (path.Count == 0) {
    //             return;
    //         }
    //         next = path.Peek();
    //     }
    //     MoveTo(next);
    // }
    //
    // private void MoveTo(IntVector2 position) {
    //     float dist = speed * Time.deltaTime;
    //     Vector3 movement = new Vector3(
    //         Mathf.Clamp(position.x - transform.position.x, -dist, dist),
    //         Mathf.Clamp(position.y - transform.position.y, -dist, dist),
    //         0
    //     );
    //
    //     transform.position += movement;
    // }
    //
    // private void WorkOnJob() {
    //     if (job != null && job.Update()) {
    //         job = null;
    //     }
    // }
}
