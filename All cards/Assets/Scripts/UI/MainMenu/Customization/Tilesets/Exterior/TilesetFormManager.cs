using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TilesetFormManager : MonoBehaviour
{
    public TilesetsMainManager tilesetsMainManager;
    public SaveTileManager saveTilesetManager;
    public TileFormManager tileFormManager;

    public InputField nameField;

    public Text tileText;
    public Image tileSprite;
    public Text tileWeightText;
    public Slider tileWeightSlider;

    public Text landformText;
    public Image landformSprite;
    public Slider landformMinSlider;
    public Slider landformMaxSlider;
    public Text minLandformText;
    public Text maxLandformText;
    public Slider landformXSlider;
    public Slider landformYSlider;
    public Text landformXNum;
    public Text landformYNum;

    public Slider tileScroll;
    public Slider landformScroll;

    public Slider lengthSlider;
    public Text lengthTextNum;
    public Slider heightSlider;
    public Text heightTextNum;

    List<float> tileWeights;
    List<TileData> tiles;

    List<LandformData> landforms;
    // Temp lists used to save tileset data
    List<int> minLandforms;
    List<int> maxLandforms;
    List<int> xPosLandforms;
    List<int> yPosLandforms;

    int isEditingSaveIndex;

    const string newTileName = "New Tile";
    const string newTilePath = ".\\SavedData\\NewTile.png";
    const string newLandformName = "New Landform";

    bool isInTiles;

    void OnEnable()
    {
        if (isInTiles && tileFormManager.GetTile() != null)
        {
            SetTile(tileFormManager.GetTile());
            isInTiles = false;
        }
    }
    public void OpenTileForm()
    {
        this.gameObject.SetActive(false);
        isInTiles = true;
        tileFormManager.gameObject.SetActive(true);
        tileFormManager.InitForm(true, this.gameObject, GetTile());
    }

    public void InitForm(bool isEditing, ExteriorTilesetData editingTileset = null, int newIsEditingSaveIndex = -1)
    {
        isEditingSaveIndex = isEditing ? newIsEditingSaveIndex : -1;

        tiles = new List<TileData>();
        tileWeights = new List<float>();
        landforms = new List<LandformData>();
        minLandforms = new List<int>();
        maxLandforms = new List<int>();
        xPosLandforms = new List<int>();
        yPosLandforms = new List<int>();

        if (isEditing)
        {
            nameField.text = editingTileset.GetName();
            tiles.AddRange(editingTileset.GetTiles());
            tileWeights.AddRange(editingTileset.GetTileWeights());
            tileScroll.maxValue = tiles.Count;
            tileScroll.value = 0;
            // Set weight to 0 if no weights exist
            tileWeightSlider.value = tileScroll.maxValue != 0 ? tileWeights[0] : 0;

            landforms.AddRange(editingTileset.GetLandforms());
            foreach (LandformData landform in landforms)
            {
                minLandforms.Add(landform.GetMinCount());
                maxLandforms.Add(landform.GetMaxCount());
                xPosLandforms.Add(landform.GetVarPosX());
                yPosLandforms.Add(landform.GetVarPosY());
            }
            landformScroll.maxValue = landforms.Count;
            landformScroll.value = 0;
            // Set to 0 if no landform exists
            if (landformScroll.maxValue != 0)
            {
                landformMinSlider.value = editingTileset.GetLandforms()[0].GetMinCount();
                landformMaxSlider.value = editingTileset.GetLandforms()[0].GetMaxCount();
                landformXSlider.value = editingTileset.GetLandforms()[0].GetVarPosX();
                landformYSlider.value = editingTileset.GetLandforms()[0].GetVarPosY();
            }
            else
            {
                landformMinSlider.value = 0;
                landformMaxSlider.value = 0;
                landformXSlider.value = 0;
                landformYSlider.value = 0;
            }

            lengthSlider.value = editingTileset.GetLength();
            heightSlider.value = editingTileset.GetHeight();
            SizeScrollChange(true);
            SizeScrollChange(false);
        }
        else
        {
            nameField.text = "";
            tileScroll.maxValue = 0;
            landformScroll.maxValue = 0;
            tileScroll.value = 0;
            landformScroll.value = 0;
            tileWeightSlider.value = 0;
            landformMinSlider.value = 0;
            landformMaxSlider.value = 0;
            lengthSlider.value = lengthSlider.minValue;
            heightSlider.value = heightSlider.minValue;
            SizeScrollChange(true);
            SizeScrollChange(false);
        }

        // Init displays
        TileScrollChange();
        LandformScrollChange();
    }

    public void ExitForm()
    {
        tilesetsMainManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    ExteriorTilesetData ConstructTileset()
    {
        // Add default tile if no tiles exist
        if (tiles.Count == 0)
            SetTile(TileFormManager.GetDefaultTile());

        // Convert lists
        TileData[] tilesToSave = new TileData[tiles.Count];
        tiles.CopyTo(tilesToSave, 0);
        float[] weightsToSave = new float[tileWeights.Count];
        tileWeights.CopyTo(weightsToSave, 0);
        LandformData[] landformsToSave = new LandformData[landforms.Count];
        landforms.CopyTo(landformsToSave, 0);

        for (int i = 0; i < landforms.Count; i++)
        {
            landforms[i].SetTilesetData(xPosLandforms[i], yPosLandforms[i], minLandforms[i], maxLandforms[i]);
        }

        return new ExteriorTilesetData(nameField.text, landformsToSave, weightsToSave, tilesToSave, (int)lengthSlider.value, (int)heightSlider.value);
    }

    public void TileWeightScrollChange()
    {
        // Cannot save to end of list
        if (tileScroll.value != tiles.Count)
            tileWeights[(int)tileScroll.value] = tileWeightSlider.value;
        tileWeightText.text = tileWeightSlider.value.ToString();
    }
    // Min, max count, X, Y pos
    public void LandformSliderChange(int sliderIndex)
    {
        switch (sliderIndex)
        {
            case 0:
                minLandformText.text = landformMinSlider.value.ToString();
                // Set curr landform tileset data
                if (landformScroll.value != landformScroll.maxValue)
                    minLandforms[(int)landformScroll.value] = (int)landformMinSlider.value;
                break;
            case 1:
                maxLandformText.text = landformMaxSlider.value.ToString();
                if (landformScroll.value != landformScroll.maxValue)
                    maxLandforms[(int)landformScroll.value] = (int)landformMaxSlider.value;
                landformMinSlider.maxValue = landformMaxSlider.value;
                break;
            case 2:
                landformXNum.text = landformXSlider.value.ToString();
                if (landformScroll.value != landformScroll.maxValue)
                    xPosLandforms[(int)landformScroll.value] = (int)landformXSlider.value;
                break;
            case 3:
                landformYNum.text = landformYSlider.value.ToString();
                if (landformScroll.value != landformScroll.maxValue)
                    yPosLandforms[(int)landformScroll.value] = (int)landformYSlider.value;
                break;
        }
    }
    public void SizeScrollChange(bool isLength)
    {
        if (isLength)
        {
            lengthTextNum.text = lengthSlider.value.ToString();
            landformXSlider.maxValue = lengthSlider.value;
        }
        else
        {
            heightTextNum.text = heightSlider.value.ToString();
            landformYSlider.maxValue = heightSlider.value;
        }
    }

    public void TileScrollChange()
    {
        // Set to new tile
        if (tileScroll.value == tiles.Count)
        {
            tileSprite.sprite = CharacterImageForm.ConstructImage(File.ReadAllBytes(newTilePath));
            tileText.text = newTileName;
            // Set slider
            tileWeightSlider.value = 0;
        }
        else
        {
            tileSprite.sprite = CharacterImageForm.ConstructImage(tiles[(int)tileScroll.value].GetImage());
            tileText.text = tiles[(int)tileScroll.value].GetName();
            // Set slider
            tileWeightSlider.value = tileWeights[(int)tileScroll.value];
        }
    }
    public void LandformScrollChange()
    {
        // Set to new landform
        if (landformScroll.value == landforms.Count)
        {
            landformSprite.sprite = CharacterImageForm.ConstructImage(File.ReadAllBytes(newTilePath));
            landformText.text = newLandformName;
            // Set sliders
            landformMinSlider.value = 0;
            landformMaxSlider.value = 0;
            landformXSlider.value = 0;
            landformYSlider.value = 0;
        }
        else
        {
            landformSprite.sprite = CharacterImageForm.ConstructImage(landforms[(int)landformScroll.value].GetTile().GetImage());
            landformText.text = landforms[(int)landformScroll.value].GetName();
            // Save and set sliders
            landformMinSlider.value = minLandforms[(int)landformScroll.value];
            landformMaxSlider.value = maxLandforms[(int)landformScroll.value];
            landformXSlider.value = xPosLandforms[(int)landformScroll.value];
            landformYSlider.value = yPosLandforms[(int)landformScroll.value];
        }
    }

    public void RemoveTile(bool update)
    {
        // Cannot remove new tile select
        if ((int)tileScroll.value != tiles.Count)
        {
            tiles.RemoveAt((int)tileScroll.value);
            tileWeights.RemoveAt((int)tileScroll.value);
            tileScroll.maxValue--;
            if (update)
                TileScrollChange();
        }
    }
    public void RemoveLandform(bool update)
    {
        // Cannot remove new landform select
        if ((int)landformScroll.value != landforms.Count)
        {
            landforms.RemoveAt((int)landformScroll.value);
            landformScroll.maxValue--;
            if (update)
                LandformScrollChange();
        }
    }

    public void SetTile(TileData newTile)
    {
        // If new, add, else replace
        if ((int)tileScroll.value != tiles.Count)
            RemoveTile(false);
        tileScroll.maxValue++;
        tiles.Insert((int)tileScroll.value, newTile);
        tileWeights.Insert((int)tileScroll.value, tileWeightSlider.value);
        // Refresh display
        TileScrollChange();
    }
    public void SetLandform(LandformData newLandform)
    {
        // If new, add, else replace
        if ((int)landformScroll.value != landforms.Count)
            RemoveLandform(false);
        else
        {
            xPosLandforms.Insert((int)landformScroll.value, (int)landformXSlider.value);
            yPosLandforms.Insert((int)landformScroll.value, (int)landformYSlider.value);
            minLandforms.Insert((int)landformScroll.value, (int)landformMinSlider.value);
            maxLandforms.Insert((int)landformScroll.value, (int)landformMaxSlider.value);
        }
        landformScroll.maxValue++;
        newLandform.SetTilesetData((int)landformXSlider.value, (int)landformYSlider.value, (int)landformMinSlider.value, (int)landformMaxSlider.value);

        landforms.Insert((int)landformScroll.value, newLandform);
        // Refresh display
        LandformScrollChange();
    }

    public TileData GetTile()
    {
        return (int)tileScroll.value == tiles.Count ? TileFormManager.GetDefaultTile() : tiles[(int)tileScroll.value];
    }
    public LandformData GetLandform()
    {
        return (int)landformScroll.value == landforms.Count ? null : landforms[(int)landformScroll.value];
    }


    // Save manager methods
    public void OpenSaveTilesetManager()
    {
        saveTilesetManager.gameObject.SetActive(true);

        List<string> tilesetNames = new List<string>();
        foreach (ExteriorTilesetData tileset in SaveData.GetETilesets())
        {
            tilesetNames.Add(tileset.GetName());
        }
        saveTilesetManager.InitDisplay(tilesetNames, false, isEditingSaveIndex);
        this.gameObject.SetActive(false);
    }
    public void SaveTilesetData()
    {
        SaveData.SaveETileset(ConstructTileset(), saveTilesetManager.GetSaveValue() == 0 ? -1 : saveTilesetManager.GetSaveValue() - 1);

        tilesetsMainManager.RefreshTilesets();

        // Return to front
        tilesetsMainManager.gameObject.SetActive(true);
        saveTilesetManager.gameObject.SetActive(false);
    }
    public void CancelTilesetSave()
    {
        this.gameObject.SetActive(true);
        saveTilesetManager.gameObject.SetActive(false);
    }
}
