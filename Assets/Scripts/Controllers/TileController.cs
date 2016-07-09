
using UnityEngine;
using System;
using System.Collections;

public class TileController : MonoBehaviour {
    public Board.TileType type = Board.TileType.Edge;
    public IntVector2 gridPosition;
    public IntVector2 displayPosition;
    public Board.Tile tile;

    private SpriteRenderer backgroundSpriteRenderer;
    private SpriteRenderer largeItemSpriteRenderer;

    void Start() {
        // Fetch the sprite renderers.
        backgroundSpriteRenderer = GetChildSpriteRenderer("Background");
        largeItemSpriteRenderer = GetChildSpriteRenderer("Large Item");

        SetType(type);
    }

    public void SetType(Board.TileType newType) {
        type = newType;
        if (backgroundSpriteRenderer != null) {
            // This function is called by the TileManager when it instantiates
            // the Tile prefab. That means it is executed _before_ Start, and
            // thus we won't have cached the sprite renderer yet.
            backgroundSpriteRenderer.sprite = TileManager.instance.tileSprites[(int)type];
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
        if (backgroundSpriteRenderer == null || largeItemSpriteRenderer == null) {
            // This can happen if `Render` is called _before_ `Start`, which
            // happens when first constructed. It is a normal thing to happen
            // so we just ignore this state.
            return;
        }

        // TODO: Render a sprite for each item on the tile.

        largeItemSpriteRenderer.sprite = null;
        TileManager tm = TileManager.instance;

        // No tile? Render an edge!
        if (tile == null) {
            SetType(Board.TileType.Edge);
            return;
        }

        // If we have a large item, fetch its sprite.
        if (tile.HasLargeItem()) {
            Debug.Log("Tile" + gridPosition + " has a large item!");
            Sprite sprite = null;
            if (tm.itemSprites.TryGetValue(tile.GetLargeItem().name, out sprite)) {
                largeItemSpriteRenderer.sprite = sprite;
            }
            else {
                Debug.LogError("Could not find sprite for " + tile.GetLargeItem().name);
            }
        }

        SetType(tile.type);
    }

    private SpriteRenderer GetChildSpriteRenderer(string goName) {
        Debug.Log("Getting sprite renderer for " + goName);
        Transform trans = transform.Find(goName);
        if (trans == null) {
            Debug.LogError("Could not find transform for " + goName);
            return null;
        }

        SpriteRenderer sr = trans.gameObject.GetComponent<SpriteRenderer>();
        if (sr == null) {
            Debug.LogError("Can not find sprite renderer for " + goName);
        }

        return sr;
    }
}
