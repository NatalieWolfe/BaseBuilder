﻿using UnityEngine;
using System.Collections;

namespace Item {

public class Wall : LargeItem {
    public Wall(): base("Wall") {
    }

    private Wall(Wall other): base(other as LargeItem) {
    }

    public override LargeItem Clone() {
        return (new Wall(this)) as LargeItem;
    }
}

}
