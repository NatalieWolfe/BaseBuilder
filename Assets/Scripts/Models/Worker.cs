
﻿using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Worker {
    public float speed = 4; // TODO: Make speed configurable.
    public IntVector2 position = IntVector2.zero;
    public IntVector2 movingIntoPosition = IntVector2.zero;
    public float moveCompletion = 0;

    // TODO: Emit JobAssigned event when job is set.
    public JobQueue.Job job;

    private enum State {
        Idle,
        Collecting,
        Working,
    };

    private State state;
    private WorkersUnion union;
    private Queue<IntVector2> path;
    private Inventory inventory;
    private LargeItem targetContainer;

    public Worker(WorkersUnion union) {
        this.state = State.Working;
        this.union = union;
        this.inventory = new Inventory(5); // TODO: Make inventory size configurable.
        this.targetContainer = null;
    }

    public void Update(float deltaTime) {
        if (state == State.Idle) {
            // TODO: Add non-work jobs like eating, sleeping, or recreation.
            return;
        }

        if (path != null && path.Count > 0) {
            // If we have a path to follow, move along it.
            UpdateMovement(deltaTime);
        }
        else if (job == null) {
            // If we don't have a job, ask our union to find us one.
            job = union.FindJob(this);
            if (job != null) {
                state = HasRequiredItemsForJob() ? State.Working : State.Collecting;
            }
        }
        else if (state == State.Working) {
            if (!position.IsNextTo(job.position)) {
                // If we are working and not next to the job, build a path to
                // the job.
                BuildPathNextTo(job.position);

                if (path == null) {
                    // TODO:    Determine if the failure to path was because the
                    //          worker is boxed in or if the job is boxed in.
                    Debug.LogError("Failed to build path to " + job.position);
                    Debug.LogError("Dropping job on the ground and idling.");
                    job = null;
                    state = State.Idle;
                    return;
                }
            }
            else {
                // If we are working and are next to the job, do some work.
                UpdateWork(deltaTime);
            }
        }
        else if (state == State.Collecting) {
            if (HasRequiredItemsForJob()) {
                // If we have all the items we need, change into the working state.
                state = State.Working;
                targetContainer = null;
                path = null;
            }
            else if (targetContainer == null) {
                // If we are collecting items and we do not have a container to
                // get the item from, find one.
                targetContainer = FindContainerWithJobItems();

                if (targetContainer == null) {
                    // TODO:    Queue a job to create the unfound item type(s)
                    //          and requeue our current job.
                    Debug.LogError("Failed to find containers with required items.");
                    Debug.LogError("Dropping job on the ground.");
                    job = null;
                    return;
                }
            }
            else if (!position.IsNextTo(targetContainer.position)) {
                // If we are collecting items and we know where to get the items
                // but are not next to it, build a path to the container.
                BuildPathNextTo(targetContainer.position);

                if (path == null) {
                    // TODO:    Limit container finding to ones we can actually
                    //          get to.
                    Debug.LogError("Failed to find path to " + targetContainer.position);
                    Debug.LogError("Dropping job on the ground.");
                    job = null;
                    targetContainer = null;
                    return;
                }
            }
            else {
                // If we are collecting items and are next to the item source,
                // pick up the items we need.
                UpdateCollection(deltaTime);
            }
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

    private LargeItem FindContainerWithJobItems() {
        // TODO: Find item by inventory component.
        ItemDatabase itemDB = union.game.itemDB;
        foreach (LargeItem chest in itemDB.GetItemsByType<LargeItem>("Chest")) {
            foreach (string itemType in GetMissingItemTypes()) {
                if ((chest as Item.Chest).inventory.HasItemOfType(itemType)) {
                    return chest;
                }
            }
        }
        return null;
    }

    private bool HasRequiredItemsForJob() {
        foreach (string type in GetMissingItemTypes()) {
            return false;
        }
        return true;
    }

    private IEnumerable<string> GetMissingItemTypes() {
        foreach (string itemType in job.requirements) {
            if (!inventory.HasItemOfType(itemType)) {
                yield return itemType;
            }
        }
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
            // TODO: Remove items for the job as progress goes forward.
            foreach (string itemType in job.requirements) {
                inventory.RemoveItemOfType(itemType);
            }
            job = null;
        }
    }

    private void UpdateCollection(float deltaTime) {
        // TODO: Pull inventory component from container.
        Item.Chest chest = targetContainer as Item.Chest;
        foreach (string itemType in GetMissingItemTypes()) {
            SmallItem item = chest.inventory.GetItemByType(itemType);
            if (item != null) {
                // TODO: Remove the item from the target container too.
                // chest.inventory.RemoveItemOfType(itemType);

                // TODO: Figure out better way of handling items than by reference.
                inventory.AddItem(new ItemReference(item));
            }
        }

        // We're done with this container.
        targetContainer = null;
    }
}
