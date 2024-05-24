using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmorHUD : MonoBehaviour //Player는 항상 ArmorHUD 보다 늦게 생성 ?? 되기 때문에 --> 여긴 그냥 진짜 ui 역할만 하는거임. 
{
    [SerializeField]
    private TextMeshProUGUI ArmorText; //아머 레벨을 적어줄 텍스트 

    private void Start()
    {
        ArmorText=GetComponent<TextMeshProUGUI>(); // 아머의 레벨을 적어줄 텍스트 
    }
    public void UpdateArmorUI(int ArmorLevel , int currentDurability,bool destroyedArmor)
    {
        // 0 레벨 아머는 파괴되지 않음. 
        if(ArmorLevel!=0 && destroyedArmor==true) //아머가 레벨 0 이 아니면서 아머가 파괴되었으면
        {
            ArmorText.text = "아머 파괴";

        }
        else
        {
            ArmorText.text = $"Armor Lv. {ArmorLevel}\n 내구: {currentDurability}";
        }
    }




}
