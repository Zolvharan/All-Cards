using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public PlayerControl playerControl;
    public EnemyControl enemyControl;
    public UnitPlacementUI unitPlacement;
    public BUIManager battleUI; 

    public Tile[][] tiles;
    public int length;
    public int height;

    Transform tempTile;
    public Transform tilePrefab;

    public List<CharacterStats> characters;
    public List<Enemy> enemies;

    //System.Random random;

    public void GenerateMapLevel(List<CharacterStats> newCharacters, List<Enemy> newEnemies, MapEditorData tileset)
    {
        length = tileset.GetLength();
        height = tileset.GetHeight();
        tiles = GenerateMapTileset(tileset, tilePrefab, this.transform);
        foreach (Tile[] tilearray in tiles)
        {
            foreach (Tile tile in tilearray)
            {
                tile.UI = battleUI;
                tile.map = this;
            }
        }

        SetUnits(newCharacters, newEnemies);
    }
    public void GenerateExteriorLevel(List<CharacterStats> newCharacters, List<Enemy> newEnemies, ExteriorTilesetData tileset)
    {
        length = tileset.GetLength();
        height = tileset.GetHeight();
        tiles = GenerateExteriorTileset(tileset, tilePrefab, this.transform);
        foreach (Tile[] tilearray in tiles)
        {
            foreach (Tile tile in tilearray)
            {
                tile.UI = battleUI;
                tile.map = this;
            }
        }

        SetUnits(newCharacters, newEnemies);
    }
    void SetUnits(List<CharacterStats> newCharacters, List<Enemy> newEnemies)
    {
        characters = newCharacters;
        enemies = newEnemies;

        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].playerControl = playerControl;
        }
        playerControl.units = characters;

        enemyControl.SetEnemies(enemies);

        unitPlacement.PlaceUnits(newCharacters, newEnemies);
    }
    public void Finish()
    {
        battleUI.gameObject.SetActive(true);
        playerControl.StartTurn();
    }

    public void DestroyLevel()
    {
        foreach (Tile[] tileArray in tiles)
        {
            foreach (Tile tile in tileArray)
            {
                Destroy(tile.gameObject);
            }
            foreach (CharacterStats character in characters)
            {
                Destroy(character.gameObject);
            }
            foreach (Enemy enemy in enemies)
            {
                Destroy(enemy.gameObject);
            }
        }
    }

    public bool IsWithinBounds(int xPos, int yPos)
    {
        return xPos >= 0 && xPos < length && yPos >= 0 && yPos < height;
    }

    static public Tile[][] GenerateMapTileset(MapEditorData tileset, Transform tilePrefab, Transform origin)
    {
        int length = tileset.GetLength();
        int height = tileset.GetHeight();
        Tile[][] tiles = new Tile[length][];

        int[] tileTypeIndexes = tileset.GetTypeIndexes();
        TileData[] tileTypes = tileset.GetTileTypes();

        int i;
        int j;
        Transform tempTile;
        for (i = 0; i < length; i++)
        {
            tiles[i] = new Tile[height];
            for (j = 0; j < height; j++)
            {
                tempTile = Instantiate(tilePrefab, new Vector3(i, j, 0), origin.rotation);
                tiles[i][j] = tempTile.GetComponent<Tile>();
                tiles[i][j].ConstructTile(tileTypes[tileTypeIndexes[j + (i * tiles[0].Length)]], i, j);
            }
        }

        return tiles;
    }

    static public Tile[][] GenerateExteriorTileset(ExteriorTilesetData tileset, Transform tilePrefab, Transform origin)
    {
        int length = tileset.GetLength();
        int height = tileset.GetHeight();
        Tile[][] tiles = new Tile[length][];

        // Init tiles
        int i;
        int j;
        Transform tempTile;
        for (i = 0; i < length; i++)
        {
            tiles[i] = new Tile[height];
            for (j = 0; j < height; j++)
            {
                tempTile = Instantiate(tilePrefab, new Vector3(i, j, 0), origin.rotation);
                tiles[i][j] = tempTile.GetComponent<Tile>();
            }
        }
        HashSet<Tile> currOccupiedTiles = new HashSet<Tile>();
        System.Random random = new System.Random();

        // Generate landforms
        int xStart;
        int yStart;
        int xLen;
        int yLen;
        int preXLen;
        int preYLen;

        int k;
        int l;
        int randIterations;
        int subRandIterations;
        SubLandformData currSubLandform;
        HashSet<Tile> currLandformTiles;
        foreach (LandformData landform in tileset.GetLandforms())
        {
            randIterations = random.Next(landform.GetMinCount(), landform.GetMaxCount() + 1);
            for (k = 0; k < randIterations; k++)
            {
                currLandformTiles = new HashSet<Tile>();
                xLen = random.Next(landform.GetMinX(), landform.GetMaxX() + 1);
                yLen = random.Next(landform.GetMinY(), landform.GetMaxY() + 1);
                // Size is initially shortened by snakiness (size is uneffected if snakiness is small enough)
                preXLen = xLen;
                preYLen = yLen;
                if ((int)((float)xLen * landform.GetSnakiness()) > 0)
                    xLen = xLen / (int)((float)xLen * landform.GetSnakiness());
                if ((int)((float)yLen * landform.GetSnakiness()) > 0)
                    yLen = yLen / (int)((float)yLen * landform.GetSnakiness());
                // Pick random location for landform, based on variance parameters
                xStart = random.Next((length - xLen - landform.GetVarPosX()) / 2, (length - xLen + landform.GetVarPosX()) / 2 + 1);
                yStart = random.Next((height - yLen - landform.GetVarPosY()) / 2, (height - yLen + landform.GetVarPosY()) / 2 + 1);
                for (i = xStart; i < xStart + xLen; i++)
                {
                    for (j = yStart; j < yStart + yLen; j++)
                    {
                        // Cannot set tile if tile has already been taken by another landform or if tile is not on map
                        if (i >= 0 && i < length && j >= 0 && j < height && !currOccupiedTiles.Contains(tiles[i][j]))
                        {
                            tiles[i][j].ConstructTile(landform.GetTile(), i, j);
                            currLandformTiles.Add(tiles[i][j]);
                        }
                    }
                }
                currDistances = new Dictionary<Tile, int>();
                newLandformTiles = new HashSet<Tile>();
                availableTiles = new HashSet<Tile>();
                // Unoccupied tiles are available
                foreach (Tile[] tileArray in tiles)
                {
                    availableTiles.UnionWith(tileArray);
                }
                availableTiles.ExceptWith(currOccupiedTiles);
                foreach (Tile tile in currLandformTiles)
                {
                    Snek(tile, (preXLen - xLen + preYLen - xLen) / 2, preXLen - xLen, preYLen - xLen, landform.GetSnakiness(), landform.GetTile(), length, height, tiles);
                }
                currLandformTiles.UnionWith(newLandformTiles);

                currSubLandform = landform.GetSubLandform();
                // Overwrite landform tiles with subLandform
                preXLen = xLen;
                preYLen = yLen;
                int preSubXLen;
                int preSubYLen;
                subRandIterations = random.Next(currSubLandform.GetMinCount(), currSubLandform.GetMaxCount() + 1);
                availableTiles.Clear();
                availableTiles.UnionWith(currLandformTiles);
                currLandformTiles.Clear();
                for (l = 0; l < subRandIterations; l++)
                {
                    xLen = random.Next(currSubLandform.GetMinX(), currSubLandform.GetMaxX() + 1);
                    yLen = random.Next(currSubLandform.GetMinY(), currSubLandform.GetMaxY() + 1);
                    preSubXLen = xLen;
                    preSubYLen = yLen;
                    if ((int)((float)xLen * landform.GetSnakiness()) > 0)
                        xLen = xLen / (int)((float)xLen * landform.GetSnakiness());
                    if ((int)((float)yLen * landform.GetSnakiness()) > 0)
                        yLen = yLen / (int)((float)yLen * landform.GetSnakiness());
                    // Pick random location for sub landform within landform area
                    xStart = random.Next(xStart + preXLen / 2 - xLen / 2 - currSubLandform.GetVarX(), xStart + preXLen / 2 - xLen / 2 + currSubLandform.GetVarX() + 1);
                    yStart = random.Next(yStart + preYLen / 2 - yLen / 2 - currSubLandform.GetVarY(), yStart + preYLen / 2 - yLen / 2 + currSubLandform.GetVarY() + 1);
                    for (i = xStart; i < xStart + xLen; i++)
                    {
                        for (j = yStart; j < yStart + yLen; j++)
                        {
                            // Cannot set tile if tile has already been taken by another landform or if tile is not on map
                            if (i >= 0 && i < length && j >= 0 && j < height && availableTiles.Contains(tiles[i][j]))
                            {
                                tiles[i][j].ConstructTile(currSubLandform.GetTile(), i, j);
                                currLandformTiles.Add(tiles[i][j]);
                            }
                        }
                    }

                    currDistances.Clear();
                    newLandformTiles.Clear();
                    foreach (Tile tile in currLandformTiles)
                    {
                        Snek(tile, (preSubXLen - xLen + preSubYLen - xLen) / 2, preSubXLen - xLen, preSubYLen - xLen, currSubLandform.GetSnakiness(), currSubLandform.GetTile(), length, height, tiles);
                    }
                    currLandformTiles.UnionWith(newLandformTiles);
                }

                // Put occupied tiles in finishedTiles
                currOccupiedTiles.UnionWith(availableTiles);
                currOccupiedTiles.UnionWith(currLandformTiles);
            }
        }

        // Fill in remaining tiles
        float[] weights = tileset.GetTileWeights();
        float range = 0;
        int randIndex;
        foreach (float weight in weights)
        {
            range += weight;
        }

        for (i = 0; i < length; i++)
        {
            for (j = 0; j < height; j++)
            {
                if (!currOccupiedTiles.Contains(tiles[i][j]))
                {
                    // Get random index
                    randIndex = 0;
                    float randFloat = (float)random.NextDouble() * range;
                    // Subtract weights until number reaches 0, when index reaches correct point
                    foreach (float weight in weights)
                    {
                        randFloat -= weight;
                        if (randFloat <= 0)
                            break;
                        else
                            randIndex++;
                    }

                    tiles[i][j].ConstructTile(tileset.GetTiles()[randIndex], i, j);
                }
            }
        }

        return tiles;
    }
    static Dictionary<Tile, int> currDistances;
    static HashSet<Tile> newLandformTiles;
    // For sublandform snakiness
    static HashSet<Tile> availableTiles;
    // Take given tiles and snake them out
    static void Snek(Tile currTile, int maxLen, int currXLen, int currYLen, float currSnakiness, TileData tileData, int length, int height, Tile[][] tiles)
    {
        System.Random random = new System.Random();
        Tile nextTile;
        // If adjacent tile is on screen, expand to tile based on random probability of snakiness and if size is not reached
        if (maxLen > 0)
        {
            if (currXLen > 0)
            {
                if (random.NextDouble() <= (double)currSnakiness && currTile.xPos - 1 >= 0 && currTile.xPos - 1 < length && currTile.yPos >= 0 && currTile.yPos < height)
                {
                    nextTile = tiles[currTile.xPos - 1][currTile.yPos];
                    if (availableTiles.Contains(nextTile) && (!currDistances.ContainsKey(nextTile) || currDistances[nextTile] < maxLen - 1))
                    {
                        nextTile.ConstructTile(tileData, currTile.xPos - 1, currTile.yPos);
                        newLandformTiles.Add(nextTile);
                        currDistances[nextTile] = maxLen - 1;
                        Snek(nextTile, maxLen - 1, currXLen - 1, currYLen, currSnakiness, tileData, length, height, tiles);
                    }
                }
                if (random.NextDouble() <= (double)currSnakiness && currTile.xPos + 1 >= 0 && currTile.xPos + 1 < length && currTile.yPos >= 0 && currTile.yPos < height)
                {
                    nextTile = tiles[currTile.xPos + 1][currTile.yPos];
                    if (availableTiles.Contains(nextTile) && (!currDistances.ContainsKey(nextTile) || currDistances[nextTile] < maxLen - 1))
                    {
                        nextTile.ConstructTile(tileData, currTile.xPos + 1, currTile.yPos);
                        newLandformTiles.Add(nextTile);
                        currDistances[nextTile] = maxLen - 1;
                        Snek(nextTile, maxLen - 1, currXLen - 1, currYLen, currSnakiness, tileData, length, height, tiles);
                    }
                }
            }
            if (currYLen > 0)
            {
                if (random.NextDouble() <= (double)currSnakiness && currTile.xPos >= 0 && currTile.xPos < length && currTile.yPos - 1 >= 0 && currTile.yPos - 1 < height)
                {
                    nextTile = tiles[currTile.xPos][currTile.yPos - 1];
                    if (availableTiles.Contains(nextTile) && (!currDistances.ContainsKey(nextTile) || currDistances[nextTile] < maxLen - 1))
                    {
                        nextTile.ConstructTile(tileData, currTile.xPos, currTile.yPos - 1);
                        newLandformTiles.Add(nextTile);
                        currDistances[nextTile] = maxLen - 1;
                        Snek(nextTile, maxLen - 1, currXLen, currYLen - 1, currSnakiness, tileData, length, height, tiles);
                    }
                }
                if (random.NextDouble() <= (double)currSnakiness && currTile.xPos >= 0 && currTile.xPos < length && currTile.yPos + 1 >= 0 && currTile.yPos + 1 < height)
                {
                    nextTile = tiles[currTile.xPos][currTile.yPos + 1];
                    if (availableTiles.Contains(nextTile) && (!currDistances.ContainsKey(nextTile) || currDistances[nextTile] < maxLen - 1))
                    {
                        nextTile.ConstructTile(tileData, currTile.xPos, currTile.yPos + 1);
                        newLandformTiles.Add(nextTile);
                        currDistances[nextTile] = maxLen - 1;
                        Snek(nextTile, maxLen - 1, currXLen, currYLen - 1, currSnakiness, tileData, length, height, tiles);
                    }
                }
            }
        }
    }
}