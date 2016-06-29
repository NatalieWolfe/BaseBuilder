using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    public static float Horizontal = 0.0f;
    public static float Vertical = 0.0f;
    public static bool Action = false;

	void Update () {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        Action = Input.GetButtonDown("Action");
	}
}
