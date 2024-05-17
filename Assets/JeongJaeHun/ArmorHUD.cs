using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmorHUD : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ArmorText; //아머 레벨을 적어줄 텍스트 

    [SerializeField]
    private ArmorManager ArmorManager; //아머를 관리하고 있는 slot인 armorManager에 접근 

    [SerializeField]
    private Armor CurrentArmor;

    [SerializeField]
    private int ArmorID; 


    private void Start()
    {
        
    }

    private void Update()
    {
        CheckArmorUI();
    }

    private void CheckArmorUI() //아머의 ui를 체크해줌.
    {
        CurrentArmor = ArmorManager.currentArmor;
        ArmorID = CurrentArmor.ArmorID; 

        ArmorText.text = $"Armor Lv. {ArmorID} ";
        
    }




}
