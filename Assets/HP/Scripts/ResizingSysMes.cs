using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResizingSysMes : MonoBehaviour
{
    [SerializeField] TMP_Text targetText; 
    [SerializeField] Image targetImage;  
    [SerializeField] Vector2 padding = new Vector2(5, 5); 

    private void Start()
    {
        if (targetText != null && targetImage != null)
        {
            Vector2 textSize = new Vector2(targetText.preferredWidth, targetText.preferredHeight);
            targetImage.rectTransform.sizeDelta = textSize + padding;
        }
    }
}
