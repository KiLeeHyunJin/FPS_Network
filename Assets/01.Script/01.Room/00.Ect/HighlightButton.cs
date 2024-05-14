using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite brushEffect;
    Sprite originalSprite;
    Image buttonImage;
    bool isFilling = false;
    float fillValue = 0f;
    float fillSpeed = 10f;
    float targetFillValue = 1f;
    float initSpeed;
    private void Start()
    {
        buttonImage = GetComponent<Image>(); 
        originalSprite = buttonImage.sprite;
        brushEffect = Resources.Load<Sprite>("Effect/ButtonBrush");

        initSpeed = fillSpeed;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (brushEffect != null)
        {
            buttonImage.sprite = brushEffect;
        }
        
        buttonImage.type = Image.Type.Filled;
        buttonImage.fillMethod = Image.FillMethod.Horizontal;
        isFilling = true;
        StartCoroutine(FillImage());

    }
    private IEnumerator FillImage()
    {
        
        while (isFilling)
        {
            fillValue = Mathf.MoveTowards(fillValue, targetFillValue, fillSpeed * Time.deltaTime);
            buttonImage.fillAmount = fillValue; // 이미지의 fillAmount 설정

            // 채우기가 완료되었을 때 코루틴 중지
            if (fillValue >= targetFillValue)
            {
                
                isFilling = false;
                
                yield break;
            }
            fillSpeed = Mathf.Lerp(initSpeed, 0f, fillValue / targetFillValue);

            yield return null;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isFilling = false;
        fillValue = 0;
        buttonImage.fillAmount = fillValue;
        buttonImage.sprite = originalSprite;
        fillSpeed = initSpeed;
    }
}
