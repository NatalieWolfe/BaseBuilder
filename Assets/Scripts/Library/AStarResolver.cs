
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class AStarResolver {
    private static ulong MAX_COST = (ulong)(UInt32.MaxValue / 2);
    private static int MAX_ITERATIONS = 10000;
    private static float DRAW_LIFETIME = 5f;

    private class Node {
        public Node previous = null;
        public IntVector2 position;
        public ulong costToHere = MAX_COST;
        public ulong costToEnd = MAX_COST;
        public bool closed = false;

        public Node(IntVector2 position) {
            this.position = position;
        }
    }

    public AStarResolver() {}

    public Queue<IntVector2> FindPath(IntVector2 start, IntVector2 end) {
        // If we're on top of our target already, just exit.
        if (start == end) {
            return new Queue<IntVector2>();
        }

        // NOTE:
        //  We run the path backwards, from the end to the start for one simple
        //  reason: when building the path we create a queue going from our last
        //  node in the solution to the first. By doing it backwards, we'll
        //  finish our solution at the starting point and thus start our path
        //  from the starting point. Otherwise we'd have to reverse the path
        //  after reconstructing it.

        ulong xDist = (ulong)Math.Abs(start.x - end.x);
        ulong yDist = (ulong)Math.Abs(start.y - end.y);
        Dictionary<IntVector2, Node> encounteredNodes
            = new Dictionary<IntVector2, Node>();

        Node endNode = new Node(end);
        endNode.costToHere = 0;
        PriorityQueue<ulong, Node> openQueue = new PriorityQueue<ulong, Node>();
        openQueue.Enqueue(xDist + yDist, endNode);
        encounteredNodes.Add(end, endNode);

        int iterCounter = 0;
        while (openQueue.Count > 0 && ++iterCounter < MAX_ITERATIONS) {
            // Pop the next element and check if we've reached the target with a
            // viable path.
            Node current = openQueue.Dequeue();
            current.closed = true;

            DebugDraw(current, current.previous, Color.white);
            if (current.position == start && current.costToEnd < MAX_COST) {
                return BuildPathFromNode(current);
            }

            // Not at the target, so check each of this node's neighbors.
            ulong neighborCostToHere = current.costToHere + 1;
            foreach (IntVector2 neighbor in current.position.GetNeighbors()) {
                ulong neighborCostToEnd = neighborCostToHere + DistanceBetween(neighbor, start);
                Node neighborNode;
                if (!encounteredNodes.TryGetValue(neighbor, out neighborNode)) {
                    // Found a new node in the graph, add it to our queue.
                    neighborNode = new Node(neighbor);
                    openQueue.Enqueue(neighborCostToEnd, neighborNode);
                    encounteredNodes.Add(neighbor, neighborNode);
                }
                else if (
                    neighborNode.closed ||
                    neighborNode.costToHere <= neighborCostToHere
                ) {
                    // Skip nodes that we've already closed and skip paths that
                    // are no more efficient than ones we've already found.
                    continue;
                }
                else {
                    // This is a node in the queue and we have a better score
                    // for it. It needs to be requeued.
                    openQueue.Requeue(
                        neighborNode.costToEnd,
                        neighborCostToEnd,
                        neighborNode
                    );
                }

                DebugDraw(current, neighborNode, Color.blue);

                // We have found the best path to this node so far. Update its
                // costs and node chain.
                neighborNode.previous = current;
                neighborNode.costToHere = neighborCostToHere;
                neighborNode.costToEnd = neighborCostToEnd;
            }
        }

        return null;
    }

    private ulong DistanceBetween(IntVector2 a, IntVector2 b) {
        return (ulong)(Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y));
    }

    private Queue<IntVector2> BuildPathFromNode(Node node) {
        Queue<IntVector2> path = new Queue<IntVector2>();
        Node step = node.previous;
        DebugDraw(node, step, Color.green);
        while (step != null) {
            DebugDraw(step, step.previous, Color.green);
            path.Enqueue(step.position);
            step = step.previous;
        }
        return path;
    }

    private void DebugDraw(Node start, Node end, Color color) {
        if (start != null && end != null) {
            DebugDraw(start.position, end.position, color);
        }
    }

    private void DebugDraw(IntVector2 start, IntVector2 end, Color color) {
        if (Debug.isDebugBuild) {
            Debug.DrawLine(
                BoardManager.GridToWorldPoint(start),
                BoardManager.GridToWorldPoint(end),
                color,
                DRAW_LIFETIME,
                false
            );
        }
    }
}

// while openSet is not empty
//     current := the node in openSet having the lowest fScore[] value
//     if current = goal
//         return reconstruct_path(cameFrom, current)
//
//     openSet.Remove(current)
//     closedSet.Add(current)
//     for each neighbor of current
//         if neighbor in closedSet
//             continue		// Ignore the neighbor which is already evaluated.
//         // The distance from start to a neighbor
//         tentative_gScore := gScore[current] + dist_between(current, neighbor)
//         if neighbor not in openSet	// Discover a new node
//             openSet.Add(neighbor)
//         else if tentative_gScore >= gScore[neighbor]
//             continue		// This is not a better path.
//
//         // This path is the best until now. Record it!
//         cameFrom[neighbor] := current
//         gScore[neighbor] := tentative_gScore
//         fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)
