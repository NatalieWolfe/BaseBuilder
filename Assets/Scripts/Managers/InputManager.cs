
﻿using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    public static float Horizontal = 0.0f;
    public static float Vertical = 0.0f;
    public static bool Action = false;
    public static Vector3 MouseWorldPosition;
    public static IntVector2 MouseGridPosition;

    public static bool MouseLeftDown;
    public static bool MouseLeftPressed;
    public static bool MouseLeftUp;

	void Update () {
        // Update our mouse position.
        MouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseWorldPosition.x += 0.5f;
        MouseWorldPosition.y += 0.5f;
        MouseGridPosition = BoardManager.WorldToGridPoint(MouseWorldPosition);

        // Determine the state of our inputs.
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        Action = Input.GetButtonDown("Action");

        // Update mouse button state.
        MouseLeftDown = Input.GetMouseButtonDown(0);
        MouseLeftPressed = Input.GetMouseButton(0);
        MouseLeftUp = Input.GetMouseButtonUp(0);
	}
}
