
using UnityEngine; // For Debug.Log
using System;

public class ItemReference {
    public LargeItem LargeItem {
        get { return item.Target as LargeItem; }
    }

    public SmallItem SmallItem {
        get { return item.Target as SmallItem; }
    }

    public bool IsAlive {
        get { return item.IsAlive; }
    }

    public bool IsLargeItem {
        get { return item.Target is LargeItem; }
    }

    public bool IsSmallItem {
        get { return item.Target is SmallItem; }
    }

    private WeakReference item;

    public ItemReference(object item) {
        if (item is LargeItem || item is SmallItem) {
            this.item = new WeakReference(item);
        }
        else {
            Debug.LogError("Creating item reference for non-item object.");
        }
    }

    public bool Is<T>() where T: class {
        return item.Target is T;
    }

    public T As<T>() where T: class {
        return item.Target as T;
    }
}
