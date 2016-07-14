using UnityEngine;
using System.Collections;

public class UnionManager : MonoBehaviour {
    public static UnionManager instance;
    public static JobQueue Jobs {get { return instance.jobs; }}

    public GameObject workerPrefab;
    public JobQueue jobs {get; private set;}

	void Start () {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        // TODO: Manage work force based on demand.
        jobs = new JobQueue();
        GameObject worker = (GameObject)Instantiate(workerPrefab);
        worker.transform.parent = transform;
	}

	void Update () {
        // TODO: Manage work queue and resolve/collapse dependencies.
	}
}
