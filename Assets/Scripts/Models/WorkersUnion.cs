
using System;
using System.Collections.Generic;

public class WorkersUnion {
    private Game game;
    private List<Worker> workers;
    private Action<Events.WorkerEvent> onWorkerEvent;

    public WorkersUnion(Game game) {
        this.game = game;
        this.workers = new List<Worker>();
    }

    public Worker CreateWorker() {
        // Create the worker and add it to our list.
        Worker worker = new Worker(game);
        workers.Add(worker);

        // Send out an event that this was created then return the new worker.
        SendWorkerEvent(Events.WorkerEventType.WorkerCreated, worker);
        return worker;
    }

    public void Update(float deltaTime) {
        // TODO: Manage work force based on demand.

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
