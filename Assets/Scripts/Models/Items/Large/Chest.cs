using UnityEngine;
using System.Collections;

namespace Item {

public class Chest : LargeItem {
    public Inventory inventory;

    public Chest(Game game): base(game, "Chest") {
        this.inventory = new Inventory(10); // Make capacity dynamic.
    }

    private Chest(Chest other): base(other as LargeItem) {
        this.inventory = new Inventory(other.inventory.capacity);
    }

    public override LargeItem Clone() {
        return (new Chest(this)) as LargeItem;
    }
}

}
