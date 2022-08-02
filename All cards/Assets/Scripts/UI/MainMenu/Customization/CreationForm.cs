using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class for forms that need to save and load items
public class CreationForm : MonoBehaviour
{
    public virtual void SaveItem()
    {

    }
    public virtual void LoadItem()
    {

    }
    public virtual void SetImage(bool saveChanges, Sprite newSprite, byte[] newData)
    {

    }
}
