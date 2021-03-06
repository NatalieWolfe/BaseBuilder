
﻿using UnityEngine; // For Debug.Log
using System;
using System.Collections.Generic;

public class ItemDatabase {
    private Game game;

    // Databases containing the full set of items created in the world.
    private HashSet<LargeItem> largeItems;
    private HashSet<SmallItem> smallItems;

    // All the known custom prototypes. If a type isn't in one of these lists
    // then the base constructor for that type will be used instead.
    private Dictionary<string, LargeItem> largeItemTypes;
    private Dictionary<string, SmallItem> smallItemTypes;

    // Index of all items in the world, keyed on type.
    private Dictionary<string, List<ItemReference>> itemsByType;

    private Action<Events.ItemEvent> onItemEvent;

    public ItemDatabase(Game game) {
        this.game           = game;
        this.largeItems     = new HashSet<LargeItem>();
        this.smallItems     = new HashSet<SmallItem>();
        this.itemsByType    = new Dictionary<string, List<ItemReference>>();
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

    public void SetLargeItemProto(string type, LargeItem proto) {
        largeItemTypes.Add(type, proto);
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
            item = new LargeItem(game, type);
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
            item = new SmallItem(game, type);
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

        List<ItemReference> items = itemsByType[type];
        List<ItemReference> toRemove = new List<ItemReference>();
        foreach (ItemReference itemRef in items) {
            // If the reference is not valid anymore, then we'll add it to our
            // list of things to clean up and move on.
            if (!itemRef.IsAlive) {
                toRemove.Add(itemRef);
                continue;
            }

            // If the referenced item is an instance of the desired type then
            // yield it to the caller.
            if (itemRef.Is<T>()) {
                yield return itemRef.As<T>();
            }
        }

        // Now that we're not iterating over the items list we can remove the
        // dead references.
        foreach (ItemReference itemRef in toRemove) {
            items.Remove(itemRef);
        }

        // Not all types will necessarilly have live references, thus we need to
        // explicitly break here since it is possible no yield statements where
        // hit.
        yield break;
    }

    public void CleanReferences() {
        // TODO: There has _got_ to be a better way to clean up dead references.

        List<ItemReference> toRemove = new List<ItemReference>();
        foreach (List<ItemReference> items in itemsByType.Values) {
            foreach (ItemReference itemRef in items) {
                if (!itemRef.IsAlive) {
                    toRemove.Add(itemRef);
                }
            }

            foreach (ItemReference itemRef in toRemove) {
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
        List<ItemReference> itemRefs;
        if (!itemsByType.TryGetValue(type, out itemRefs)) {
            itemRefs = new List<ItemReference>();
            itemsByType.Add(type, itemRefs);
        }
        itemRefs.Add(new ItemReference(item));
    }
}
