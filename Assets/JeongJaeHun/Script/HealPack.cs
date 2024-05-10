using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealPack : MonoBehaviour,IInteractable
{
    public float hp = 100;
    public int heal = 50;

    public Slider slider;
    public TextMeshProUGUI text;

    
    public void Interaction()
    {
        hp += heal; 
        if(hp>=100) hp = 100;
        text.text=$"{Mathf.Round(hp)}";
        slider.value = hp;

    }

    public void SkillHeal(object heal)
    {
        if (hp >= 100) hp = 100;
        hp +=(float)heal;
        text.text = $"{Mathf.Round(hp)}";
        slider.value = hp;
    }


    
}
