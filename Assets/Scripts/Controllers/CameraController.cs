using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public Vector3 movement = Vector3.zero;
    public float speed;

	void Update () {
        movement = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
            movement.y += speed;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            movement.y -= speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            movement.x -= speed;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            movement.x += speed;
        }

        transform.position += movement * Time.deltaTime;
	}
}
