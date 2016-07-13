
﻿using UnityEngine; // For Debug.Log
using System;
using System.Collections.Generic;

public class ItemDatabase {
    public static ItemDatabase instance {
        get {
            if (_instance == null) {
                _instance = new ItemDatabase();
            }
            return _instance;
        }
    }
    private static ItemDatabase _instance;

    // Databases containing the full set of items created in the world.
    private HashSet<LargeItem> largeItems;
    private HashSet<SmallItem> smallItems;

    // All the known custom prototypes. If a type isn't in one of these lists
    // then the base constructor for that type will be used instead.
    private Dictionary<string, LargeItem> largeItemTypes;
    private Dictionary<string, SmallItem> smallItemTypes;

    // Index of all items in the world, keyed on type.
    private Dictionary<string, List<WeakReference>> itemsByType;

    private Action<Events.ItemEvent> onItemEvent;

    private ItemDatabase() {
        this.largeItems     = new HashSet<LargeItem>();
        this.smallItems     = new HashSet<SmallItem>();
        this.itemsByType    = new Dictionary<string, List<WeakReference>>();
        this.largeItemTypes = new Dictionary<string, LargeItem>();
        this.smallItemTypes = new Dictionary<string, SmallItem>();
    }

    public void Update(float deltaTime) {
        foreach (LargeItem item in largeItems) {
            item.Update(deltaTime);
        }

        foreach (SmallItem item in smallItems) {
            item.Update(deltaTime);
        }
    }

    public LargeItem CreateLargeItem(string type) {
        // Look up if we have a custom prototype for this item type. If we do,
        // clone that prototype for our new item.
        LargeItem item;
        LargeItem proto;
        if (largeItemTypes.TryGetValue(type, out proto)) {
            item = proto.Clone();
        }
        else {
            // This type doesn't have it's own prototype, so we'll just use the
            // generic base type instead.
            item = new LargeItem(type);
        }

        // Store the item in our database for future lookups and add it to our
        // indexes.
        largeItems.Add(item);
        UpdateIndexes(type, item);

        // And done. Return the item!
        return item;
    }

    public SmallItem CreateSmallItem(string type) {
        // Look up if we have a custom prototype for this item type. If we do,
        // clone that prototype for our new item.
        SmallItem item;
        SmallItem proto;
        if (smallItemTypes.TryGetValue(type, out proto)) {
            item = proto.Clone();
        }
        else {
            // This type doesn't have it's own prototype, so we'll just use the
            // generic base type instead.
            item = new SmallItem(type);
        }

        // Store the item in our database for future lookups and add it to our
        // indexes.
        smallItems.Add(item);
        UpdateIndexes(type, item);

        // And done. Return the item!
        return item;
    }

    public void Destroy(LargeItem item) {
        // Only need to remove the item from the main database. The indexes will
        // clean themselves on next access.
        largeItems.Remove(item);
    }

    public void Destroy(SmallItem item) {
        // Only need to remove the item from the main database. The indexes will
        // clean themselves on next access.
        smallItems.Remove(item);
    }

    public IEnumerable<T> GetItemsByType<T>(string type) where T: class {
        if (!itemsByType.ContainsKey(type)) {
            yield break;
        }

        List<WeakReference> items = itemsByType[type];
        List<WeakReference> toRemove = new List<WeakReference>();
        foreach (WeakReference itemRef in items) {
            // If the reference is not valid anymore, then we'll add it to our
            // list of things to clean up and move on.
            if (!itemRef.IsAlive) {
                toRemove.Add(itemRef);
                continue;
            }

            // If the referenced item is an instance of the desired type then
            // yield it to the caller.
            if (itemRef.Target is T) {
                yield return itemRef.Target as T;
            }
        }

        // Now that we're not iterating over the items list we can remove the
        // dead references.
        foreach (WeakReference itemRef in toRemove) {
            items.Remove(itemRef);
        }

        // Not all types will necessarilly have live references, thus we need to
        // explicitly break here since it is possible no yield statements where
        // hit.
        yield break;
    }

    public void CleanReferences() {
        // TODO: There has _got_ to be a better way to clean up dead references.

        List<WeakReference> toRemove = new List<WeakReference>();
        foreach (List<WeakReference> items in itemsByType.Values) {
            foreach (WeakReference itemRef in items) {
                if (!itemRef.IsAlive) {
                    toRemove.Add(itemRef);
                }
            }

            foreach (WeakReference itemRef in toRemove) {
                items.Remove(itemRef);
            }
            toRemove.Clear();
        }
    }

    public void OnItemEvent(Events.ItemEvent e) {
        if (onItemEvent != null) {
            onItemEvent(e);
        }
    }

    public void RegisterOnItemEvent(Action<Events.ItemEvent> action) {
        onItemEvent += action;
    }

    public void UnregisterOnItemEvent(Action<Events.ItemEvent> action) {
        onItemEvent -= action;
    }

    private void UpdateIndexes(string type, object item) {
        List<WeakReference> itemRefs;
        if (!itemsByType.TryGetValue(type, out itemRefs)) {
            itemRefs = new List<WeakReference>();
            itemsByType.Add(type, itemRefs);
        }
        itemRefs.Add(new WeakReference(item));
    }
}
