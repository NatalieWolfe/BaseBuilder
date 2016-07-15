
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
        game.board.GetTile(0, 1).SetLargeItem(game.itemDB.CreateLargeItem("Chest"));

    }

    void Update () {
        game.Update(Time.deltaTime);
    }
}
