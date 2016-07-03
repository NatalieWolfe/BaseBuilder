
using UnityEngine;
using System;
using System.Collections;

public class TileManager : MonoBehaviour {
    public static TileManager instance;

    public GameObject tilePrefab;
    public Sprite[] sprites;

    public int displayWidth;
    public int displayHeight;
    private TileController[,] displayTiles;

    private CameraController camController;
    private IntVector2 tileTL = IntVector2.zero;
    private IntVector2 tileBR = IntVector2.zero;

	// Use this for initialization
	void Start() {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        if (Enum.GetValues(typeof(Board.TileType)).Length != sprites.Length) {
            Debug.LogError("Tile sprites and TileTypes do not match!");
        }

        camController = Camera.main.GetComponent<CameraController>();
        if (camController == null) {
            Debug.LogError("TileManager could not find main camera's controller.");
        }

        BuildDisplayBoard();
	}

	void Update() {
        // If the camera controller moved this last frame, let's move our tiles
        // to keep centered around it.
        if (camController.movement.x != 0 || camController.movement.y != 0) {
            MoveTiles();
        }
	}

    public void Redraw() {
        Board board = BoardManager.Board;
        for (int x = 0; x < displayWidth; ++x) {
            for (int y = 0; y < displayHeight; ++y) {
                TileController controller = displayTiles[x, y];
                controller.SetType(board.GetTileType(controller.gridPosition));
            }
        }
    }

    private void MoveTiles() {
        // To start, we need to figure out where the board's top-left and
        // bottom-right corners _should_ be. This is determined simply from the
        // position of the camera like this:
        //
        // TargetTop    = CameraY + HalfBoardHeight;
        // TargetLeft   = CameraX - HalfBoardWidth;
        // TargetBottom = TargetTop - BoardHeight;
        // TargetRight  = TargetLeft + BoardWidth;
        TileController topLeft  = displayTiles[tileTL.x, tileTL.y];
        TileController botRight = displayTiles[tileBR.x, tileBR.y];


        Vector2 targetTL = new Vector2(
            camController.transform.position.x - (displayWidth / 2f),
            camController.transform.position.y + (displayHeight / 2f)
        );
        Vector2 targetBR = new Vector2(
            targetTL.x + (float)displayWidth,
            targetTL.y - (float)displayHeight
        );

        // Now we know where the board _should_ be, so move the rows until they
        // are close to where we want them. The moving functions will also
        // update the top-left and bottom-right positions as they go, hence why
        // they are passed in by reference.

        // Move the top or bottom rows until they are at the target locations.
        // Typically only one of these will run depending on the direction the
        // camera moved.
        while (topLeft.transform.position.y > targetTL.y) {
            MoveTopRow(ref topLeft, ref botRight);
        }
        while (botRight.transform.position.y < targetBR.y) {
            MoveBottomRow(ref topLeft, ref botRight);
        }

        // Do the same for the left and right columns.
        while (topLeft.transform.position.x < targetTL.x) {
            MoveLeftColumn(ref topLeft, ref botRight);
        }
        while (botRight.transform.position.x > targetBR.x) {
            MoveRightColumn(ref topLeft, ref botRight);
        }

        // We've moved our tiles around, now store the positions in our
        // displayTiles grid of the visually top-left and bottom-right tiles.
        tileTL = topLeft.displayPosition;
        tileBR = botRight.displayPosition;
        Debug.Log("New corners: " + tileTL + ", " + tileBR)
    }

    private void BuildDisplayBoard() {
        // First, destroy our existing board.
        if (displayTiles != null) {
            DestroyDisplayTiles();
        }

        // Now construct the new tiles.
        displayTiles = new TileController[displayWidth, displayHeight];
        for (int x = 0; x < displayWidth; ++x) {
            for (int y = 0; y < displayHeight; ++y) {
                // Create a new tile instance.
                GameObject tile = Instantiate(tilePrefab) as GameObject;
                tile.name = "Tile (" + x + ", " + y + ")";
                tile.transform.parent = gameObject.transform;

                // Set up the tile's controller.
                TileController controller = tile.GetComponent<TileController>();
                controller.displayPosition = new IntVector2(x, y);
                controller.MoveTo(x, y);
                displayTiles[x, y] = controller;
            }
        }

        tileTL = new IntVector2(0, displayHeight - 1);
        tileBR = new IntVector2(displayWidth - 1, 0);
        Debug.Log("Board constructed at " + displayWidth + "x" + displayHeight);

        // After building the board we'll need to move the board to be centered
        // around the screen.
        MoveTiles();
    }

    private void DestroyDisplayTiles() {
        for (int x = 0; x < displayWidth; ++x) {
            for (int y = 0; y < displayHeight; ++y) {
                Destroy(displayTiles[x, y].gameObject);
            }
        }
        displayTiles = null;
    }

    private void MoveTopRow(ref TileController topLeft, ref TileController botRight) {
        int topDispRowY = topLeft.displayPosition.y;
        int botGridRowY = botRight.gridPosition.y;

        TileController rightMost = null;

        for (int x = 0; x < displayWidth; ++x) {
            TileController controller = displayTiles[x, topDispRowY];
            controller.MoveTo(controller.gridPosition.x, botGridRowY - 1);

            if (rightMost == null || controller.IsRightOf(rightMost.gridPosition)) {
                rightMost = controller;
            }
        }

        botRight = rightMost;
        topLeft = displayTiles[
            topLeft.displayPosition.x,
            (botRight.displayPosition.y - 1 + displayHeight) % displayHeight
        ];
    }

    private void MoveBottomRow(ref TileController topLeft, ref TileController botRight) {
        int botDispRowY = botRight.displayPosition.y;
        int topGridRowY = topLeft.gridPosition.y;

        TileController leftMost = null;

        for (int x = 0; x < displayWidth; ++x) {
            TileController controller = displayTiles[x, botDispRowY];
            controller.MoveTo(controller.gridPosition.x, topGridRowY + 1);

            if (leftMost == null || controller.IsLeftOf(leftMost.gridPosition)) {
                leftMost = controller;
            }
        }

        topLeft = leftMost;
        botRight = displayTiles[
            botRight.displayPosition.x,
            (topLeft.displayPosition.y + 1) % displayHeight
        ];
    }

    private void MoveLeftColumn(ref TileController topLeft, ref TileController botRight) {
        int leftDispColX = topLeft.displayPosition.x;
        int rightGridColX = botRight.gridPosition.x;

        TileController lowest = null;

        for (int y = 0; y < displayHeight; ++y) {
            TileController controller = displayTiles[leftDispColX, y];
            controller.MoveTo(rightGridColX + 1, controller.gridPosition.y);

            if (lowest == null || controller.IsBelow(lowest.gridPosition)) {
                lowest = controller;
            }
        }

        botRight = lowest;
        topLeft = displayTiles[
            (topLeft.displayPosition.x + 1) % displayWidth,
            topLeft.displayPosition.y
        ];
    }

    private void MoveRightColumn(ref TileController topLeft, ref TileController botRight) {
        int rightDispColX = botRight.displayPosition.x;
        int leftGridColX = topLeft.gridPosition.x;

        TileController highest = null;

        for (int y = 0; y < displayHeight; ++y) {
            TileController controller = displayTiles[rightDispColX, y];
            controller.MoveTo(leftGridColX - 1, controller.gridPosition.y);

            if (highest == null || controller.IsAbove(highest.gridPosition)) {
                highest = controller;
            }
        }

        topLeft = highest;
        botRight = displayTiles[
            (botRight.displayPosition.x - 1 + displayWidth) % displayWidth,
            botRight.displayPosition.y
        ];
    }
}
