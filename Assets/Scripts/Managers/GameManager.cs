
﻿using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance {get; private set;}
    public static Game Game {get {return instance.game; }}

    public Game game {get; private set;}

    void Start () {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        // TODO: Make board generation dynamic.
        game = new Game(50, 50);

        // TODO: Load the item database from configurations.
        game.itemDB.SetLargeItemProto("Chest", new Item.Chest(game));
        game.itemDB.SetLargeItemProto("Wall", new Item.Wall(game));

        // TODO: Make layout of board dynamic.
        // FIXME: Remove all this crap, just for debugging.
        Item.Chest chest = game.itemDB.CreateLargeItem("Chest") as Item.Chest;
        game.board.GetTile(0, 1).SetLargeItem(chest);

        SmallItem item = game.itemDB.CreateSmallItem("Wood");
        chest.inventory.AddItem(new ItemReference(item));
    }

    void Update () {
        game.Update(Time.deltaTime);
    }
}
