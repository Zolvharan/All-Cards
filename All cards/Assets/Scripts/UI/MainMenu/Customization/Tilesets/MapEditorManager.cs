using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour
{
    public Camera playerCam;

    public MainMapEditManager frontPage;
    public TileFormManager tileFormManager;
    public SaveTileManager saveTilesetManager;

    public InputField nameField;
    public Slider xSlider;
    public Slider ySlider;
    public Text xNumText;
    public Text yNumText;

    Transform tempTile;
    public Transform tilePrefab;
    public Transform tileParent;
    List<List<Tile>> tiles;

    const int MIN_X = 10;
    const int MIN_Y = 10;

    int currTileIndex;
    TileData currTile;
    List<TileData> currTileData;
    List<GameObject> buttons;

    bool isInTiles;

    public RectTransform viewportContent;
    public Transform buttonParent;
    public GameObject buttonPrefab;
    public Image selectedTileImage;
    // Horizontal distance between 2 buttons
    const int H_OFFSET = 90;
    // Vertical distance between 2 buttons
    const int V_OFFSET = 90;
    // Distance from view border
    const int MARGIN = 5;

    int currEditingIndex;
    int isEditingIndex;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(Inputs.select) && currTileIndex != -1)
        {
            Vector3 mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);
            // Do nothing if UI is clicked
            if (rayHit.collider != null && rayHit.collider.gameObject.layer != 5 && rayHit.collider.GetComponent<Tile>() != null)
            {
                rayHit.collider.GetComponent<Tile>().ConstructTile(currTileData[currTileIndex], rayHit.collider.GetComponent<Tile>().xPos, rayHit.collider.GetComponent<Tile>().yPos);
            }
        }
    }

    public void OpenMapEditor(bool isEditing, MapEditorData mapData = null, int newEditingIndex = -1)
    {
        isEditingIndex = newEditingIndex;
        currTileIndex = -1;
        frontPage.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        currTile = null;

        currTileData = new List<TileData>();
        buttons = new List<GameObject>();
        tiles = new List<List<Tile>>();
        if (isEditing)
        {
            nameField.text = mapData.GetName();
            int j;
            currTileData.AddRange(mapData.GetTileTypes());
            GameObject newButton;
            for (j = 0; j < currTileData.Count; j++)
            {
                newButton = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity, buttonParent);
                newButton.transform.localPosition = new Vector3((H_OFFSET * (j % 2)) + MARGIN, -(j / 2 * (V_OFFSET + MARGIN) + MARGIN), 0);
                newButton.SetActive(true);
                newButton.GetComponent<Image>().sprite = CharacterImageForm.ConstructImage(currTileData[j].GetImage());
                buttons.Add(newButton);
            }

            for (int i = 0; i < mapData.GetLength(); i++)
            {
                tiles.Add(new List<Tile>());
                for (j = 0; j < mapData.GetHeight(); j++)
                {
                    tempTile = Instantiate(tilePrefab, new Vector3(i, j, 0), transform.rotation, tileParent);
                    tempTile.GetComponent<Tile>().ConstructTile(currTileData[mapData.GetTypeIndexes()[j + (i * mapData.GetHeight())]], i, j);
                    tiles[i].Add(tempTile.GetComponent<Tile>());
                }
            }
            xSlider.value = mapData.GetLength();
            ySlider.value = mapData.GetHeight();
        }
        else
        {
            int j;
            for (int i = 0; i < MIN_X; i++)
            {
                tiles.Add(new List<Tile>());
                for (j = 0; j < MIN_Y; j++)
                {
                    tempTile = Instantiate(tilePrefab, new Vector3(i, j, 0), transform.rotation, tileParent);
                    tempTile.GetComponent<Tile>().ConstructTile(TileFormManager.GetDefaultTile(), i, j);
                    tiles[i].Add(tempTile.GetComponent<Tile>());
                }
            }
            xSlider.value = MIN_X;
            ySlider.value = MIN_Y;
        }

        // Set viewport size
        viewportContent.sizeDelta = new Vector2(viewportContent.sizeDelta.x, (currTileData.Count - 1) / 2 * (V_OFFSET + MARGIN) + MARGIN);
        if (viewportContent.sizeDelta.y < 300)
            viewportContent.sizeDelta = new Vector2(viewportContent.sizeDelta.x, 300);
    }
    
    void OnEnable()
    {
        tileParent.gameObject.SetActive(true);
        if (isInTiles && tileFormManager.GetTile() != null)
        {
            isInTiles = false;
            if (currEditingIndex == -1)
            {
                currTileData.Add(tileFormManager.GetTile());
                GameObject newButton = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity, buttonParent);
                // Position is left for first button, and every other button after
                newButton.transform.localPosition = new Vector3((H_OFFSET * ((currTileData.Count - 1) % 2)) + MARGIN, -((currTileData.Count - 1) / 2 * (V_OFFSET + MARGIN) + MARGIN), 0);
                newButton.SetActive(true);
                newButton.GetComponent<Image>().sprite = CharacterImageForm.ConstructImage(currTileData[currTileData.Count - 1].GetImage());
                buttons.Add(newButton);

                // Set viewport size
                viewportContent.sizeDelta = new Vector2(viewportContent.sizeDelta.x, currTileData.Count / 2 * (V_OFFSET + MARGIN) + MARGIN);
                if (viewportContent.sizeDelta.y < 300)
                    viewportContent.sizeDelta = new Vector2(viewportContent.sizeDelta.x, 300);
            }
            else
            {
                currTileData[currTileIndex] = tileFormManager.GetTile();
                buttons[currTileIndex].GetComponent<Image>().sprite = CharacterImageForm.ConstructImage(tileFormManager.GetTile().GetImage());
            }
        }
    }
    public void OpenTileForm(bool isEditing)
    {
        // Cannot edit if no tile is selected
        if (!isEditing || currTileIndex != -1)
        {
            tileParent.gameObject.SetActive(false);
            currEditingIndex = isEditing ? currTileIndex : -1;

            this.gameObject.SetActive(false);
            isInTiles = true;
            tileFormManager.gameObject.SetActive(true);
            tileFormManager.InitForm(isEditing, this.gameObject, currTile);
        }
    }

    public void ReturnToFrontPage()
    {
        foreach (List<Tile> tileList in tiles)
        {
            foreach (Tile tile in tileList)
            {
                Destroy(tile.gameObject);
            }
        }
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        this.gameObject.SetActive(false);
        frontPage.gameObject.SetActive(true);
    }

    public void ButtonClick(GameObject button)
    {
        currTileIndex = buttons.IndexOf(button);
        currTile = currTileData[currTileIndex];
        selectedTileImage.sprite = CharacterImageForm.ConstructImage(currTileData[currTileIndex].GetImage());
    }
    public void RemoveTile()
    {
        if (currTileIndex != -1)
        {
            Destroy(buttons[currTileIndex]);
            buttons.RemoveAt(currTileIndex);
            currTileData.RemoveAt(currTileIndex);
            for (int i = currTileIndex; i < currTileData.Count; i++)
            {
                buttons[i].transform.localPosition = new Vector3((H_OFFSET * (i % 2)) + MARGIN, -(i / 2 * (V_OFFSET + MARGIN) + MARGIN), 0);
            }
            currTileIndex = -1;
        }
    }

    // Updates text nums and changes field size
    public void SliderChange(bool isX)
    {
        int j;
        int currCount;

        if (isX)
        {
            xNumText.text = xSlider.value.ToString();
            if (tiles.Count > xSlider.value)
            {
                currCount = tiles.Count;
                for (int i = 0; i < currCount - xSlider.value; i++)
                {
                    foreach (Tile tile in tiles[tiles.Count - 1])
                    {
                        Destroy(tile.gameObject);
                    }
                    tiles.RemoveAt(tiles.Count - 1);
                }
            }
            else if (tiles.Count < xSlider.value)
            {
                currCount = tiles.Count;
                for (int i = 0; i < xSlider.value - currCount; i++)
                {
                    tiles.Add(new List<Tile>());
                    for (j = 0; j < tiles[0].Count; j++)
                    {
                        tempTile = Instantiate(tilePrefab, new Vector3(tiles.Count - 1, j, 0), transform.rotation, tileParent);
                        tempTile.GetComponent<Tile>().ConstructTile(TileFormManager.GetDefaultTile(), i, j);
                        tiles[tiles.Count - 1].Add(tempTile.GetComponent<Tile>());
                    }
                }
            }
        }
        else
        {
            yNumText.text = ySlider.value.ToString();
            if (tiles[0].Count > ySlider.value)
            {
                currCount = tiles[0].Count;
                for (int i = 0; i < currCount - ySlider.value; i++)
                {
                    foreach (List<Tile> tileArray in tiles)
                    {
                        Destroy(tileArray[tileArray.Count - 1].gameObject);
                        tileArray.RemoveAt(tileArray.Count - 1);
                    }
                }
            }
            else if (tiles.Count < xSlider.value)
            {
                currCount = tiles[0].Count;
                for (int i = 0; i < ySlider.value - currCount; i++)
                {
                    for (j = 0; j < tiles.Count; j++)
                    {
                        tempTile = Instantiate(tilePrefab, new Vector3(j, currCount + i, 0), transform.rotation, tileParent);
                        tempTile.GetComponent<Tile>().ConstructTile(TileFormManager.GetDefaultTile(), j, currCount + i);
                        tiles[j].Add(tempTile.GetComponent<Tile>());
                    }
                }
            }
        }
    }

    MapEditorData ConstructTileset()
    {
        int[][] tileTypeIndexes;
        TileData[] tileTypes;

        HashSet<TileData> tileTypeSet = new HashSet<TileData>();
        // Collect tile types
        foreach (List<Tile> tileList in tiles)
        {
            foreach (Tile tile in tileList)
            {
                tileTypeSet.Add(tile.GetTileData());
            }
        }
        tileTypes = new TileData[tileTypeSet.Count];
        tileTypeSet.CopyTo(tileTypes, 0);
        List<TileData> tileTypeList = new List<TileData>();
        tileTypeList.AddRange(tileTypes);

        tileTypeIndexes = new int[tiles.Count][];
        int j;
        for (int i = 0; i < tiles.Count; i++)
        {
            tileTypeIndexes[i] = new int[tiles[i].Count];
            for (j = 0; j < tiles[i].Count; j++)
            {
                tileTypeIndexes[i][j] = tileTypeList.IndexOf(tiles[i][j].GetTileData());
            }
        }

        MapEditorData newTileset = new MapEditorData(nameField.text, tileTypeIndexes, tileTypes, (int)xSlider.value, (int)ySlider.value);
        return newTileset;
    }

    // Save manager methods
    public void OpenSaveTilesetManager()
    {
        saveTilesetManager.gameObject.SetActive(true);

        List<string> tilesetNames = new List<string>();
        foreach (MapEditorData tileset in SaveData.GetMapTilesets())
        {
            tilesetNames.Add(tileset.GetName());
        }
        saveTilesetManager.InitDisplay(tilesetNames, false, isEditingIndex);
        tileParent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
    public void SaveTilesetData()
    {
        SaveData.SaveMapTileset(ConstructTileset(), saveTilesetManager.GetSaveValue() == 0 ? -1 : saveTilesetManager.GetSaveValue() - 1);

        // Return to front
        frontPage.gameObject.SetActive(true);
        frontPage.RefreshTilesets();
        saveTilesetManager.gameObject.SetActive(false);
    }
    public void CancelTilesetSave()
    {
        this.gameObject.SetActive(true);
        saveTilesetManager.gameObject.SetActive(false);
    }
}
