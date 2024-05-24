using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이거 인벤토리임!!!! 

[CreateAssetMenu(fileName ="Item Data",menuName ="New Item/item")]
public class Item : ScriptableObject //게임 오브젝트에 붙일 필요가 없음. 
{
    // 아머는 어차피 데이터로만 있으면 되게 때문에 스크립터블 오브젝트로 만들어서 인벤토리에 있으면 능력치 적용되도록
    // 스프라이트 추가 해서 하단에 스프라이트 넣어주기 (ui 연계 ) 
    // 아머는 인벤토리에 들어가지 않고 그냥 방어도랑 내구도만 있으면된다. 
    // 장착된 상태인지 확인해야 하고 그 장착된 방어도에 따라 받는 데미지가 달라짐 
    // 결국 핵심은 
    // 1. 상점에서 price 비교에 따라 구입 성공/실패를 구현한다.
    // 2. 구입에 성공하면 자동으로 장착된다.
    // 3. 방어구의 종류에 따라 내구도와 방어력이 달라진다. --> 이를 플레이어에게 반영해준다.
    // 4. 방어구 관련된 이미지 등 ui를 업데이트 해준다. 
    // 5. 인벤토리에 직접 들어가지는 않으나 장착중이라는 표시를 해야하기 때문에 따로 저장을 해줘야한다. 
    
    // 여러 아이템의 공통적인 데이터 
    public enum ItemType  //아이템 유형
    {
        Bomb,Sword,Gun,Pistol,Armor,Skill
    }

    [Tooltip("아이템의 유형")]
    public ItemType itemType;


    [Tooltip("플레이어가 가지고 있는 스크립트")]
    PlayerInputController player;

    [Tooltip("상점의 물품 가격-->물품가격 비교 및 itemType 을 가져오기.")]
    public int price;

    

    [Tooltip("프리팹  --> 플레이어에게 Active 해줘야함. ")]
    public GameObject itemPrefab;

    [Tooltip("생성해줄 프리팹인데 이름을 맞춰줘야 하나? 일단 이름 적어주자.")]
    public string prefabName;

    [Tooltip("아이템의 idx --> 인벤토리 Add 시에 이용하자. ")]
    public int itemID; // ID에 맞는 오브젝트를 TRUE 해주면 된다. 

    // 이게 이정도만 있어도 실제 효과를 낼 수 잇는건지? 어렵다 ㅠㅠㅠㅠ 

    
    public string itemName; //아이템의 이름 
    public Sprite itemImage; //아이템 스프라이트 --> ui 인데 이미 다른 스크립트에 있기는 해서.. 생각해보기. 
    

    public string weaponType; //무기 유형 
    [TextArea(1,2)]
    public string weaponSpec; //무기 유형 

    public int maxBullet;
    public int totalBullet;



}
