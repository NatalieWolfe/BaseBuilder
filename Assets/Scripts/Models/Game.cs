
public class Game {
    public Board        board   {get; private set;}
    public ItemDatabase itemDB  {get; private set;}
    public JobQueue     jobs    {get; private set;}
    public WorkersUnion union   {get; private set;}

    public Game(int width, int height) {
        this.board  = new Board(this, width, height);
        this.itemDB = new ItemDatabase(this);
        this.jobs   = new JobQueue(this);
        this.union  = new WorkersUnion(this);
    }

    public void Update(float deltaTime) {
        board   .Update(deltaTime);
        itemDB  .Update(deltaTime);
        jobs    .Update(deltaTime);
        union   .Update(deltaTime);
    }
}
