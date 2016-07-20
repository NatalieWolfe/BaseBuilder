
using UnityEngine; // For Debug.Log
using System.Collections.Generic;

public class Inventory {
    public int capacity {get; private set;}

    public int UsedCapacity {get { return items.Count; }}
    public bool IsFull {get { return capacity == UsedCapacity; }}

    private List<ItemReference> items = new List<ItemReference>();

    public Inventory(int capacity) {
        this.capacity = capacity;
    }

    public IEnumerable<SmallItem> GetItems() {
        foreach (ItemReference itemRef in GetItemReferences()) {
            yield return itemRef.SmallItem;
        }
    }

    public void AddItem(ItemReference item) {
        if (!item.IsSmallItem) {
            Debug.LogError("Trying to add large item to inventory.");
            return;
        }
        if (!item.IsAlive) {
            Debug.LogError("Trying to add dead item to inventory.");
            return;
        }
        if (IsFull) {
            Debug.LogError("Trying to add item to full inventory.");
            return;
        }

        // TODO: Item stacks.
        items.Add(item);
    }

    public bool HasItemOfType(string type) {
        return GetItemByType(type) != null;
    }

    public SmallItem GetItemByType(string type) {
        foreach (SmallItem item in GetItems()) {
            if (item.type == type) {
                return item;
            }
        }
        return null;
    }

    public bool RemoveItemOfType(string type) {
        ItemReference toRemove = null;
        foreach (ItemReference itemRef in GetItemReferences()) {
            if (itemRef.SmallItem.type == type) {
                toRemove = itemRef;
                break;
            }
        }

        if (toRemove != null) {
            items.Remove(toRemove);
            return true;
        }
        return false;
    }

    private IEnumerable<ItemReference> GetItemReferences() {
        foreach (ItemReference itemRef in items) {
            if (itemRef.IsAlive) {
                yield return itemRef;
            }
        }

    }
}
