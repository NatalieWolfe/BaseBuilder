
﻿using UnityEngine;
using System;

public class CameraController : MonoBehaviour {
    public Vector3 movement = Vector3.zero;
    public float speed;
    public float speedScale;

    [Range(0,2)]
    public float zoomRate;
    public float minimumZoom;
    public float maximumZoom;

    private Camera camera;
    private Action onCameraChanged;
    private Dragger3 mouseDrag;

    void Start() {
        camera = GetComponent<Camera>();
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
        float dist = speed * Time.deltaTime * camera.orthographicSize / speedScale;
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
        float zoom = Input.mouseScrollDelta.y;
        if (Mathf.Abs(zoom) > Mathf.Epsilon) {
            float size = camera.orthographicSize - zoom * zoomRate;
            size = Math.Min(maximumZoom, Math.Max(minimumZoom, size));
            if (size != camera.orthographicSize) {
                camera.orthographicSize = size;
                return true;
            }
        }

        return false;
    }
}
