using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillEntry : MonoBehaviour
{
    public Image img;
    [SerializeField]KeyCode keyCode;
    public bool isIt;

    public KeyCode KeyCode { get { return keyCode; } }
    public void ChangeImage(Sprite newImage)
    {
        if (img != null)
            img.sprite = newImage;
        else
            Debug.LogError("isn't image");
    }
    
}
