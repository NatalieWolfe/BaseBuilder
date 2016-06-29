using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public Vector3 movement = Vector3.zero;
    public float speed;

	void Update () {
        movement = Vector3.zero;
        if (InputManager.Vertical > 0) {
            movement.y += speed;
        }
        else if (InputManager.Vertical < 0) {
            movement.y -= speed;
        }

        if (InputManager.Horizontal > 0) {
            movement.x += speed;
        }
        else if (InputManager.Horizontal < 0) {
            movement.x -= speed;
        }

        transform.position += movement * Time.deltaTime;
	}
}
