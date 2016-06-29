
using UnityEngine;
using System;
using System.Collections;

public class TileManager : MonoBehaviour {
    public enum Border { North, East, South, West };

    public static TileManager instance;

    public GameObject tilePrefab;
    public Sprite[] sprites;

    private int displayWidth;
    private int displayHeight;
    private TileController[,] displayTiles;

    private Vector3 cameraBoundsTL;
    private Vector3 cameraBoundsBR;
    private IntVector2 tileTL = IntVector2.zero;
    private IntVector2 tileBR = IntVector2.zero;

	// Use this for initialization
	void Start () {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        if (Enum.GetValues(typeof(Board.TileType)).Length != sprites.Length) {
            Debug.LogError("Tile sprites and TileTypes do not match!");
        }

        ResizeDisplayBoard();
	}

	void Update () {
        // Get the camera controller and see if it has moved recently. If it has
        // not, then don't bother updating the displayed tiles.
        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (cam == null || (cam.movement.x == 0 && cam.movement.y == 0)) {
            return;
        }

        // On Update we will be adjusting the tiles that make up the displayed
        // board area. When these tiles move beyond the bounds of the camera
        // render area we move them to the opposite side of the board.
        TileController topLeft = displayTiles[tileTL.x, tileTL.y];
        TileController botRight = displayTiles[tileBR.x, tileBR.y];
        Vector3 camTL = cam.transform.position + cameraBoundsTL;
        Vector3 camBR = cam.transform.position + cameraBoundsBR;

        // Shift the top or bottom row if either is out of bounds.
        if (cam.movement.y > 0 && topLeft.transform.position.y < camTL.y) {
            MoveTopRow(ref topLeft, ref botRight);
        }
        else if (cam.movement.y < 0 && botRight.transform.position.y > camBR.y) {
            MoveBottomRow(ref topLeft, ref botRight);
        }

        // Do the same for the left and right columns.
        if (cam.movement.x > 0 && topLeft.transform.position.x < camTL.x) {
            MoveLeftColumn(ref topLeft, ref botRight);
        }
        else if (cam.movement.x < 0 && botRight.transform.position.x > camBR.x) {
            MoveRightColumn(ref topLeft, ref botRight);
        }

        // Get the new corner tile indexes.
        tileTL = topLeft.displayPosition;
        tileBR = botRight.displayPosition;
	}

    private void ResizeDisplayBoard() {
        // First, destroy our existing board.
        if (displayTiles != null) {
            DestroyDisplayTiles();
        }

        // Find the bounds of the camera. These are used check the position of
        // tiles at the edges to see if they need to be moved.
        // Since this is an orthographic 2D game, we can directly use the
        // camera's own pixel dimensions to determine the world-coords of the
        // bounds.
        cameraBoundsTL = Camera.main.ScreenToWorldPoint(Vector3.zero);
        cameraBoundsBR = Camera.main.ScreenToWorldPoint(
            new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0)
        );
        cameraBoundsTL -= Camera.main.transform.position;
        cameraBoundsBR -= Camera.main.transform.position;
        Debug.Log("Camera bounds: " + cameraBoundsTL + "; " + cameraBoundsBR);

        // Adjust the camera bounds to have an extra layer of tiles around them.
        Vector3 tileDims = new Vector3(1, 1, 0);
        cameraBoundsTL -= tileDims;
        cameraBoundsBR += tileDims;

        // Determine how many tiles we need to fill the camera view.
        Vector3 cameraSize = cameraBoundsBR - cameraBoundsTL;
        displayWidth = Mathf.CeilToInt(cameraSize.x);
        displayHeight = Mathf.CeilToInt(cameraSize.y);
        Debug.Log("New display size: " + displayWidth + "x" + displayHeight);

        // Now construct the new tiles.
        IntVector2 camGridTL = BoardManager.Board.WorldToGridPoint(cameraBoundsTL);
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
                controller.MoveTo(x + camGridTL.x, y + camGridTL.y);
                displayTiles[x, y] = controller;
            }
        }

        tileTL = new IntVector2(0, 0);
        tileBR = new IntVector2(displayWidth - 1, displayHeight - 1);
        Debug.Log("Board constructed at " + camGridTL);
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
            controller.MoveTo(controller.gridPosition.x, botGridRowY + 1);

            if (rightMost == null || controller.IsRightOf(rightMost.gridPosition)) {
                rightMost = controller;
            }
        }

        botRight = rightMost;
        topLeft = displayTiles[
            topLeft.displayPosition.x,
            (topLeft.displayPosition.y + 1) % displayHeight
        ];
    }

    private void MoveBottomRow(ref TileController topLeft, ref TileController botRight) {
        int botDispRowY = botRight.displayPosition.y;
        int topGridRowY = topLeft.gridPosition.y;

        TileController leftMost = null;

        for (int x = 0; x < displayWidth; ++x) {
            TileController controller = displayTiles[x, botDispRowY];
            controller.MoveTo(controller.gridPosition.x, topGridRowY - 1);

            if (leftMost == null || controller.IsLeftOf(leftMost.gridPosition)) {
                leftMost = controller;
            }
        }

        topLeft = leftMost;
        botRight = displayTiles[
            botRight.displayPosition.x,
            (botRight.displayPosition.y - 1 + displayHeight) % displayHeight
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
