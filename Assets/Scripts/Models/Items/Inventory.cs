
using UnityEngine; // For Debug.Log
using System.Collections.Generic;

public class Inventory {
    public int capacity {get; private set;}

    public int UsedCapacity {get { return items.Count; }}
    public bool IsFull {get { return capacity == UsedCapacity; }}

    private List<ItemRef> items = new List<ItemRef>();

    public Inventory(int capacity) {
        this.capacity = capacity;
    }

    public IEnumerable<SmallItem> GetItems() {
        foreach (ItemRef item in items) {
            if (item.IsActive) {
                yield return item.SmallItem;
            }
        }
    }

    public void AddItem(ItemRef item) {
        if (!item.IsSmallItem) {
            Debug.LogError("Trying to add large item to inventory.");
            return;
        }
        if (!item.IsActive) {
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
        SmallItem toRemove = null;
        foreach (SmallItem item in GetItems()) {
            if (item.type == type) {
                toRemove = item;
                break;
            }
        }

        if (toRemove != null) {
            items.Remove(toRemove);
            return true;
        }
        return false;
    }
}
