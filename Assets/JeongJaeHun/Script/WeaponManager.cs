using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일단 만든 다음에 인벤토리랑 연계해보자 일단 만들자.

public class WeaponType //아 이거 이용안되나? --> 아 인벤토리랑 연계를 어떻게하지??
{
    public Gun.GunType GunType;
    public CloseWeapon.CloseWeaponType CloseWeaponType;
    public Bomb.BombType bombType;

    public void ReturnType()
    {
        GunType = Gun.GunType.SHOTGUN;
    }
}

public class WeaponManager : MonoBehaviour
{
    // Holder에 붙일 웨폰 매니저. 모두가 공유가능한 static 변수 존재.
    // 무기 교체 및 여러 무기들의 동작을 관리함. --> 일단 적어보고 player에 setactive로
    // 넣어두는 방법과 얼마나 차이나는지 어떻게 이용할 수 있는지 생각해보기.

    public static bool isChangeWeapon = false; //무기 교체 중복 실행 방지 (true면 무기 교체 불가 상태 )

    [SerializeField] float changeWeaponDelayTime; //무기 교체 딜레이 시간 (총 집어넣는 타임. )
    [SerializeField] float changeWeaponEndDelayTime; //무기 교체가 완전히 끝난 시점(새로운 웨폰으로 교체된 시간)

    [SerializeField]
    private Gun[] guns; //모든 종류의 총을 원소로 가지는 배열 
    private Bomb[] bomb; //모든 정류의 폭탄을 원소로 가지는 배열 (수류탄 + 섬광탄 2종류)
    private CloseWeapon[] closeWeapon; // ( 검 1종류 만들 예정이지만 일반화 도전 ) 

    //관리 차원에서 이름으로 쉽게 무기 접근이 가능하도록 Dictionary 구조 사용  //건의 타입은 언제 써야할지 생각하기.

    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, Bomb> bombDictionary = new Dictionary<string, Bomb>();
    private Dictionary<string, CloseWeapon> weaponDictionary = new Dictionary<string, CloseWeapon>();

    [SerializeField]
    private string currentWeaponType; // 현재 무기의 타입 (총,폭탄,칼)


    public static Transform currentWeapon; //현재 무기 static 선언으로 클래스를 통해 바로접근 가능하도록함.
    //public static Animator currentWeaponAnim; //현재 무기의 애니메이션

    [SerializeField]
    private GunController theGunController; 
    [SerializeField]
    private CloseWeapon theCloseWeaponController;  
    [SerializeField]
    private BombController theBombController;

    //public Animator currentWeaponAnim; --> 애니 메이션 여기서 작업안할듯? 


    private void Start()
    {
        // 그런데 상점이 있는데 이게 의미가 있을까?? --> 어차피 처음에는 하나만 켜져있을거고..
        // 인벤토리를 따로 구성할 건데? --> 생각해보자.
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]); //이름과 오브젝트 저장 
        }

        for (int i = 0; i < closeWeapon.Length; i++)
        {
            weaponDictionary.Add(closeWeapon[i].closeWeaponName, closeWeapon[i]);
        }


        for (int i = 0; i < bomb.Length; i++) //수류탄 + 섬광탄은 기본 지급이니까 여기서 start에도 괜찮지.
        {
            bombDictionary.Add(bomb[i].bombName, bomb[i]);
        }

    }
    public void OnInputSwapWeapon() //이거 어차피 equip에서 함수 부르는데 일단.. 
                                    // 각 번호에 알맞는 무기가 저장되어야 하고 그 무기가 있어야 하고 (특히 1번 무기 -->주무기) 
    {
        // 이 부분은 진짜 생각해보자. 
        /*if (!isChangeWeapon)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 누르면 '맨손'으로 무기 교체 실행
            {
                StartCoroutine(ChangeWeaponCoroutine("HAND", "맨손"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 누르면 '서브 머신건'으로 무기 교체 실행
            {
                StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));
            }
        }*/
    }

    //매개변수로 string 대신에 weapon type으로 넣어주고.. 넣어준 상태에서 +로 무기 구분 가능할까?
    public IEnumerator ChangeWeaponCoroutine(string type, string name) //무기 변환 코루틴 
    {
        isChangeWeapon = true; //변환 상태 트루 --> 변환 중 또 변환 못하도록
        // 어차피 얘가 플레이어 자식? 에 달릴거라 플레이어가 애니메이션 가지고 있어도 같이 사용가능한듯?
        //currentWeaponAnim.SetTrigger("Weapon_Out");
        yield return new WaitForSeconds(changeWeaponDelayTime); //변환 애니 재생동안 중지 

        CancelPreWeaponAction();
        WeaponChange(type, name);
        yield return new WaitForSeconds(changeWeaponEndDelayTime);
        currentWeaponType = type; // 아 이게 enum을 연결해 줄 수는 없나? 무기 종류가 많아서 연결하기 힘든가??
        isChangeWeapon = false; // 무기 변환 완료 


    }

    private void CancelPreWeaponAction() //기존에 들고있는 무기 해제 
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight();
                theGunController.CancelReload();
                GunController.isActivate = false;
                break;

        }
    }

    private void WeaponChange(string type, string name) // 바꾸고자 하는 무기로 전환 
    {
        if (type == "GUN")
        {
            theGunController.GunChange(gunDictionary[name]);
        }
        else if (type == "BOMB")
        {

        }
        else if (type == "SOWRD")
        {

        }

        // 총 무기가 1,2,3 종류가 있고 하면 배열에 원소로 추가해주면된다. 
        // 이거 권총형을 만드는게 훨씬 나을것 같기도 하고.. 고민되네.. 
    }


}
