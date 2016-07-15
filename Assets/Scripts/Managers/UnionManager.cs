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

        GameManager.Game.union.RegisterOnWorkerEvent(OnWorkerEvent);

        // FIXME: This is just for debugging!
        GameManager.Game.union.CreateWorker();
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
        else if (e.workerEventType == Events.WorkerEventType.WorkerDestroyed) {
            // TODO: Destroy game object when worker destroyed.
            // TODO: Death animations?
        }

    }
}
