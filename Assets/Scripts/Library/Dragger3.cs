
﻿using UnityEngine;

public class Dragger3 {
    public KeyCode key {get; private set;}
    public Vector3 downPosition {get; private set;}
    public Vector3 currentPosition {get; private set;}
    public Vector3 delta {get; private set;}
    public bool down {get; private set;}
    public bool up {get; private set;}
    public bool pressed {get; private set;}
    public bool moved {get; private set;}
    public bool dragging {get; private set;}

    public Dragger3(KeyCode key) {
        this.key = key;
    }

    private Vector3 lastMousePosition;

    public void Update() {
        Vector3 newPos = InputManager.MouseWorldPosition;
        delta = Camera.main.ScreenToWorldPoint(lastMousePosition) - newPos;
        down = Input.GetKeyDown(key);
        pressed = Input.GetKey(key);
        up = Input.GetKeyUp(key);
        moved = newPos != currentPosition;
        dragging = pressed && moved;

        if (down) {
            downPosition = currentPosition;
        }

        lastMousePosition = Input.mousePosition;
        currentPosition = newPos;
    }


}
