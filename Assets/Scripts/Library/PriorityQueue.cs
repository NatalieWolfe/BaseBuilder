
using System;
using System.Collections.Generic;

public class PriorityQueue<P, T> where P: IComparable<P> {
    private class Node {
        public T val;
        public P priority;
        public Node left = null;
        public Node right = null;

        public Node(P priority, T val) {
            this.val = val;
            this.priority = priority;
        }
    }

    public int Count {get; private set;}

    private Node root;

    public PriorityQueue() {
        Count = 0;
        root = null;
    }

    public void Enqueue(P priority, T val) {
        ++Count;
        EnqueueNode(new Node(priority, val));
    }

    public T Dequeue() {
        Node parent = null;
        Node next = root;
        --Count;

        while (next.left != null) {
            parent = next;
            next = next.left;
        }

        if (parent == null) {
            // If we're dequeueing the root node, update root.
            root = next.right;
        }
        else {
            // Otherwise update the parent.
            parent.left = next.right;
        }

        return next.val;
    }

    public void Requeue(P oldPriority, P newPriority, T val) {
        Node requeueParent = FindParentNode(oldPriority, val);
        Node toRequeue = null;
        if (requeueParent == null) {
            toRequeue = root;
        }
        else if (oldPriority.CompareTo(requeueParent.priority) < 0) {
            toRequeue = requeueParent.left;
        }
        else {
            toRequeue = requeueParent.right;
        }

        RemoveNode(requeueParent, toRequeue);

        // Finally, requeue the node.
        toRequeue.left = null;
        toRequeue.right = null;
        toRequeue.priority = newPriority;
        EnqueueNode(toRequeue);
    }

    public IEnumerable<T> GetContents() {
        Stack<Node> stack = new Stack<Node>();
        stack.Push(root);

        while (stack.Count > 0) {
            Node next = stack.Pop();
            if (next == null) {
                if (stack.Count > 0) {
                    next = stack.Pop();
                    yield return next.val;
                    stack.Push(next.right);
                }
            }
            else {
                stack.Push(next.left);
            }
        }
    }

    private Node FindParentNode(P priority) {
        Node prev = null;
        Node next = root;
        while (next != null) {
            prev = next;
            if (priority.CompareTo(next.priority) < 0) {
                next = next.left;
            }
            else {
                next = next.right;
            }
        }
        return prev;
    }

    private Node FindParentNode(P priority, T val) {
        Node prev = null;
        Node next = root;
        while (next != null && !Object.ReferenceEquals(next.val, val)) {
            prev = next;
            if (priority.CompareTo(next.priority) < 0) {
                next = next.left;
            }
            else {
                next = next.right;
            }
        }
        return prev;
    }

    private void EnqueueNode(Node node) {
        Node parent = FindParentNode(node.priority);
        if (parent == null) {
            root = node;
        }
        else if (node.priority.CompareTo(parent.priority) < 0) {
            parent.left = node;
        }
        else {
            parent.right = node;
        }
    }

    private void RemoveNode(Node parent, Node toRemove) {
        Node toShift = null;
        if (toRemove.left == null || toRemove.right == null) {
            // If either child is null, simply shift the not-null child up to
            // take the removed node's place.
            toShift = toRemove.left != null ? toRemove.right : toRemove.left;
        }
        else {
            // We have two children.
            // In this case, we will need to find the right-most left child of the
            // to-be-removed node.
            Node toShiftParent = toRemove;
            toShift = toRemove.left;
            while (toShift.right != null) {
                toShiftParent = toShift;
                toShift = toShift.right;
            }

            // We have the right-most left child which we _know_ only has one child.
            // Now we can remove this new node from the tree and replace it where
            // we are instead.
            RemoveNode(toShiftParent, toShift);

            // Now we have a free node to replace ourselves with.
            toShift.left = toRemove.left;
            toShift.right = toRemove.right;
        }

        if (parent == null) {
            root = toShift;
        }
        else if (toRemove.priority.CompareTo(parent.priority) < 0) {
            parent.left = toShift;
        }
        else {
            parent.right = toShift;
        }
    }
}
