using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{ 
    [field: SerializeField] public GameObject sellItem 
    { get; private set; }
    [SerializeField] Sprite itemIcon;
    [SerializeField] int sellPrice;

    private void Awake()
    {
        if(TryGetComponent<Image>(out Image img))
            img.sprite = itemIcon;
    }


}
