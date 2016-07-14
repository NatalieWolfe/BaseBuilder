using UnityEngine;
using System.Collections;

public class UnionManager : MonoBehaviour {
    public static UnionManager instance;

    public GameObject workerPrefab;

	void Start () {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        // TODO: Manage work force based on demand.
        WorkersUnion union = WorkersUnion.instance;
        union.RegisterOnWorkerEvent(OnWorkerEvent);

        // FIXME: This is just for debugging!
        union.CreateWorker();
	}

	void Update () {
        // TODO: Manage work queue and resolve/collapse dependencies.
        WorkersUnion.instance.Update(Time.deltaTime);
	}

    private void OnWorkerEvent(Events.WorkerEvent e) {
        if (e.workerEventType == Events.WorkerEventType.WorkerCreated) {
            GameObject worker = (GameObject)Instantiate(workerPrefab);
            worker.transform.parent = transform;

            WorkerController controller = worker.GetComponent<WorkerController>();
            if (controller == null) {
                Debug.LogError("Worker prefab does not contain worker controller!");
                Destroy(worker);
            }

            controller.worker = e.worker;
        }
    }
}
