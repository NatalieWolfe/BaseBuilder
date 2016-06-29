using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float speed;

    private bool hasTarget = false;
    private Vector2 targetPosition;

	void Update () {
        if (InputManager.Action) {
            // Use the mouse grid position to ensure our target is in the middle
            // of the tile clicked rather than whatever pixel the mouse clicked.
            targetPosition = BoardManager.GridToWorldPoint(
                InputManager.MouseGridPosition
            );
            hasTarget = true;
            Debug.Log("Setting target to " + targetPosition);
        }
        if (hasTarget) {
            Vector2 ourPosition = transform.position;
            if (targetPosition == ourPosition) {
                Debug.Log("Arrived at target position.");
                hasTarget = false;
            }
            else {
                float dist = speed * Time.deltaTime;
                Vector3 movement = new Vector3(
                    Mathf.Clamp(targetPosition.x - ourPosition.x, -dist, dist),
                    Mathf.Clamp(targetPosition.y - ourPosition.y, -dist, dist),
                    0
                );

                transform.position += movement;
            }
        }
	}
}
