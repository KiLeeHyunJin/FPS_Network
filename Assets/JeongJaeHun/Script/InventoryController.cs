using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Define;

public class InventoryController : MonoBehaviour
{
    
    // 여기서 골드 관리 및 상점 연계 (골드쓰니까)
    public int Gold { get; set; }
    public TextMeshProUGUI goldText; 
    
    private  Item item; //이 부분 p[ublic 참조 해야하나? 아닐 것 같은데 
    [SerializeField] private IKWeapon[] weapons;
    [SerializeField] Transform pistolHolder;
    [SerializeField] Transform rifleHolder;
    [SerializeField] Transform swordHolder;
    [SerializeField] Transform throwHolder;
    public IKWeapon this[AnimationController.AnimatorWeapon weaponType]
    {
        get {
            Debug.Log($"List {weaponType}");
            return weapons[(int)weaponType]; }
    }
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

    private void Awake()
    {
        weapons = new IKWeapon[(int)AnimationController.AnimatorWeapon.END];
        weapons[(int)AnimationController.AnimatorWeapon.Sword] = swordHolder.GetChild(0).GetComponent<IKWeapon>();
    }
    private void Start()
    {
       //slots= gameObject.GetComponentsInChildren<Slot>();
       if(goldText != null)
            goldText.text = $"{0}"; //시작 시에 0원으로 초기화 
    }

    public void GetCoin(int coin) //골드 획득 기능 -->text 업데이트 연계
    {
        if (goldText != null)
        {
            Gold += coin; //골드 추가. 
            goldText.text = $"{Gold}";
        }
    }
    
    public void LoseCoin(int coin) //상점 아이템 구매 등
    {
        if (goldText == null)
            return;
        Gold -= coin;
        if (Gold < 0) Gold = 0; //최소값 0으로 제한 
        goldText.text = $"{Gold}"; //골드텍스트 초기화 
    }


    public IKWeapon AddItem(Item _item,int ID) // 매개변수로 ID 받아서 그 ID에 맞춘 자식 오브젝트 활성화 시키기. 
    {
        GameObject obj = _item.itemPrefab;
        if (!obj.TryGetComponent<IKWeapon>(out IKWeapon weapon))
            return null;
        return AddWeapon(weapon.GetInstanceID());
    }
    public IKWeapon AddItem(IKWeapon _weapon)
    {
        return AddWeapon(_weapon.GetInstanceID());
    }
    public IKWeapon AddItem(int _id)
    {
        return AddWeapon(_id);
    }
    IKWeapon AddWeapon(int id)
    {
        PooledObject getPoolObject = Manager.Pool.GetPool(id, Vector3.zero, Quaternion.identity);

        if (getPoolObject == null)
            return null;

        IKWeapon newWeapon = getPoolObject as IKWeapon;
        if(newWeapon != null)
        {
            AnimationController.AnimatorWeapon weaponType = newWeapon.weaponType;

            IKWeapon beforeWeapon = weapons[(int)weaponType] != null ? weapons[(int)weaponType]  : null;

            weapons[(int)weaponType] = newWeapon;
            SetWeapon(newWeapon);

            return beforeWeapon;
        }
        return null;
    }

    public void RemoveItem() //인벤토리에서 아이템을 제거해주는 함수 --> 이거 무기 버리기 함수 가져오자. 어딨더라?
    {
        // 무기 떨구기. --> 기본적으로 id로 체크해서 id가 겹치면 그 프리팹을 생성해줘야하는데 어떻게 생성하지? 



    }

    void SetWeapon(IKWeapon _newWeapon)
    {
        if (_newWeapon == null)
            return;
        Transform parentHolder = _newWeapon.weaponType switch
        {
            AnimationController.AnimatorWeapon.Pistol => pistolHolder,
            AnimationController.AnimatorWeapon.Rifle => rifleHolder,
            AnimationController.AnimatorWeapon.Sword => swordHolder,
            AnimationController.AnimatorWeapon.Throw => throwHolder,
            AnimationController.AnimatorWeapon.END => null,
            _ => null,
        };
        if (parentHolder != null)
        {
            _newWeapon.transform.SetParent(parentHolder);

            _newWeapon.transform.localPosition = Vector3.zero;
            _newWeapon.transform.localRotation = Quaternion.identity;
        }
    }

}
