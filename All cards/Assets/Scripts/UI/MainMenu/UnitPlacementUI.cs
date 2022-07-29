using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPlacementUI : MonoBehaviour
{
    public LevelGenerator levelGenerator;
    CharacterStats selectedUnit;
    public Image selectedImage;
    public Camera playerCam;
    public Button finishButton;

    public GameObject characterButtonPrefab;
    public GameObject enemyButtonPrefab;
    public Transform buttonParent;
    public RectTransform characterViewport;
    List<GameObject> characterButtons;
    public RectTransform enemyViewport;
    List<GameObject> enemyButtons;

    bool allPlaced;
    List<CharacterStats> characters;
    List<Enemy> enemies;

    // Horizontal distance between 2 buttons
    const int H_OFFSET = 90;
    // Vertical distance between 2 buttons
    const int V_OFFSET = 90;
    // Distance from view border
    const int MARGIN = 5;

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
                if (rayHit.collider.GetComponent<Tile>().occupied)
                {
                    selectedUnit = rayHit.collider.GetComponent<Tile>().currUnit;
                    selectedImage.sprite = rayHit.collider.GetComponent<Tile>().currUnit.portrait;
                }
                else if (selectedUnit != null)
                {
                    if (selectedUnit.currTile != null)
                        selectedUnit.currTile.ClearUnit(false);
                    selectedUnit.gameObject.SetActive(true);
                    rayHit.collider.GetComponent<Tile>().PlaceUnit(selectedUnit);

                    allPlaced = true;
                    foreach (CharacterStats character in characters)
                    {
                        if (character.currTile == null)
                            allPlaced = false;
                    }
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.currTile == null)
                            allPlaced = false;
                    }
                    finishButton.interactable = allPlaced;
                }
            }
        }
    }

    public void PlaceUnits(List<CharacterStats> newCharacters, List<Enemy> newEnemies)
    {
        this.gameObject.SetActive(true);
        characters = newCharacters;
        enemies = newEnemies;
        finishButton.interactable = false;
        characterButtons = new List<GameObject>();
        enemyButtons = new List<GameObject>();

        // Create buttons
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].gameObject.SetActive(false);

            GameObject newButton = Instantiate(characterButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, characterViewport);
            // Position is left for first button, and every other button after
            newButton.transform.localPosition = new Vector3((H_OFFSET * (i % 2)) + MARGIN - 100, -(i / 2 * (V_OFFSET + MARGIN) + MARGIN) + 150, 0);
            newButton.SetActive(true);
            newButton.GetComponent<Image>().sprite = characters[i].portrait;
            characterButtons.Add(newButton);
        }
        // Set viewport size
        characterViewport.sizeDelta = new Vector2(characterViewport.sizeDelta.x, characters.Count / 2 * (V_OFFSET + MARGIN) + MARGIN);
        if (characterViewport.sizeDelta.y < 300)
            characterViewport.sizeDelta = new Vector2(characterViewport.sizeDelta.x, 300);
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].gameObject.SetActive(false);

            GameObject newButton = Instantiate(enemyButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity, enemyViewport);
            newButton.transform.localPosition = new Vector3((H_OFFSET * (i % 2)) + MARGIN - 100, -(i / 2 * (V_OFFSET + MARGIN) + MARGIN) + 150, 0);
            newButton.SetActive(true);
            newButton.GetComponent<Image>().sprite = enemies[i].portrait;
            enemyButtons.Add(newButton);
        }
        enemyViewport.sizeDelta = new Vector2(enemyViewport.sizeDelta.x, enemies.Count / 2 * (V_OFFSET + MARGIN) + MARGIN);
        if (enemyViewport.sizeDelta.y < 300)
            enemyViewport.sizeDelta = new Vector2(enemyViewport.sizeDelta.x, 300);
    }
    public void SelectCharacter(GameObject button)
    {
        selectedUnit = characters[characterButtons.IndexOf(button)];
        selectedImage.sprite = characters[characterButtons.IndexOf(button)].portrait;
    }
    public void SelectEnemy(GameObject button)
    {
        selectedUnit = enemies[enemyButtons.IndexOf(button)];
        selectedImage.sprite = enemies[enemyButtons.IndexOf(button)].portrait;
    }
    public void ResetCharacters()
    {
        foreach (CharacterStats character in characters)
        {
            if (character.currTile != null)
                character.currTile.ClearUnit(false);
        }
        finishButton.interactable = false;
    }
    public void ResetEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.currTile != null)
                enemy.currTile.ClearUnit(false);
        }
        finishButton.interactable = false;
    }
    public void ClearUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.currTile.ClearUnit(false);
            selectedUnit.gameObject.SetActive(false);
            selectedImage.sprite = null;
            selectedUnit = null;
            finishButton.interactable = false;
        }
    }
    public void FinishPlacement()
    {
        foreach (GameObject button in characterButtons)
        {
            Destroy(button);
        }
        foreach (GameObject button in enemyButtons)
        {
            Destroy(button);
        }

        levelGenerator.Finish();
        this.gameObject.SetActive(false);
    }
}
