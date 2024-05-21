using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArmorManager : MonoBehaviour
{
    // 이거를 실제 아머에다 슬롯에 있는 아머에다 넣자. 
    public int ArmorId;

    public int Defense; //방어력 

    public int Durability; //내구도 

    // 각 프리팹 마다 붙여줘서 active 되면 가지고 있는
    // 디펜스 , 듀라빌리티 이용해서 그걸 equip에서 불러와서 
    // 자신의 방어구 변경 해주면 되지용 

    public Sprite ArmorSprite; //스프라이트 필요 없을것 같은디 

    public Armor currentArmor;

    // 함수 생성 (현재 아머를 받는 )
    public Armor GetCurrentArmor() { return currentArmor; }


    private void Start()
    {
        CurrentArmorCheck(0); //0번 아머 키기 
    }

    //매개변수로 armorId를 주면 ok. (0 1 2 ) 
    public void CurrentArmorCheck(int num) // 외부에서 armor 변경 시 (아머 구입 ) --> 이 함수 호출해주면 됨. 
    {
        int childOfNum = transform.childCount;

        for (int i = 0; i < childOfNum; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            
        }

        transform.GetChild(num).gameObject.SetActive(true); //0번 아머는 켜줘야함 start 시에 
        currentArmor = transform.GetChild(num).GetComponent<Armor>();
    }


    

}
