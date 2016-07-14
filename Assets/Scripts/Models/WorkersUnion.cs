
using System;
using System.Collections.Generic;

public class WorkersUnion {
    public static WorkersUnion instance {
        get {
            if (_instance == null) {
                _instance = new WorkersUnion();
            }
            return _instance;
        }
    }
    private static WorkersUnion _instance;

    public static JobQueue Jobs {get { return instance.jobs; }}
    public JobQueue jobs {get; private set;}

    private List<Worker> workers;
    private Action<Events.WorkerEvent> onWorkerEvent;

    private WorkersUnion() {
        this.jobs = new JobQueue();
        this.workers = new List<Worker>();
    }

    public Worker CreateWorker() {
        // Create the worker and add it to our list.
        Worker worker = new Worker();
        workers.Add(worker);

        // Send out an event that this was created then return the new worker.
        SendWorkerEvent(Events.WorkerEventType.WorkerCreated, worker);
        return worker;
    }

    public void Update(float deltaTime) {
        foreach (Worker worker in workers) {
            worker.Update(deltaTime);
        }
    }

    public void OnWorkerEvent(Events.WorkerEvent e) {
        if (onWorkerEvent != null) {
            onWorkerEvent(e);
        }
    }

    public void RegisterOnWorkerEvent(Action<Events.WorkerEvent> action) {
        onWorkerEvent += action;
    }

    public void UnregisterOnWorkerEvent(Action<Events.WorkerEvent> action) {
        onWorkerEvent -= action;
    }

    private void SendWorkerEvent(Events.WorkerEventType type, Worker worker) {
        OnWorkerEvent(new Events.WorkerEvent(type, worker));
    }
}
