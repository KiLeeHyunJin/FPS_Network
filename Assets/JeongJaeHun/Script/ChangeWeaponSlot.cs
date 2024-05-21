using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ChangeWeaponSlot : MonoBehaviour
{
    // 어차피 부모 inventory가 켜져 잇으니까 그냥 slot을 껏다 켯다 하는식으로하면
    // 그 내부의 무기들을 active 하는거는 다른 곳에서 작업하기 때문에 
    // 그냥 여기서는 큰 슬롯들을 켰다 껏다 하는 방식만 사용하면 된다. 

    [SerializeField] private Slot[] slots; //무기 슬롯 배열 

    [Tooltip("총기 장비 중일 때 나올 UHD ")]
    [SerializeField] private GunHUD gunHUD;
    [Tooltip("근접 무기 장비시에 나올 UHD")]
    [SerializeField] private CloseWeaponHUD closeWeaponHUD;
    [Tooltip("폭탄 무기 장비시에 나올 UHD")]
    [SerializeField] private BombHUD bombHUD;

    [Tooltip("이미 무기를 변경 중 인 상황을 체크 할 bool변수--> fire 시에 이거 가져다 쓰자. ")]
    [SerializeField] public bool isChangeWeapon;

    [Tooltip("무기 전환 시 적용 할 쿨타임")]
    [SerializeField] private float changeWeaponDelayTime;




    public enum slotType //HUD와 연계 할 슬롯 타입 (매개변수로 받아서 HUD ON/OFF --> 하드 코딩 방지 ) 
    {
        PistolType = 0,
        ArType = 1,
        CloseWeaponType = 2,
        GrenadeType = 3,
        FlashBangType = 4
    }


    private slotType curSlotType;

    public UnityEvent<slotType> slotChange;

    public slotType CurSlotType
    {
        get { return curSlotType; }
        set
        {
            curSlotType = value;
            slotChange?.Invoke(curSlotType);
        }
    }

    //[SerializeField] private WeaponManager weaponManager;

    Inventory inventory;

    public void Change()
    {
        slotChange.AddListener(SlotChange);
    }

    public void SlotChange(slotType nextType)
    {
        // 현재 타일 끄고
        //switch(nextType)
        // 다음 타일켜기
    }

    private void Start()
    {
        inventory = GameObject.FindObjectOfType<Inventory>();
        slots = inventory.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            slot.gameObject.SetActive(false); //일단 모든 슬롯을 꺼주고. 
        }

        slots[0].gameObject.SetActive(true); //0번 슬롯 --> 즉 피스톨 부분만 start 시에 켜줘서 시작해주기. 
    }

    // 무기 버리는 경우가 있을 수 있으니까 만약 slot 내부에 모든 종류의 아이템이 꺼져 있다면 
    // 그 슬롯으로 전환이 불가능한 경우이므로 .. 전환을 return 해주자. 

    public void Excute(AnimationController.AnimatorWeapon weapon) //실제 전환 실행 --> equip컨트롤에서 불러서 작업해주기. 
    {
        // 무기 체인지가 진행 중이면 다시 변경 못하도록 잠시 리턴 띄워주기.
        if (isChangeWeapon == true)
        {
            return;
        }
        switch (weapon)
        {
            case AnimationController.AnimatorWeapon.Pistol:  //권총 1번 

                closeWeaponHUD.gameObject.SetActive(false);
                bombHUD.gameObject.SetActive(false);
                gunHUD.gameObject.SetActive(true);

                StartCoroutine(ChangeWeaponCoroutine());
                break;

            case AnimationController.AnimatorWeapon.Rifle:  // 소총 2번 
                closeWeaponHUD.gameObject.SetActive(false);
                bombHUD.gameObject.SetActive(false);
                gunHUD.gameObject.SetActive(true);

                StartCoroutine(ChangeWeaponCoroutine());

                break;

            case AnimationController.AnimatorWeapon.Sword:  // 칼 3번 
                closeWeaponHUD.gameObject.SetActive(true);
                bombHUD.gameObject.SetActive(false);
                gunHUD.gameObject.SetActive(false);

                StartCoroutine(ChangeWeaponCoroutine());

                break;
            case AnimationController.AnimatorWeapon.Throw: //수류탄 4번 

                closeWeaponHUD.gameObject.SetActive(false);
                bombHUD.gameObject.SetActive(true);
                gunHUD.gameObject.SetActive(false);

                StartCoroutine(ChangeWeaponCoroutine());

                break;
            case AnimationController.AnimatorWeapon.END: // 섬광탄용 마지막 위치 일단 넣어둠. -->(5번) 

                closeWeaponHUD.gameObject.SetActive(false);
                bombHUD.gameObject.SetActive(true);
                gunHUD.gameObject.SetActive(false);
                StartCoroutine(ChangeWeaponCoroutine());

                break;
        }
    }
        // 슬롯 내부의 자식들을 체크해서 그 자식들이 켜져 있는지 확인하고 하나도 안켜져 있다면 return 해버리기

        // Temp 로 1~5 번 슬롯까지 input 눌러서 바꾸는거 실험해보자.

        private IEnumerator ChangeWeaponCoroutine()

        {
            // 공통으로 진행 할 무기 전환 작업 --> 공격 불가능 + 재장전 불가능 상태로 만들어줘야함. 

            isChangeWeapon = true;

            yield return new WaitForSeconds(changeWeaponDelayTime); // 무기 전환 시간동안 재 변환금지
            isChangeWeapon = false;

        }

    }

