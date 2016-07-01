
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

    private Vector3 worldGridOffset = new Vector3(0.5f, 0.5f, 0.0f);

	void Update () {
        // Update our mouse position.
        MouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseGridPosition = BoardManager.WorldToGridPoint(
            MouseWorldPosition + worldGridOffset
        );

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
