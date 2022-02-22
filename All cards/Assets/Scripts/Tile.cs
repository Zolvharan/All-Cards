using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer image;
    Sprite[] tileImages;
    public CharacterStats currUnit = null;
    public BUIManager UI;

    public int xPos;
    public int yPos;
    public int moveWeight;

    public bool selected;
    public bool occupied = false;

    public LevelGenerator map;

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
                image.sprite = tileImages[0];
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
                image.sprite = tileImages[1];
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

    public void CreateTile(TilePrefab tilePrefab, int initXPos, int initYPos)
    {
        tileImages = new Sprite[4];

        image.sprite = tilePrefab.image;
        tileImages[0] = tilePrefab.image;
        tileImages[1] = tilePrefab.selectImage;
        tileImages[2] = tilePrefab.attackImage;
        tileImages[3] = tilePrefab.abilityImage;
        moveWeight = tilePrefab.moveWeight;
        xPos = initXPos;
        yPos = initYPos;

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
        currUnit.transform.position = transform.position + new Vector3(0, 0, -1);
    }

    public void HighlightTiles(int targetingMode, int range = -1, bool walking = true)
    {
        // 1: moving, 2: attacking, 3: casting
        // highlight tiles within range
        if (range == -1)
        {
            if (targetingMode == 1)
                range = currUnit.GetMoveSpeed();
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
        if (image.sprite == tileImages[0])
            image.sprite = tileImages[targetingMode];
        else if (image.sprite == tileImages[targetingMode])
            image.sprite = tileImages[0];
    }

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

        return tiles;
    }

    Dictionary<Tile, int> currDistances;
    public HashSet<Tile> FindTilesInRange(Tile sourceTile, int range)
    {
        Tile currTile;
        Tile peekTile;
        int currRange;
        currDistances = new Dictionary<Tile, int>();
        Queue<Tile> tilesToTrack = new Queue<Tile>();
        Queue<int> tilesToTrackDistances = new Queue<int>();
        tilesToTrack.Enqueue(sourceTile);
        tilesToTrackDistances.Enqueue(range);

        while (tilesToTrack.Count != 0)
        {
            currTile = tilesToTrack.Dequeue();
            currRange = tilesToTrackDistances.Dequeue();

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
        foreach (KeyValuePair<Tile, int> pair in currDistances)
        {
            tilesToHighlight.Add(pair.Key);
        }
        return tilesToHighlight;
    }
    public void TrackTile(Tile peekTile, Queue<Tile> tilesToTrack, Queue<int> distances, int currRange)
    {
        if (!currDistances.ContainsKey(peekTile) && currRange - peekTile.moveWeight >= 0)
        {
            tilesToTrack.Enqueue(peekTile);
            distances.Enqueue(currRange - peekTile.moveWeight);
            currDistances.Add(peekTile, currRange - peekTile.moveWeight);
        }
        else if (currDistances.ContainsKey(peekTile) && currDistances[peekTile] < currRange - peekTile.moveWeight)
        {
            tilesToTrack.Enqueue(peekTile);
            distances.Enqueue(currRange - peekTile.moveWeight);
            currDistances[peekTile] = currRange - peekTile.moveWeight;
        }
    }
}
