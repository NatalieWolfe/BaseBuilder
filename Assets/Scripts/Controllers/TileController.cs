
using UnityEngine;
using System;
using System.Collections;

public class TileController : MonoBehaviour {
    public Board.TileType type = Board.TileType.Edge;
    public IntVector2 gridPosition;
    public IntVector2 displayPosition;
    public Board.Tile tile;

    private SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) {
            Debug.LogError("Can not find tile's sprite renderer.");
        }

        SetType(type);
    }

    public void SetType(Board.TileType newType) {
        type = newType;
        if (spriteRenderer != null) {
            // This function is called by the TileManager when it instantiates
            // the Tile prefab. That means it is executed _before_ Start, and
            // thus we won't have cached the sprite renderer yet.
            spriteRenderer.sprite = TileManager.instance.sprites[(int)type];
        }
    }

    public void MoveTo(IntVector2 gridPos) {
        // Update our position in the world.
        gridPosition = gridPos;
        Vector3 newPos = BoardManager.GridToWorldPoint(gridPos);
        newPos.z = transform.position.z;
        transform.position = newPos;

        // Update the tile and render it.
        tile = BoardManager.Board.GetTile(gridPosition);
        Render();
    }

    public void MoveTo(int x, int y) {
        MoveTo(new IntVector2(x, y));
    }

    public bool IsAbove(IntVector2 gridPos) {
        return gridPosition.IsAbove(gridPos);
    }

    public bool IsBelow(IntVector2 gridPos) {
        return gridPosition.IsBelow(gridPos);
    }

    public bool IsRightOf(IntVector2 gridPos) {
        return gridPosition.IsRightOf(gridPos);
    }

    public bool IsLeftOf(IntVector2 gridPos) {
        return gridPosition.IsLeftOf(gridPos);
    }

    void OnMouseDown() {
        Debug.Log(
            "Clicked " + gridPosition + " (mouse " + InputManager.MouseGridPosition + ")"
        );
    }

    public void OnTileEvent(Events.TileEvent e) {
        Render();
    }

    public void Render() {
        // TODO: Render a sprite for each item on the tile.
        if (tile == null) {
            SetType(Board.TileType.Edge);
        }
        else if (tile.HasLargeItem()) {
            Debug.Log("Tile" + gridPosition + " has a large item!");

            // TODO: Render the large item.
            SetType(Board.TileType.Edge);
        }
        else {
            SetType(tile.type);
        }
    }
}
