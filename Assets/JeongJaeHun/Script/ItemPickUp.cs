using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUp : MonoBehaviour
{
    // 이거를 상점 품목 버튼에다 달아놓고 --> item 스크립터블 오브젝트 붙여놓기. 

    [field : SerializeField] public Item item{ get; private set; } // item 스크립터블 오브젝트를 참조하기 위함. 
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI weawponName;
    [SerializeField] TextMeshProUGUI price;
    [SerializeField] TextMeshProUGUI spec;

    private void OnEnable()
    {
        if (item == null)
            return;
        icon.sprite = item.itemImage;
        weawponName.text = item.itemName;
        price.text = item.price.ToString();
        spec.text = item.weaponSpec;
    }

}
