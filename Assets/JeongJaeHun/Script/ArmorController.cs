using Photon.Pun;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ArmorController : MonoBehaviourPun
{
    // player에게 붙어있으며 자식으로 Armor 1 2 3 을 가지고 있는 게임오브젝트 (여기서 ArmorHud와 연계해주자. ) 
    public int ArmorId;

    public int Defense; //방어력 

    public int Durability; //내구도 

    // 각 프리팹 마다 붙여줘서 active 되면 가지고 있는
    // 디펜스 , 듀라빌리티 이용해서 그걸 equip에서 불러와서 
    // 자신의 방어구 변경 해주면 되지용 

    public Sprite ArmorSprite; //스프라이트 필요 없을것 같은디 

    public Armor currentArmor;

    public ArmorHUD armorHUD;


    // 함수 생성 (현재 아머를 받는 )
    public Armor GetCurrentArmor() { return currentArmor; }


    private void Start()
    {
        //아머 컨트롤러에서 아머 UI를 가지자. 
        if (photonView.IsMine)
        {
            armorHUD = FindObjectOfType<ArmorHUD>();
        }

        if (armorHUD != null)
        {
            CurrentArmorCheck(0,0,0); //0번 아머 키기 --> Lv.0 Armor 
            ArmorControllerUpdate(currentArmor.ArmorDurability,false); // 파괴되지 않은 0번 아머 전송. 
        }
    }

    //매개변수로 armorId를 주면 ok. (0 1 2 ) 
    public void CurrentArmorCheck(int lv,int defense,int durablity) // 외부에서 armor 변경 시 (아머 구입 ) --> 이 함수 호출해주면 됨. 
    {
        int childOfNum = transform.childCount;

        for (int i = 0; i < childOfNum; i++) //ArmorController 하단의 Armor 자식들 가져오기. 
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        transform.GetChild(lv).gameObject.SetActive(true); //0번 아머는 켜줘야함 start 시에 
        currentArmor = transform.GetChild(lv).GetComponent<Armor>();
        currentArmor.ArmorLevel = lv;
        currentArmor.ArmorDefense = defense;
        currentArmor.ArmorDurability = durablity;
        
        armorHUD.UpdateArmorUI(currentArmor.ArmorLevel,currentArmor.ArmorDurability, false); // 새로 구매하는 경우 이므로 무조건 false 보내주면 될듯? 
        

    }

    public void ArmorControllerUpdate(int currentDurabilty,bool Destroyed)
    {

        armorHUD.UpdateArmorUI(currentArmor.ArmorLevel,currentArmor.ArmorDurability, Destroyed); // UI에 표기되는 TEXT를 업데이트 해줘야함. --> 매개변수 필요. 
        // 실제 변경 함수는 아이템 구매시에 인벤토리에서 진행해준다. 
        // 어차피 실드 데미지 체크는 인벤토리에서 진행하기 때문에 UI는 한 번만 업데이트 해주면 된다. 

        // -> controller의 Armor LV와 inventory에서 받을 아머 파괴를 확인하고 hud에 업데이트 해주기. 
    }

    public void ArmorPurChase(int ArmorLevel,int ArmorDefense,int ArmorDurability)
    {
        CurrentArmorCheck(ArmorLevel, ArmorDefense, ArmorDurability);

        // 이거 item에 spec 붙여야 숫자 초기화 안될듯? 
        
    }


    // 이렇게 되면 Armor 파괴 시에 다시 구매해도 Armor의 스펙은 변하지 않는지 확인할 것. 


}
