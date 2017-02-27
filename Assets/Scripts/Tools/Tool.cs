
using System.Collections.Generic;

public abstract class Tool {
    public virtual IEnumerable<IntVector2> GetSelectedPositions(IntDragger2 selection) {
        return GetPositions_Fill(selection);
    }

    public virtual IEnumerable<Board.Tile> FilterSelectedTiles(
        IEnumerable<Board.Tile> selectedTiles
    ) {
        return selectedTiles;
    }

    public abstract void DoAction(IEnumerable<Board.Tile> tiles);

    protected IEnumerable<IntVector2> GetPositions_Fill(IntDragger2 selection) {
        return selection.box.Positions();
    }

    protected IEnumerable<IntVector2> GetPositions_Border(IntDragger2 selection) {
        return selection.box.Border();
    }

    protected IEnumerable<IntVector2> GetPositions_Line(IntDragger2 selection) {
        IntBox2D box = selection.box;
        if (box.height > box.width) {
            for (int y = box.top; y >= box.bottom; --y) {
                yield return new IntVector2(selection.downPosition.x, y);
            }
        }
        else {
            for (int x = box.left; x <= box.right; ++x) {
                yield return new IntVector2(x, selection.downPosition.y);
            }
        }
    }
}
