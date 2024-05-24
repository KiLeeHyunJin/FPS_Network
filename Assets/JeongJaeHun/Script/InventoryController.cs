using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Define;

public class InventoryController : MonoBehaviourPun
{

    // 여기서 골드 관리 및 상점 연계 (골드쓰니까)
    [field : SerializeField] public int Gold { get ; set; }
    public TextMeshProUGUI goldText;
    Action<AnimationController.AnimatorWeapon> ChangeWeapon;
    private Item item; 
    [SerializeField] private IKWeapon[] weapons;
    [SerializeField] Transform pistolHolder;
    [SerializeField] Transform rifleHolder;
    [SerializeField] Transform swordHolder;
    [SerializeField] Transform throwHolder;

    [SerializeField] Transform pistolSaver;
    [SerializeField] Transform rifleSaver;
    [SerializeField] Transform swordSaver;
    [SerializeField] Transform throwSaver;

    // 웨폰 스왑 함수 발동시에 --> swap 컴포넌트의 변수 가져와서 스왑 방지 시켜주기. 
    [SerializeField] ArmorManager armorManager;
    [SerializeField] Armor currentArmor;

    [SerializeField] GunHUD gunHud;
    [SerializeField] CloseWeaponHUD CloseWeaponHUD;
    [SerializeField] BombHUD BombHUD;

    [SerializeField] SkillHolder skill;


    const string SpawnItem = "DropWeapon";
    public IKWeapon this[AnimationController.AnimatorWeapon weaponType]
    {
        get
        {
            return weapons[(int)weaponType];
        }
    }

    private BombController bombController;
    public bool BombUsePossible { 
        get 
        {
            if (bombController == null)
            {
                bombController = throwHolder.gameObject.GetComponent<BombController>();
                if(bombController == null)
                {
                    Debug.Log("Not Find Controller");
                    return false;
                }
            }
            if (bombController.CurrentBomb == null)
            {
                Debug.Log("No Has Bomb");
                return false;

            }
            if(bombController.CurrentBomb.currentBombNumber <= 0)
            {
                Debug.Log("Not Enought");
                return false;
            }
            return true;
        } 
    }

    #region
    // int slotIndex; --> 미사용


    // 획득한 아이템 --> 일단 구매라고 생각하고 하자. 획득은 그냥 땅에 떨군거 주워먹으면 되서...
    // 캐릭터 trigger GUn 체크 --> 이름 가져와서 자기 이름똑같은 Holder의 자식 active 해주면됨. 

    //public Image itemImage; --> 사실 이부분은 크게 중요하지는 않은듯함. -> hud에서 스프라이트 바꿔주면되서.. 

    // private List<GameObject> slotInventory= new List<GameObject>(); --> 나중에 list 인벤토리 필요할 때 

    //[SerializeField]
    // int slotIndex; --> 미사용


    // 획득한 아이템 --> 일단 구매라고 생각하고 하자. 획득은 그냥 땅에 떨군거 주워먹으면 되서...
    // 캐릭터 trigger GUn 체크 --> 이름 가져와서 자기 이름똑같은 Holder의 자식 active 해주면됨. 

    //public Image itemImage; --> 사실 이부분은 크게 중요하지는 않은듯함. -> hud에서 스프라이트 바꿔주면되서.. 

    // private List<GameObject> slotInventory= new List<GameObject>(); --> 나중에 list 인벤토리 필요할 때 
    #endregion

    private void Awake()
    {
        Gold = 1000;
        weapons = new IKWeapon[(int)AnimationController.AnimatorWeapon.END];
        weapons[(int)AnimationController.AnimatorWeapon.Sword] = swordHolder.GetChild(0).GetComponent<IKWeapon>();
    }

    private void Start()
    {

        skill = FindObjectOfType<SkillHolder>();


        if (photonView.IsMine)
        {
            gunHud = FindObjectOfType<GunHUD>();
            CloseWeaponHUD = FindObjectOfType<CloseWeaponHUD>();
            BombHUD = FindObjectOfType<BombHUD>();

            gunHud?.gameObject.SetActive(false);
            CloseWeaponHUD?.gameObject.SetActive(true);
            BombHUD?.gameObject.SetActive(false);
             
            Gold = 100; //시작 시에 100원으로 초기화. 

            if (goldText != null)
                goldText.text = $"{Gold}";

            ShopUIManager shopManager = FindObjectOfType<ShopUIManager>();
            if (shopManager != null)
                shopManager.inventory = this;
        }
        bombController = throwHolder.gameObject.GetComponent<BombController>();
    }

    public void GetCoin(int coin) //골드 획득 기능 -->text 업데이트 연계
    {
        if (goldText != null)
        {
            Gold += coin; //골드 추가. 
            goldText.text = $"{Gold}";
        }
    }
    public void SetChangePose(Action<AnimationController.AnimatorWeapon> action)
    {
        ChangeWeapon = action;
    }
    public void LoseCoin(int coin) //상점 아이템 구매 등
    {
        if (goldText == null)
            return;
        Gold -= coin;
        if (Gold < 0) Gold = 0; //최소값 0으로 제한 
        goldText.text = $"{Gold}"; //골드텍스트 초기화 
    }

    public void AddItem(Item _item) // 매개변수로 ID 받아서 그 ID에 맞춘 자식 오브젝트 활성화 시키기. 
    {

        if (_item.itemType == Item.ItemType.Skill)
        {
            for (int i = 0; i < skill.skillSlots.Length; i++)
            {
                SkillEntry skillEntry = skill.skillSlots[i];
                if (skillEntry.isIt)
                    continue;
                else
                {
                    AddSkill(_item,i);
                    Debug.Log("스킬 추가");
                    
                    return;
                }
            }
            Debug.Log("남은 스킬 슬롯 없음");
            
            
        }
        else
        {
            GameObject obj = _item.itemPrefab;
            if (!obj.TryGetComponent<IKWeapon>(out IKWeapon weapon))
                return;
            AddWeapon(weapon.weaponType, weapon.InstanceId);
        }
            
       
    }
    public void AddItem(IKWeapon _weapon)
    {
        AddWeapon(_weapon.weaponType,_weapon.InstanceId);
    }
    public void AddItem(AnimationController.AnimatorWeapon weaponType, int id)
    {
        AddWeapon(weaponType, id);
    }
    public void AddSkill(Item item, int i)
    {
        string skillName = item.itemName;
        Type skillType = Type.GetType(skillName);
        if (skillType != null)
        {
            Component skillComponent = gameObject.GetComponent(skillType);
            if (skillComponent != null)
            {
                skillComponent.GetType().GetProperty("enabled").SetValue(skillComponent, true);
                skill.skillSlots[i].isIt = true;
                skill.skillSlots[i].img.sprite = item.itemImage;
                skill.skillSlots[i].img.gameObject.SetActive(true);
                skillComponent.GetType().GetField("skillEntryImg").SetValue(skillComponent, skill.skillSlots[i].img);
                skillComponent.GetType().GetField("rewindKey").SetValue(skillComponent, skill.skillSlots[i].KeyCode);
                skillComponent.GetType().GetField("thisEntry").SetValue(skillComponent, skill.skillSlots[i]);



                Debug.Log($"스킬 : {skillName} 활성화됨.");
            }
        }
    }
    void AddWeapon(AnimationController.AnimatorWeapon weaponType, int id)
    {
        if (weapons[(int)weaponType] != null)
            Dequip(weapons[(int)weaponType]);

        weapons[(int)weaponType] = Equip(weaponType, id);
        ChangeWeapon?.Invoke(weaponType);
    }

    public void Throw(AnimationController.AnimatorWeapon weaponType)
    {
        Dequip(weapons[(int)weaponType]);
    }

    void Dequip(IKWeapon _weapon)
    {
        Transform parent = _weapon.weaponType switch
        {
            AnimationController.AnimatorWeapon.Pistol => pistolSaver,
            AnimationController.AnimatorWeapon.Rifle => rifleSaver,
            AnimationController.AnimatorWeapon.Sword => swordSaver,
            AnimationController.AnimatorWeapon.Throw => throwSaver,
            AnimationController.AnimatorWeapon.END => null,
            _ => null,
        };
        _weapon.transform.SetParent(parent);
        _weapon.gameObject.SetActive(false);
        weapons[(int)_weapon.weaponType] = null;
        photonView.RPC(SpawnItem, RpcTarget.MasterClient, _weapon.name);
    }

    [PunRPC]
    void DropWeapon(string _weaponName)
    {
        PhotonNetwork.Instantiate(_weaponName, transform.position, transform.rotation);
    }

    IKWeapon Equip(AnimationController.AnimatorWeapon weaponType, int id)
    {
        (Transform parent, Transform saver) pos = weaponType switch
        {
            AnimationController.AnimatorWeapon.Pistol =>(pistolHolder, pistolSaver),
            AnimationController.AnimatorWeapon.Rifle => (rifleHolder, rifleSaver),
            AnimationController.AnimatorWeapon.Sword => (swordHolder, swordSaver),
            AnimationController.AnimatorWeapon.Throw => (throwHolder, throwSaver),
            AnimationController.AnimatorWeapon.END => (null,null),
            _ => (null, null),
        };

        for (int i = 0; i < pos.saver.childCount; i++)
        {
            if (pos.saver.GetChild(i).TryGetComponent<IKWeapon>(out IKWeapon weapon) && weapon.InstanceId == id)
            {
                weapon.transform.SetParent(pos.parent);
                weapon.gameObject.SetActive(true);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                return weapon;
            }
        }
        return null;
    }


    public int ShieldCheck(int _damage)
    {
        return _damage;
        currentArmor = armorManager.GetCurrentArmor(); //현재 아머 가져오기.

        currentArmor.ArmorDurability--;

        if (currentArmor.ArmorDurability > 0)
        {
            _damage -= currentArmor.ArmorDefense;
        }

        return _damage;
    }

    public void Test(IKWeapon weapon)
    {
        //무기 바뀌는 시점을 원하는 함수
        // 잘 찾아지나 확인. 

        if(BombHUD.gameObject!=null || CloseWeaponHUD.gameObject!=null 
            || gunHud.gameObject!=null)
        {
            return;
        }


        switch (weapon.weaponType)
        {
            case AnimationController.AnimatorWeapon.Pistol:

                Gun gun = weapon as Gun;

                gunHud.CurrentGunCheck(gun);

                Debug.Log(gun.name);


                BombHUD.gameObject.SetActive(false);
                CloseWeaponHUD.gameObject.SetActive(false);
                gunHud.gameObject.SetActive(true);


                break;
            case AnimationController.AnimatorWeapon.Rifle:

                Gun gun2 = weapon as Gun;
                gunHud.CurrentGunCheck(gun2);

                BombHUD.gameObject.SetActive(false);
                CloseWeaponHUD.gameObject.SetActive(false);
                gunHud.gameObject.SetActive(true);


                break;
            case AnimationController.AnimatorWeapon.Sword:

                CloseWeapon closeWeapon = weapon as CloseWeapon;
                CloseWeaponHUD.CurrentSwordCheck(closeWeapon);

                gunHud.gameObject.SetActive(false);
                BombHUD.gameObject.SetActive(false);

                CloseWeaponHUD.gameObject.SetActive(true);

                break;
            case AnimationController.AnimatorWeapon.Throw:

                Bomb bomb = weapon as Bomb;
                BombHUD.SetCurrentBomb(bomb);

                CloseWeaponHUD.gameObject.SetActive(false);
                gunHud.gameObject.SetActive(false);
                BombHUD.gameObject.SetActive(true);

                break;
            case AnimationController.AnimatorWeapon.END:
                break;
        }
    }

    void OnEnable()
    {
        ChangeWeaponCallback(Test); //메소드 지정
    }




    Action<IKWeapon> changeWeaponCallback;
    public void ChangeWeaponCallback(Action<IKWeapon> setChangeWeapon) //무기가 바뀔때마다 호출을 원하는 함수를 지정
    {
        changeWeaponCallback = setChangeWeapon;
    }
    public void ChangeWeaponUpdate(AnimationController.AnimatorWeapon weaponType) //애니메이션에서 무기가 변경되는 타임에 무기 업데이트호출(지정한 함수 호출)
    {
        changeWeaponCallback?.Invoke(weapons[(int)weaponType]);
    }



    private void OnDestroy() // 라운드 재시작 시 player 파괴 후 재생성 하는 것 같음. --> hud를 켜줘야함. 
    {
        Debug.Log("플레이어 디스트로이");
        if(photonView.IsMine)
        {
            gunHud.gameObject.SetActive(true);
            BombHUD.gameObject.SetActive(true);
            CloseWeaponHUD.gameObject.SetActive(true); //일단 다시 켜보기. 
        }
       
    }
}
