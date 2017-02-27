
using UnityEngine; // For Debug.Log
using System.Collections.Generic;

public class JobQueue {
    public abstract class Job {
        //  vvv For some future, generic job class. vvv
        // TODO:    Take in parameters describing the job, such as its name,
        //          effort requirements, actions, and possibly position validator.

        public IntVector2 position {get; private set;}
        public IEnumerable<string> requirements {get { return requiredItemTypes; }}

        protected Game game;
        protected List<string> requiredItemTypes;

        protected Job(Game game, IntVector2 pos) {
            this.game = game;
            this.position = pos;
            this.requiredItemTypes = new List<string>();
        }

        protected Job() {}

        public abstract Job Clone(Game game, IntVector2 pos);

        public abstract bool Update(float deltaTime);

        public virtual bool IsTileValid(Board.Tile tile) {
            // TODO: Make this configurable using a lambda.
            return true;
        }
    }

    public IEnumerable<Job> Jobs {get { return jobs; }}
    public int Count {get { return jobs.Count; }}

    private Game game;
    private Queue<Job> jobs = new Queue<Job>();

    public JobQueue(Game game) {
        this.game = game;
    }

    public void Update(float deltaTime) {
        // TODO: Manage work queue and resolve/collapse dependencies.
        // TODO: "Age" tasks, expire/reprioritize by age.
    }

    public void AddJob(Job jobProto, IntVector2 position) {
        Job job = jobProto.Clone(game, position);
        jobs.Enqueue(job);
    }

    public Job ClaimJob() {
        Job job = jobs.Dequeue();
        return job;
    }

    public void UnclaimJob(Job job) {
        // TODO: Store the job's priority and requeue it in the same position.
        jobs.Enqueue(job);
    }
}
