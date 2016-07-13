using UnityEngine;
using System.Collections;

namespace Item {

public class Chest : LargeItem {
    public Chest(): base("Chest") {
    }

    private Chest(Chest other): base(other as LargeItem) {
    }

    public override LargeItem Clone() {
        return (new Chest(this)) as LargeItem;
    }
}

}
