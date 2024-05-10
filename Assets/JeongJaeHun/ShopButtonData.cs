using JJH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonData : MonoBehaviour
{
    //각 상점에 떠있는 버튼이 지닌 아이템들이
    // 어떤 가격인지 어떤 타입인지 파악해주고 플레이어에게 붙자.

    public GoldManager goldManager;
    public ShopUIManager ShopUIManager;

    public GameObject player;

    public ItemDataSet ItemDataSet; //아이템의 전체적인 데이터
     
    public ArmorData ArmorData; //방어구에 관련된 데이터
    public GunData gunData; // 총에 관련된 데이터 
    public BoomData BoomData; //수류탄 또는 섬광탄에 대한 데이터
    public SwordData SwordData; //칼 종류에 대한 데이터 

    public EquipController EquipController; //장착중인 장비 관련 (인벤토리 처럼 사용하자.) 


    public void OnButtonClick() //버튼 클릭에 할당할 함수. 
    {
        
    }

    
    
    public void Test()
    {
        ArmorData = new ArmorData(ItemDataSet.ArmorType.Heavy_Armor);
        // 일단 오류는 없음 
        // 그렇다면 이거를 player와 연계하려면? 플레이어가 현재 이 아이템을 착용중이라는것을 알게 해주려면? 
        
    }

    private void Start()
    {
        ArmorData=gameObject.AddComponent<ArmorData>();
        print(ArmorData.Price);
    }


}
