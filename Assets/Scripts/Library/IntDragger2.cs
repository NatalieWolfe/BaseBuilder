
﻿using UnityEngine;

public class IntDragger2 {
    public KeyCode key {get; private set;}
    public IntVector2 downPosition {get; private set;}
    public IntVector2 currentPosition {get; private set;}
    public bool down {get; private set;}
    public bool up {get; private set;}
    public bool pressed {get; private set;}
    public bool moved {get; private set;}
    public bool dragging {get; private set;}

    public IntBox2D box {
        get { return new IntBox2D(downPosition, currentPosition); }
    }

    public IntDragger2(KeyCode key) {
        this.key = key;
    }

    public void Update() {
        IntVector2 newPos = InputManager.MouseGridPosition;
        down = Input.GetKeyDown(key);
        pressed = Input.GetKey(key);
        up = Input.GetKeyUp(key);
        moved = newPos != currentPosition;
        dragging = pressed && moved;

        if (down) {
            downPosition = newPos;
        }

        currentPosition = newPos;
    }
}
