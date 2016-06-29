using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    public static float Horizontal = 0.0f;
    public static float Vertical = 0.0f;
    public static bool Action = false;

    public static Vector3 MouseWorldPosition {
        get {
            if (!_mouseUpdated) {
                _mouseUpdated = true;
                _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _mousePosition.x += 0.5f;
                _mousePosition.y += 0.5f;
            }
            return _mousePosition;
        }
    }

    public static IntVector2 MouseGridPosition {
        get { return BoardManager.WorldToGridPoint(MouseWorldPosition); }
    }

    // Cache for mouse position so we only do the translation from screen space
    // to world space at most _once_ per frame. Probably the root of all evil. ;)
    private static bool _mouseUpdated;
    private static Vector3 _mousePosition;

	void Update () {
        // Clear that old mouse position, it will be refilled if needed.
        _mouseUpdated = false;

        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        Action = Input.GetButtonDown("Action");
	}
}
