using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandformFormManager : MonoBehaviour
{
    public TilesetsMainManager tilesetsMainManager;
    public TilesetFormManager tilesetFormManager;
    public SaveTileManager saveLandformManager;
    public TileFormManager tileFormManager;

    public InputField nameField;
    public GameObject loadApplyComponents;
    public GameObject subLandformSelection;
    public GameObject subLandformComponents;

    public Text tileText;
    public Image tileImage;
    TileData currTile;

    public Slider[] sizeSliders;
    public Text[] sizeTextNums;
    // Min X, Min Y, Max X, Max Y
    int[] sizeNums;
    const int MAX_SIZE = 10;
    const int MIN_SIZE = 1;

    public Slider snakinessSlider;
    public Text snakinessNum;

    LandformData currLandform;
    SubLandformData subLandform;

    int[] subNums;
    public Slider[] subLandformSliders;
    public Text[] subSizeTextNums;

    int isEditingSaveIndex;

    bool editing;
    bool isInTileset;

    bool isInTiles;

    void OnEnable()
    {
        if (isInTiles && tileFormManager.GetTile() != null)
        {
            currTile = tileFormManager.GetTile();
            tileText.text = currTile.GetName();
            tileImage.sprite = CharacterData.ConstructImage(currTile.GetImage());
            isInTiles = false;
        }
    }

    public void OpenSubForm()
    {
        // Save landform
        currLandform = ConstructLandform();
        // Cannot save sublandforms
        loadApplyComponents.SetActive(false);

        subLandformComponents.SetActive(true);
        subLandformSelection.SetActive(false);

        sizeSliders[2].value = subLandform.GetMaxX();
        sizeTextNums[2].text = sizeSliders[2].value.ToString();
        sizeSliders[3].value = subLandform.GetMaxY();
        sizeTextNums[3].text = sizeSliders[3].value.ToString();
        sizeSliders[0].value = subLandform.GetMinX();
        sizeTextNums[0].text = sizeSliders[0].value.ToString();
        sizeSliders[1].value = subLandform.GetMinY();
        sizeTextNums[1].text = sizeSliders[1].value.ToString();
        snakinessSlider.value = subLandform.GetSnakiness();
        subLandformSliders[0].value = subLandform.GetVarX();
        subSizeTextNums[0].text = subLandformSliders[0].value.ToString();
        subLandformSliders[1].value = subLandform.GetVarY();
        subSizeTextNums[1].text = subLandformSliders[1].value.ToString();
        subLandformSliders[3].value = subLandform.GetMaxCount();
        subSizeTextNums[3].text = subLandformSliders[3].value.ToString();
        subLandformSliders[2].value = subLandform.GetMinCount();
        subSizeTextNums[2].text = subLandformSliders[2].value.ToString();

        currTile = subLandform.GetTile();
        tileImage.sprite = CharacterData.ConstructImage(currTile.GetImage());
        tileText.text = currTile.GetName();
    }
    public void OpenMainForm(bool saveChanges)
    {
        if (saveChanges)
        {
            // Save subLandform
            subLandform = ConstructSubLandform();
        }

        if (isInTileset)
            loadApplyComponents.SetActive(true);
        subLandformSelection.SetActive(true);
        subLandformComponents.SetActive(false);

        sizeSliders[2].value = currLandform.GetMaxX();
        sizeTextNums[2].text = sizeSliders[2].value.ToString();
        sizeSliders[3].value = currLandform.GetMaxY();
        sizeTextNums[3].text = sizeSliders[3].value.ToString();
        sizeSliders[0].value = currLandform.GetMinX();
        sizeTextNums[0].text = sizeSliders[0].value.ToString();
        sizeSliders[1].value = currLandform.GetMinY();
        sizeTextNums[1].text = sizeSliders[1].value.ToString();
        snakinessSlider.value = currLandform.GetSnakiness();

        currTile = currLandform.GetTile();
        tileImage.sprite = CharacterData.ConstructImage(currTile.GetImage());
        tileText.text = currTile.GetName();
    }

    public void OpenTileForm()
    {
        this.gameObject.SetActive(false);
        isInTiles = true;
        tileFormManager.gameObject.SetActive(true);
        tileFormManager.InitForm(true, this.gameObject, currTile);
    }
    public void OpenFormInTileset()
    {
        isInTileset = true;
        tilesetFormManager.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        if (tilesetFormManager.GetLandform() == null)
            InitForm(false);
        else
            InitForm(true, tilesetFormManager.GetLandform());
    }
    public void OpenForm(bool isEditing, LandformData currLandform = null, int newIsEditingIndex = -1)
    {
        isInTileset = false;
        InitForm(isEditing, currLandform, newIsEditingIndex);
    }
    void InitForm(bool isEditing, LandformData newLandform = null, int newIsEditingIndex = -1)
    {
        isEditingSaveIndex = newIsEditingIndex;
        editing = isEditing;
        // Load and apply buttons only active when in tileset
        loadApplyComponents.SetActive(isInTileset);
        if (isEditing)
        {
            currLandform = newLandform;
            subLandform = currLandform.GetSubLandform();

            subLandform = currLandform.GetSubLandform();
            nameField.text = currLandform.GetName();
            currTile = currLandform.GetTile();
            tileImage.sprite = CharacterData.ConstructImage(currLandform.GetTile().GetImage());
            tileText.text = currLandform.GetTile().GetName();
            sizeNums = new int[4];
            sizeNums[0] = currLandform.GetMinX();
            sizeNums[1] = currLandform.GetMinY();
            sizeNums[2] = currLandform.GetMaxX();
            sizeNums[3] = currLandform.GetMaxY();
            subNums = new int[4];
            subNums[0] = subLandform.GetVarX();
            subNums[1] = subLandform.GetVarY();
            subNums[2] = subLandform.GetMinCount();
            subNums[3] = subLandform.GetMaxCount();
            for (int i = 0; i < sizeNums.Length; i++)
            {
                sizeSliders[i].value = sizeNums[i];
                sizeTextNums[i].text = sizeNums[i].ToString();
            }
            // Min cannot exceed max
            sizeSliders[0].maxValue = sizeSliders[2].value;
            sizeSliders[1].maxValue = sizeSliders[3].value;

            snakinessSlider.value = currLandform.GetSnakiness();
            snakinessNum.text = snakinessSlider.value.ToString();
        }
        else
        {
            subLandform = SubLandformData.ConstructDefault();
            nameField.text = "";
            currTile = TileFormManager.GetDefaultTile();
            tileImage.sprite = CharacterData.ConstructImage(currTile.GetImage());
            tileText.text = currTile.GetName();
            sizeNums = new int[4];
            subNums = new int[4];
            for (int i = 0; i < sizeNums.Length; i++)
            {
                sizeNums[i] = MIN_SIZE;
                subNums[i] = 0;
                sizeSliders[i].value = MIN_SIZE;
                sizeTextNums[i].text = MIN_SIZE.ToString();
            }
            // Min cannot exceed max
            sizeSliders[0].maxValue = sizeSliders[2].value;
            sizeSliders[1].maxValue = sizeSliders[3].value;

            snakinessSlider.value = 0;
            snakinessNum.text = "0";
        }
    }

    // Save changes used by tileset form
    public void ExitForm(bool saveChanges)
    {
        if (saveChanges)
        {
            tilesetFormManager.SetLandform(ConstructLandform());
        }
        if (isInTileset)
            tilesetFormManager.gameObject.SetActive(true);
        else
            tilesetsMainManager.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public TileData GetTile()
    {
        return currTile;
    }

    public void SizeChange(int sliderNum)
    {
        sizeNums[sliderNum] = (int)System.Math.Round(sizeSliders[sliderNum].value);
        sizeTextNums[sliderNum].text = sizeNums[sliderNum].ToString();
        // Min cannot exceed max
        if (sliderNum > 1)
        {
            sizeSliders[sliderNum - 2].maxValue = sizeSliders[sliderNum].value;
        }
    }
    public void SubSizeChange(int sliderNum)
    {
        subNums[sliderNum] = (int)System.Math.Round(subLandformSliders[sliderNum].value);
        subSizeTextNums[sliderNum].text = subNums[sliderNum].ToString();
        // Min cannot exceed max
        if (sliderNum == 3)
        {
            subLandformSliders[2].maxValue = subLandformSliders[3].value;
        }
    }
    public void SnakinessChange()
    {
        snakinessNum.text = snakinessSlider.value.ToString();
    }

    LandformData ConstructLandform()
    {
        // Construct used to return landform and so save for sub creation
            return new LandformData(nameField.text, sizeNums[0], sizeNums[1], sizeNums[2], sizeNums[3], snakinessSlider.value, currTile, subLandform);
    }
    SubLandformData ConstructSubLandform()
    {
        return new SubLandformData(nameField.text, sizeNums[0], sizeNums[1], sizeNums[2], sizeNums[3], snakinessSlider.value, currTile,
            subNums[0], subNums[1], subNums[2], subNums[3]);
    }

    // Save manager methods
    public void OpenSaveLandformManager(bool isLoading)
    {
        saveLandformManager.gameObject.SetActive(true);

        List<string> landformNames = new List<string>();
        foreach (LandformData landform in SaveData.GetLandforms())
        {
            landformNames.Add(landform.GetName());
        }
        saveLandformManager.InitDisplay(landformNames, isLoading, isEditingSaveIndex);
        this.gameObject.SetActive(false);
    }
    public void LoadLandformData()
    {
        // Set the values so that when the form exits it builds the loaded tile
        InitForm(true, SaveData.GetLandforms()[saveLandformManager.GetLoadValue()], -1);
        saveLandformManager.gameObject.SetActive(false);
        ExitForm(true);
    }
    public void SaveLandformData()
    {
        SaveData.SaveLandform(ConstructLandform(), saveLandformManager.GetSaveValue() == 0 ? -1 : saveLandformManager.GetSaveValue() - 1);

        tilesetsMainManager.RefreshLandforms();

        // Keep form open if in other form, return to front otherwise
        if (isInTileset)
            this.gameObject.SetActive(true);
        else
            tilesetsMainManager.gameObject.SetActive(true);
        saveLandformManager.gameObject.SetActive(false);
    }
    public void CancelLandformSave()
    {
        this.gameObject.SetActive(true);
        saveLandformManager.gameObject.SetActive(false);
    }
}
