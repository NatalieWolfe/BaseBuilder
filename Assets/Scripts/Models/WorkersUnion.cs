
using System;
using System.Collections.Generic;

public class WorkersUnion {
    private Game game;
    private List<Worker> workers;
    private Action<Events.WorkerEvent> onWorkerEvent;

    public WorkersUnion(Game game) {
        // TODO:    Different kinds of unions that focus their workers on
        //          specific kinds of tasks.
        this.game = game;
        this.workers = new List<Worker>();
    }

    public Worker CreateWorker() {
        // Create the worker and add it to our list.
        Worker worker = new Worker(this);
        workers.Add(worker);

        // Send out an event that this was created then return the new worker.
        SendWorkerEvent(Events.WorkerEventType.WorkerCreated, worker);
        return worker;
    }

    public JobQueue.Job FindJob(Worker worker) {
        // TODO:    Move to some central meeting area or frequently used
        //          stockpile to be ready for a new job.
        // TODO: Take jobs away from other workers.
        // TODO: Grab the job based on our position.
        // TODO: Grab the job based on our skill set.

        if (game.jobs.Count > 0) {
            return game.jobs.ClaimJob();
        }
        return null;
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
