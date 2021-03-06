using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

// Character images manager
public class CIManager : MonoBehaviour
{
    public MMManager mainManager;
    public MCManager charForm;

    public RectTransform viewportContent;
    public Image currImage;
    bool isPortrait;
    public GameObject buttonPrefab;
    public Transform buttonParent;

    List<GameObject> buttons;
    List<byte[]> imageData;
    byte[] currData;

    // Horizontal distance between 2 buttons
    const int H_OFFSET = 90;
    // Vertical distance between 2 buttons
    const int V_OFFSET = 90;
    // Distance from view border
    const int MARGIN = 5;

    const string PORTRAIT_PATH = ".\\SavedData\\Portraits";
    const string BATTLE_SPRITE_PATH = ".\\SavedData\\BattleSprites";

    public void InitDisplay(bool portrait)
    {
        isPortrait = portrait;

        GameObject newButton;
        Texture2D texture;
        Sprite newSprite;
        imageData = new List<byte[]>();
        byte[] data;

        int imageCount = 0;
        buttons = new List<GameObject>();
        string filePath = isPortrait ? PORTRAIT_PATH : BATTLE_SPRITE_PATH;
        // Collect all images in folder
        foreach (string file in Directory.EnumerateFiles(filePath, "*.png"))
        {
            data = File.ReadAllBytes(file);
            // Convert file into sprite, and create button from it
            texture = new Texture2D(50, 50);
            if (texture.LoadImage(data))
            {
                // Preserve old data to reconstruct image from saved character later
                imageData.Add(new byte[data.Length]);
                data.CopyTo(imageData[imageCount], 0);
                newSprite = Sprite.Create(texture, new Rect(0, 0, 50, 50), new Vector2(0, 0), 1);

                newButton = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity, buttonParent);
                // Position is left for first button, and every other button after
                newButton.transform.localPosition = new Vector3((H_OFFSET * (imageCount % 2)) + MARGIN, -(imageCount / 2 * (V_OFFSET + MARGIN) + MARGIN), 0);
                newButton.SetActive(true);
                newButton.GetComponent<Image>().sprite = newSprite;
                buttons.Add(newButton);
                imageCount++;
            }
        }
        currImage.sprite = buttons[0].GetComponent<Image>().sprite;
        currData = imageData[0];
        // Set viewport size
        viewportContent.sizeDelta = new Vector2(viewportContent.sizeDelta.x, imageCount / 2 * (V_OFFSET + MARGIN) + MARGIN);
        if (viewportContent.sizeDelta.y < 200)
            viewportContent.sizeDelta = new Vector2(viewportContent.sizeDelta.x, 200);
    }

    public void SetCurrImage(Button imageSetter)
    {
        currImage.sprite = imageSetter.GetComponent<Image>().sprite;
        currData = new byte[imageData[buttons.IndexOf(imageSetter.gameObject)].Length];
        imageData[buttons.IndexOf(imageSetter.gameObject)].CopyTo(currData, 0);
    }
    public Sprite GetCurrImage()
    {
        return currImage.sprite;
    }

    public static byte[] GetTheEmpty()
    {
        return File.ReadAllBytes(".\\SavedData\\TheEmpty.png");
    }

    // Returns to charForm
    public void ExitImages(bool saveImage)
    {
        if (saveImage)
            charForm.SetImage(currImage.sprite, currData, isPortrait);

        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();

        mainManager.DisplayCharForm();
    }
}
