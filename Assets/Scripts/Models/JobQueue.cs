
using System.Collections.Generic;

public class JobQueue {
    public class Job {
        public IntVector2 position {get; private set;}

        private Job(IntVector2 pos) {
            position = pos;
        }

        private Job() {}

        public static Job MakeProtoJob() {
            // TODO:    Take in parameters describing the job, such as its name,
            //          effort requirements, actions, and possibly position
            //          validator.
            return new Job();
        }

        public static Job Instantiate(Job jobProto, IntVector2 pos) {
            // TODO: Update the new job with copies of our action/update funcs.
            return new Job(pos);
        }

        public bool Update() {
            // TODO:    Take in an "effort" or "work" amount to subtract from
            //          the job's time-to-completion meter. When meter reaches
            //          zero, job is completed so return true.
            BoardManager.Board.SetTileType(position, Board.TileType.Edge);
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

    private Queue<Job> jobs = new Queue<Job>();

    public JobQueue() {}

    public Job MakeProtoJob() {
        return Job.MakeProtoJob();
    }

    public void AddJob(Job jobProto, IntVector2 position) {
        Job job = Job.Instantiate(jobProto, position);
        jobs.Enqueue(job);
    }

    public Job ClaimJob() {
        Job job = jobs.Dequeue();
        return job;
    }
}
