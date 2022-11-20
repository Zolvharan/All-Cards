using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesetLoadForm : MonoBehaviour
{
    public BattleForm battleForm;

    public Dropdown tileTypeList;
    public Dropdown tilesetList;

    public void OpenForm()
    {
        this.gameObject.SetActive(true);
        tileTypeList.value = 0;
        GetExteriorTilesets();
    }

    public void ReturnToBattleForm(bool loadData)
    {
        if (loadData)
        {
            switch (tileTypeList.value)
            {
                case 0:
                    battleForm.SetTileset(GetExteriorTileset());
                    break;
                case 1:
                    battleForm.SetTileset(GetMapTileset());
                    break;
            }
        }

        battleForm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public ExteriorTilesetData GetExteriorTileset()
    {
        return SaveData.GetETilesets()[tilesetList.value];
    }
    public MapEditorData GetMapTileset()
    {
        return SaveData.GetMapTilesets()[tilesetList.value];
    }
    public void SetTileType()
    {
        switch (tileTypeList.value)
        {
            case 0:
                GetExteriorTilesets();
                break;
            case 1:
                GetMapTilesets();
                break;
        }
    }
    public void GetExteriorTilesets()
    {
        List<ExteriorTilesetData> tilesets = SaveData.GetETilesets();
        List<string> tileNames = new List<string>();
        foreach (ExteriorTilesetData tileset in tilesets)
        {
            tileNames.Add(tileset.GetName());
        }
        tilesetList.ClearOptions();
        tilesetList.AddOptions(tileNames);
    }
    public void GetMapTilesets()
    {
        List<MapEditorData> tilesets = SaveData.GetMapTilesets();
        List<string> tileNames = new List<string>();
        foreach (MapEditorData tileset in tilesets)
        {
            tileNames.Add(tileset.GetName());
        }
        tilesetList.ClearOptions();
        tilesetList.AddOptions(tileNames);
    }
}
