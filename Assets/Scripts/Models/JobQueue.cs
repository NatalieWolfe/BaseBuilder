
using UnityEngine; // For Debug.Log
using System.Collections.Generic;

public class JobQueue {
    public class Job {
        public IntVector2 position {get; private set;}
        public IEnumerable<string> requirements {get { return requiredItemTypes; }}

        private Game game;
        private List<string> requiredItemTypes;

        private Job(Game game, IntVector2 pos) {
            this.game = game;
            this.position = pos;
            this.requiredItemTypes = new List<string>();

            // TODO: Have required items be configurable.
            requiredItemTypes.Add("Wood");
        }

        private Job() {}

        public static Job MakeProtoJob() {
            // TODO:    Take in parameters describing the job, such as its name,
            //          effort requirements, actions, and possibly position
            //          validator.
            return new Job();
        }

        public static Job Instantiate(Game game, Job jobProto, IntVector2 pos) {
            // TODO: Update the new job with copies of our action/update funcs.
            return new Job(game, pos);
        }

        public bool Update() {
            // TODO:    Take in an "effort" or "work" amount to subtract from
            //          the job's time-to-completion meter. When meter reaches
            //          zero, job is completed so return true.
            Board.Tile tile = game.board.GetTile(position);
            if (tile.HasLargeItem()) {
                Debug.LogError(
                    "Dropping job, Tile" + position + " already has large item."
                );
                return true; // TODO: Replace with some other status check.
            }

            Item.Wall wall = new Item.Wall(game);
            wall.position = position;
            tile.SetLargeItem(wall);

            return true;
        }

        public bool IsPositionValid(IntVector2 pos) {
            // TODO: Make this configurable using a lambda.
            return true;
        }

        public IEnumerable<IntVector2> ValidPositions(IntBox2D box) {
            // TODO: Make this configurable using a lambda.
            return box.Positions();
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

    public Job MakeProtoJob() {
        return Job.MakeProtoJob();
    }

    public void AddJob(Job jobProto, IntVector2 position) {
        Job job = Job.Instantiate(game, jobProto, position);
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
