using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PlaceUnitsDisplay : MonoBehaviour
{
    public BattleForm battleForm;
    public Camera playerCam;

    public Image selectedImage;
    public Text selectedLabel;
    public Button generate;
    public RectTransform characterViewport;
    public GameObject characterButtonPrefab;
    public Transform baseEnemyPrefab;

    List<UnitData> enemies;
    List<GameObject> characterButtons;
    GameObject selectedUnit;

    // Horizontal distance between 2 buttons
    const int H_OFFSET = 90;
    // Vertical distance between 2 buttons
    const int V_OFFSET = 90;
    // Distance from view border
    const int MARGIN = 5;

    public Transform tilePrefab;
    Tile[][] tiles;
    Tile currTile;
    Enemy currEnemy;
    GameObject currTileEnemy;
    ExteriorTilesetData currData;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Inputs.move) && selectedUnit != null)
        {
            Vector3 mousePos = playerCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);
            // Do nothing if UI is clicked
            if (rayHit.collider != null && rayHit.collider.gameObject.layer != 5 && rayHit.collider.GetComponent<Tile>() != null)
            {
                currTile = rayHit.collider.GetComponent<Tile>();
                if (selectedUnit != null)
                {
                    if (currTile.currUnit != null)
                    {
                        currTileEnemy = currTile.currUnit.gameObject;
                        currTile.ClearUnit(false);
                        Destroy(currTileEnemy);
                    }
                    // Player location, 0 is clear
                    if (characterButtons.IndexOf(selectedUnit) == 1)
                    {
                        Transform newEnemy = Instantiate(baseEnemyPrefab);
                        GetPlayerLocation().ConstructUnit(newEnemy.GetComponent<Enemy>(), true);
                        currTile.PlaceUnit(newEnemy.GetComponent<Enemy>());
                    }
                    // Enemy
                    else if (characterButtons.IndexOf(selectedUnit) > 1)
                    {
                        Transform newEnemy = Instantiate(baseEnemyPrefab);
                        enemies[characterButtons.IndexOf(selectedUnit) - 2].ConstructUnit(newEnemy.GetComponent<Enemy>(), false);
                        currTile.PlaceUnit(newEnemy.GetComponent<Enemy>());
                    }
                }
            }
        }
    }

    UnitData GetPlayerLocation()
    {
        return new UnitData("", null, File.ReadAllBytes(".\\SavedData\\PlayerLocation.png"), File.ReadAllBytes(".\\SavedData\\PlayerLocation.png"), null, false, 0);
    }

    public void OpenDisplay(ExteriorTilesetData tilesetData, UnitData[] newEnemies)
    {
        currData = tilesetData;
        generate.gameObject.SetActive(true);
        tiles = LevelGenerator.GenerateExteriorTileset(tilesetData, tilePrefab, this.transform);
        FillTileBar(newEnemies);
    }
    public void OpenDisplay(MapEditorData tilesetData, UnitData[] newEnemies)
    {
        generate.gameObject.SetActive(false);
        tiles = LevelGenerator.GenerateMapTileset(tilesetData, tilePrefab, this.transform);
        FillTileBar(newEnemies);
    }

    void FillTileBar(UnitData[] newEnemies)
    {
        enemies = new List<UnitData>();
        enemies.AddRange(newEnemies);
        selectedLabel.text = "Clear";

        characterButtons = new List<GameObject>();

        // Create buttons
        GameObject newButton = Instantiate(characterButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, characterViewport);
        newButton.transform.localPosition = new Vector3((H_OFFSET * (0 % 2)) + MARGIN, -(0 / 2 * (V_OFFSET + MARGIN) + MARGIN), 0);
        newButton.SetActive(true);
        newButton.GetComponent<Image>().sprite = CharacterImageForm.ConstructImage(File.ReadAllBytes(".\\SavedData\\PlaceholderTile.png"));
        characterButtons.Add(newButton);
        newButton = Instantiate(characterButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, characterViewport);
        newButton.transform.localPosition = new Vector3((H_OFFSET * (1 % 2)) + MARGIN, -(1 / 2 * (V_OFFSET + MARGIN) + MARGIN), 0);
        newButton.SetActive(true);
        newButton.GetComponent<Image>().sprite = CharacterImageForm.ConstructImage(File.ReadAllBytes(".\\SavedData\\PlayerLocation.png"));
        characterButtons.Add(newButton);
        for (int i = 2; i < enemies.Count + 2; i++)
        {
            newButton = Instantiate(characterButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, characterViewport);
            // Position is left for first button, and every other button after
            newButton.transform.localPosition = new Vector3((H_OFFSET * (i % 2)) + MARGIN, -(i / 2 * (V_OFFSET + MARGIN) + MARGIN), 0);
            newButton.SetActive(true);
            newButton.GetComponent<Image>().sprite = CharacterImageForm.ConstructImage(enemies[i - 2].portrait);
            characterButtons.Add(newButton);
        }
        // Set viewport size
        characterViewport.sizeDelta = new Vector2(characterViewport.sizeDelta.x, (enemies.Count + 2) / 2 * (110 + MARGIN) + MARGIN);
        if (characterViewport.sizeDelta.y < 400)
            characterViewport.sizeDelta = new Vector2(characterViewport.sizeDelta.x, 400);
    }
    public void SelectCharacter(GameObject button)
    {
        selectedUnit = button;
        selectedImage.sprite = button.GetComponent<Image>().sprite;
        if (characterButtons.IndexOf(button) == 0)
            selectedLabel.text = "Clear";
        else if (characterButtons.IndexOf(button) == 1)
            selectedLabel.text = "Player";
        else
            selectedLabel.text = enemies[characterButtons.IndexOf(button) - 2].GetName();
    }

    public void GenerateNew()
    {
        foreach (Tile[] tilearray in tiles)
        {
            foreach (Tile tile in tilearray)
            {
                if (tile.currUnit != null)
                    Destroy(tile.currUnit.gameObject);
                Destroy(tile.gameObject);
            }
        }
        tiles = LevelGenerator.GenerateExteriorTileset(currData, tilePrefab, this.transform);
    }

    public void ReturnToBattleForm(bool saveChanges)
    {
        if (saveChanges)
        {

        }

        foreach (Tile[] tilearray in tiles)
        {
            foreach (Tile tile in tilearray)
            {
                if (tile.currUnit != null)
                    Destroy(tile.currUnit.gameObject);
                Destroy(tile.gameObject);
            }
        }

        battleForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
