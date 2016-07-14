using UnityEngine;
using System.Collections;

namespace Item {

public class Chest : LargeItem {
    public Chest(Game game): base(game, "Chest") {
    }

    private Chest(Chest other): base(other as LargeItem) {
    }

    public override LargeItem Clone() {
        return (new Chest(this)) as LargeItem;
    }
}

}
