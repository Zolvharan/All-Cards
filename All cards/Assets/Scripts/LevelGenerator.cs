using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public PlayerControl playerControl;

    public Tile[][] tiles;
    public int length;
    public int height;

    Transform tempTile;
    public Transform tilePrefab;
    public Sprite tileImage;
    public Sprite tileSelectedImage;
    public Sprite attackingImage;
    public Sprite abilityImage;
    public BUIManager UI;

    public List<CharacterStats> characters;
    public CharacterStats[] enemies;

    public TilePrefab[] tileSet;

    public void GenerateLevel(int horizontalSize, int verticalSize, List<CharacterStats> newCharacters)
    {
        characters = newCharacters;

        length = horizontalSize;
        height = verticalSize;
        tiles = new Tile[horizontalSize][];

        int i;
        int j;
        for (i = 0; i < horizontalSize; i++)
        {
            tiles[i] = new Tile[verticalSize];
            for (j = 0; j < verticalSize; j++)
            {
                tempTile = Instantiate(tilePrefab, new Vector3(i, j, 0), transform.rotation);
                tempTile.GetComponent<Tile>().UI = UI;
                tiles[i][j] = tempTile.GetComponent<Tile>();
                tiles[i][j].CreateTile(tileSet[Random.Range(0, tileSet.Length)], i, j);
                tiles[i][j].map = this;
            }
        }

        // temp code
        for (i = 0; i < characters.Count; i++)
        {
            characters[i].playerControl = playerControl;
            tiles[0][i].PlaceUnit(characters[i]);
        }
        playerControl.units = characters;

        tiles[horizontalSize - 1][verticalSize - 1].PlaceUnit(enemies[0]);
        tiles[horizontalSize - 2][verticalSize - 3].PlaceUnit(enemies[1]);
        tiles[horizontalSize - 4][verticalSize - 2].PlaceUnit(enemies[2]);
    }
}