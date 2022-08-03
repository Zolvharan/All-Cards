using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer image;
    public SpriteRenderer tintImage;
    // Defuault, selected, attacking, ability
    Color32[] tints = { new Color32(255, 255, 255, 0), new Color32(0, 150, 255, 180), new Color32(255, 0, 0, 180), new Color32(255, 255, 0, 180) };

    public CharacterStats currUnit = null;
    public BUIManager UI;

    public int xPos;
    public int yPos;
    public float moveWeight;
    // Cannot be traversed even if flying
    //public bool impassable;

    public bool selected;
    public bool occupied = false;

    // Custom properties
    bool impassable;

    public LevelGenerator map;

    const int Z_OFFSET = -2;

    TileData tileData;

    // Start is called before the first frame update
    void Start()
    {
        selected = false;
    }

    public void SelectTile(bool tilesHighlighted = true)
    {
        if (selected)
        {
            if (currUnit == null)
                tintImage.color = tints[0];
            else
            {
                if (!currUnit.moved && tilesHighlighted)
                    HighlightTiles(1, currUnit.GetMoveSpeed(), !currUnit.flying);
                if (currUnit.player)
                    UI.ToggleCharDisplay();
                else
                    UI.DisableEnemyDisplay();
            }
            selected = false;
        }
        else
        {
            if (currUnit == null)
                tintImage.color = tints[1];
            selected = true;
            if (currUnit != null)
            {
                if (currUnit.player)
                    UI.DisplayCharacter(currUnit);
                else
                    UI.DisplayEnemy(currUnit);
                if (!currUnit.moved)
                    HighlightTiles(1, currUnit.GetMoveSpeed(), !currUnit.flying);
            }
        }
    }

    public void ConstructTile(TileData newTileData, int initXPos, int initYPos)
    {
        tileData = newTileData;
        impassable = newTileData.GetImpassable();
        image.sprite = CharacterImageForm.ConstructImage(tileData.GetImage());
        moveWeight = tileData.GetMoveWeight();
        xPos = initXPos;
        yPos = initYPos;
    }
    public TileData GetTileData()
    {
        return tileData;
    }

    public void ClearUnit(bool selected)
    {
        occupied = false;
        if (selected)
            HighlightTiles(1, currUnit.GetMoveSpeed(), !currUnit.flying);
        currUnit.currTile = null;
        currUnit = null;
    }
    public void PlaceUnit(CharacterStats newUnit)
    {
        occupied = true;
        currUnit = newUnit;
        currUnit.currTile = this;
        currUnit.transform.position = transform.position + new Vector3(0, 0, Z_OFFSET);
    }
    public void MoveUnit(CharacterStats newUnit)
    {
        newUnit.currTile.currUnit = null;
        newUnit.currTile.occupied = false;
        PlaceUnit(newUnit);
    }

    public void HighlightTiles(int targetingMode, int range = -1, bool walking = true)
    {
        // 1: moving, 2: attacking, 3: casting
        // highlight tiles within range
        if (range == -1)
        {
            if (targetingMode == 1)
            {
                range = currUnit.GetMoveSpeed();
                walking = !currUnit.GetFlying();
            }
            else if (targetingMode == 2)
                range = currUnit.GetAttackRange();
        }
        HashSet<Tile> tilesToHighlight = CollectTiles(range);
        if (walking && targetingMode == 1)
        {
            tilesToHighlight = FindTilesInRange(this, range);
        }
        foreach (Tile tile in tilesToHighlight)
        {
            tile.HighlightTile(targetingMode);
        }
    }
    void HighlightTile(int targetingMode)
    {
        if (tintImage.color == tints[0])
            tintImage.color = tints[targetingMode];
        else if (tintImage.color == tints[targetingMode])
            tintImage.color = tints[0];
    }

    // Used to find places for character to move
    public HashSet<Tile> GetTiles(Tile sourceTile, int range, bool flying)
    {
        return flying ? CollectTiles(range) : FindTilesInRange(sourceTile, range);
    }
    // For when move weight is irrelevant
    public HashSet<Tile> CollectTiles(int range)
    {
        HashSet<Tile> tiles = new HashSet<Tile>();

        int j;
        for (int i = xPos - range - 1; i <= xPos + range; i++)
        {
            for (j = yPos - (range - Math.Abs(i - xPos)); j <= yPos + range - Math.Abs(i - xPos); j++)
            {
                if (i >= 0 && j >= 0 && i < map.length && j < map.height)
                    tiles.Add(map.tiles[i][j]);
            }
        }

        tiles.Add(this);
        return tiles;
    }
    Dictionary<Tile, float> currDistances;
    HashSet<Tile> FindTilesInRange(Tile sourceTile, int range)
    {
        Tile currTile;
        Tile peekTile;
        float currRange;
        currDistances = new Dictionary<Tile, float>();
        Queue<Tile> tilesToTrack = new Queue<Tile>();
        Queue<float> tilesToTrackDistances = new Queue<float>();
        tilesToTrack.Enqueue(sourceTile);
        tilesToTrackDistances.Enqueue(range);

        while (tilesToTrack.Count != 0)
        {
            currTile = tilesToTrack.Dequeue();
            currRange = tilesToTrackDistances.Dequeue();

            // Test routes in all four directions, if not at map border
            if (currTile.xPos + 1 < map.length)
            {
                peekTile = map.tiles[currTile.xPos + 1][currTile.yPos];
                TrackTile(peekTile, tilesToTrack, tilesToTrackDistances, currRange);
            }
            if (currTile.xPos > 0)
            {
                peekTile = map.tiles[currTile.xPos - 1][currTile.yPos];
                TrackTile(peekTile, tilesToTrack, tilesToTrackDistances, currRange);
            }
            if (currTile.yPos + 1 < map.height)
            {
                peekTile = map.tiles[currTile.xPos][currTile.yPos + 1];
                TrackTile(peekTile, tilesToTrack, tilesToTrackDistances, currRange);
            }
            if (currTile.yPos > 0)
            {
                peekTile = map.tiles[currTile.xPos][currTile.yPos - 1];
                TrackTile(peekTile, tilesToTrack, tilesToTrackDistances, currRange);
            }
        }

        HashSet<Tile> tilesToHighlight = new HashSet<Tile>();
        // Log all tiles reached
        foreach (KeyValuePair<Tile, float> pair in currDistances)
        {
            tilesToHighlight.Add(pair.Key);
        }

        tilesToHighlight.Add(this);
        return tilesToHighlight;
    }
    public void TrackTile(Tile peekTile, Queue<Tile> tilesToTrack, Queue<float> distances, float currRange)
    {
        // Check if tile is reachable and not logged
        // Tile is reachable if in range or range is greater or equal to 1
        if (!currDistances.ContainsKey(peekTile) && (currRange - peekTile.moveWeight >= 0 || currRange >= 1))
        {
            tilesToTrack.Enqueue(peekTile);
            distances.Enqueue(currRange - peekTile.moveWeight);
            currDistances.Add(peekTile, currRange - peekTile.moveWeight);
        }
        // Check if tile is logged, but the current route is more efficient
        else if (currDistances.ContainsKey(peekTile) && currDistances[peekTile] < currRange - peekTile.moveWeight)
        {
            tilesToTrack.Enqueue(peekTile);
            distances.Enqueue(currRange - peekTile.moveWeight);
            currDistances[peekTile] = currRange - peekTile.moveWeight;
        }
    }

    public bool IsPassable()
    {
        return !occupied && !impassable;
    }
}
