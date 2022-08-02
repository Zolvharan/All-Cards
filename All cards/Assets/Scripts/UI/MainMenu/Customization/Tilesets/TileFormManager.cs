using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TileFormManager : CreationForm
{
    public TilesetsMainManager tilesetsMainManager;
    public CharacterImageForm imageSelectionManager;
    public SaveTileManager saveTileManager;

    public TilesetFormManager tilesetFormManager;
    public LandformFormManager landformFormManager;

    public InputField nameField;
    public Slider moveWeightSlider;
    public Toggle impassable;
    public Text moveWeightText;
    public GameObject loadApplyComponents;

    public Image tileImage;
    byte[] tileData;
    const string TILE_PATH = ".\\SavedData\\Tiles";

    int isEditingSaveIndex;

    GameObject otherForm;
    bool isInOtherForm;
    TileData currTile;

    // Value is divided by 10
    const int DEFAULT_WEIGHT = 10;

    public void InitForm(bool isEditing, GameObject currForm = null, TileData tileToEdit = null, int newIsEditingSaveIndex = -1)
    {
        currTile = null;
        otherForm = null;
        isInOtherForm = false;
        if (currForm != null)
        {
            otherForm = currForm;
            isInOtherForm = true;
        }

        // Load and apply buttons only active when accessing from another form
        loadApplyComponents.SetActive(isInOtherForm);

        isEditingSaveIndex = newIsEditingSaveIndex;

        if (isEditing)
        {
            // load attributes
            TileData currTile = tileToEdit;
            nameField.text = currTile.GetName();
            moveWeightSlider.value = currTile.GetMoveWeight() * DEFAULT_WEIGHT;
            moveWeightText.text = (moveWeightSlider.value / DEFAULT_WEIGHT).ToString();
            impassable.isOn = currTile.GetImpassable();
            tileData = currTile.GetImage();
            tileImage.sprite = CharacterImageForm.ConstructImage(tileData);
        }
        else
        {
            impassable.isOn = false;
            moveWeightSlider.value = DEFAULT_WEIGHT;
            moveWeightText.text = (moveWeightSlider.value / 10).ToString();
            nameField.text = "";
            // Init image
            tileData = GetPlaceholderImage();
            tileImage.sprite = CharacterImageForm.ConstructImage(tileData);
        }
    }

    // Save changes used by ??
    public void ExitForm(bool saveChanges)
    {
        if (saveChanges && isInOtherForm)
            currTile = ConstructTile();
        if (isInOtherForm)
            otherForm.SetActive(true);
        else
            tilesetsMainManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public static TileData GetDefaultTile()
    {
        return new TileData("Default", 1, false, GetPlaceholderImage());
    }
    static byte[] GetPlaceholderImage()
    {
        return File.ReadAllBytes(".\\SavedData\\PlaceholderTile.png");
    }
    public void OpenImages()
    {
        imageSelectionManager.gameObject.SetActive(true);
        imageSelectionManager.InitDisplay(TILE_PATH, this);
        this.gameObject.SetActive(false);
    }
    public override void SetImage(bool saveChanges, Sprite newSprite, byte[] newData)
    {
        this.gameObject.SetActive(true);

        if (saveChanges)
        {
            tileImage.sprite = newSprite;
            tileData = newData;
        }
    }

    public void ToggleImpassable()
    {
        // Slider is irrelevant when impassable
        moveWeightSlider.interactable = !impassable.isOn;
    }
    public void ChangedWeight()
    {
        moveWeightText.text = (moveWeightSlider.value / 10).ToString();
    }

    // Save manager methods
    public void OpenSaveTileManager(bool isLoading)
    {
        saveTileManager.gameObject.SetActive(true);

        List<string> tileNames = new List<string>();
        foreach (TileData tile in SaveData.GetTiles())
        {
            tileNames.Add(tile.GetName());
        }
        saveTileManager.InitDisplay(tileNames, isLoading, isEditingSaveIndex);
        this.gameObject.SetActive(false);
    }
    public void LoadTileData()
    {
        // Set the values so that when the form exits it builds the loaded tile
        InitForm(true, otherForm, SaveData.GetTiles()[saveTileManager.GetLoadValue()], -1);
        saveTileManager.gameObject.SetActive(false);
        ExitForm(true);
    }
    public void SaveTileData()
    {
        SaveData.SaveTile(ConstructTile(), saveTileManager.GetSaveValue() == 0 ? -1 : saveTileManager.GetSaveValue() - 1);

        tilesetsMainManager.RefreshTiles();

        // Keep form open if in other form, return to front otherwise
        if (isInOtherForm)
            this.gameObject.SetActive(true);
        else
            tilesetsMainManager.gameObject.SetActive(true);
        saveTileManager.gameObject.SetActive(false);
    }
    public void CancelTileSave()
    {
        this.gameObject.SetActive(true);
        saveTileManager.gameObject.SetActive(false);
    }
    TileData ConstructTile()
    {
        return new TileData(nameField.text, moveWeightSlider.value / 10, impassable.isOn, tileData);
    }
    public TileData GetTile()
    {
        return currTile;
    }
}
