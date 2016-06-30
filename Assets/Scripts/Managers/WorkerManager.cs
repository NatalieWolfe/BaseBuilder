using UnityEngine;
using System.Collections;

public class WorkerManager : MonoBehaviour {
    public static WorkerManager instance;

    public GameObject workerPrefab;

	void Start () {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        // TODO: Manage work force based on demand.
        GameObject worker = (GameObject)Instantiate(workerPrefab);
        worker.transform.parent = transform;
	}

	void Update () {
        // TODO: Manager work queue and resolve/collapse dependencies.
	}
}
