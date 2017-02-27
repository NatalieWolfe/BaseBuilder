
using UnityEngine; // For Debug.Log
using System.Collections.Generic;

public class ConstructTool : Tool {
    private LargeItem itemToBuild;
    private JobQueue.Job jobProto;

    public ConstructTool(LargeItem blueprint) {
        itemToBuild = blueprint;
        jobProto = Jobs.ConstructionJob.MakeProto(itemToBuild);
    }

    public override IEnumerable<IntVector2> GetSelectedPositions(IntDragger2 selection) {
        // TODO: Enum of construction modes.
        IEnumerable<IntVector2> items = null;
        if (itemToBuild.constructionMode == "Fill") {
            items = GetPositions_Fill(selection);
        }
        else if (itemToBuild.constructionMode == "Border") {
            items = GetPositions_Border(selection);
        }
        else if (itemToBuild.constructionMode == "Line") {
            items = GetPositions_Line(selection);
        }
        else if (itemToBuild.constructionMode == "Single") {
            yield return selection.downPosition;
            yield break;
        }
        else {
            Debug.LogError("Unknown construction mode: " + itemToBuild.constructionMode);
            yield break;
        }

        foreach (IntVector2 item in items) {
            yield return item;
        }
    }

    public override IEnumerable<Board.Tile> FilterSelectedTiles(
        IEnumerable<Board.Tile> selectedTiles
    ) {
        foreach (Board.Tile tile in selectedTiles) {
            if (jobProto.IsTileValid(tile)) {
                yield return tile;
            }
        }
    }

    public override void DoAction(IEnumerable<Board.Tile> tiles) {
        JobQueue jobs = GameManager.Game.jobs;

        foreach (Board.Tile tile in tiles) {
            jobs.AddJob(jobProto, tile.position);
        }
    }
}
