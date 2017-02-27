
using UnityEngine; // For Debug.Log

namespace Jobs {

public class ConstructionJob : JobQueue.Job {
    private LargeItem proto;

    private ConstructionJob(Game game, IntVector2 pos, LargeItem proto):
        base(game, pos)
    {
        this.proto = proto;
        this.requiredItemTypes.AddRange(proto.requirements);
    }

    private ConstructionJob(LargeItem proto) {
        this.proto = proto;
    }

    public static ConstructionJob MakeProto(LargeItem proto) {
        return new ConstructionJob(proto);
    }

    public override JobQueue.Job Clone(Game game, IntVector2 pos) {
        return new ConstructionJob(game, pos, proto);
    }

    public override bool Update(float deltaTime) {
        Board.Tile tile = game.board.GetTile(position);
        if (tile.HasLargeItem()) {
            Debug.LogError(
                "Dropping construction job, " + tile + " already has large item."
            );
            return true; // TODO: Replace with some other status check.
        }

        LargeItem item = game.itemDB.CreateLargeItem(proto);
        item.position = position;
        tile.SetLargeItem(item);

        return true;
    }

    public override bool IsTileValid(Board.Tile tile) {
        return !tile.HasLargeItem();
    }
}

}
