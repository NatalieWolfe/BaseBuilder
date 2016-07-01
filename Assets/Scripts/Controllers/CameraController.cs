
﻿using UnityEngine;
using System;

public class CameraController : MonoBehaviour {
    public Vector3 movement = Vector3.zero;
    public float speed;

    private Action onCameraChanged;
    private Dragger3 mouseDrag;

    void Start() {
        mouseDrag = new Dragger3(KeyCode.Mouse2);
    }

	void Update() {
        UpdateMovement();
        transform.position += movement;

        if (UpdateCameraSize()) {
            onCameraChanged();
        }
	}

    public void RegisterOnCameraChanged(Action callback) {
        onCameraChanged += callback;
    }

    public void UnregisterOnCameraChanged(Action callback) {
        onCameraChanged -= callback;
    }

    private void UpdateMovement() {
        movement = Vector3.zero;
        UpdateMovementFromKeys();
        UpdateMovementFromMouse();
    }

    private void UpdateMovementFromKeys() {
        float dist = speed * Time.deltaTime;
        if (InputManager.Vertical > 0) {
            movement.y += dist;
        }
        else if (InputManager.Vertical < 0) {
            movement.y -= dist;
        }

        if (InputManager.Horizontal > 0) {
            movement.x += dist;
        }
        else if (InputManager.Horizontal < 0) {
            movement.x -= dist;
        }
    }

    private void UpdateMovementFromMouse() {
        mouseDrag.Update();
        if (mouseDrag.dragging) {
            movement += mouseDrag.delta;
        }
    }

    private bool UpdateCameraSize() {
        return false;
    }
}
