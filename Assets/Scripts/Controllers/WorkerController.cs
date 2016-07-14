
﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class WorkerController : MonoBehaviour {
    public Worker worker;

	void Update () {
        // TODO: Short-circuit update if we're outside of rendering.
        // TODO: Update facing.
        // TODO: Only update the position if our position has changed.
        transform.position = IntVector2.Lerp(
            worker.position,
            worker.movingIntoPosition,
            worker.moveCompletion
        );
	}
}
