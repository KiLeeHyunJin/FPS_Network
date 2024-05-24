using Cinemachine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Define;

public class Controller : MonoBehaviourPun, IPunObservable
{
    #region
    [Range(0.05f, 3f)]
    [SerializeField] float groundCheckLength;
    [Range(0.05f, 2f)]
    [SerializeField] float ignoreGroundCheckLength;
    [Range(0.2f, 3)]
    [SerializeField] float jumpHeight;
    [SerializeField] float walkStandSpeed;
    [SerializeField] float runStandSpeed;
    [SerializeField] float walkCrouchSpeed;
    [SerializeField] float runCrouchSpeed;
    [Range(1f, 3f)]
    [SerializeField] float gravitySpeed;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] FrontSensor sensor;

    [SerializeField] GameObject rootBone;
    [SerializeField] GameObject foot;

    [SerializeField] Transform zoomIn;
    [SerializeField] Transform zoomOut;

    [SerializeField] float mouseSensitivity;
    [SerializeField] GameObject[] FPSIgnoreObject;
    [SerializeField] GameObject[] FPSHand;
    [SerializeField] GameObject[] weaponObj;
    [SerializeField] Transform target;

    [SerializeField] Transform cameraRoot;
    [SerializeField] Camera overlayCam;
    [SerializeField] CinemachineVirtualCamera cam;

    [SerializeField] GameObject minimapIcon_m;
    [SerializeField] GameObject minimapIcon_Ally;
    [SerializeField] EnemySearcher enemySearcher;
    [SerializeField] GameObject enemyIcon;

    [SerializeField] int teamCode;
    [SerializeField] GameObject miniCam;

    [SerializeField] ParticleSystem rewindEff;
    [SerializeField] ParticleSystem outRewindEff;
    int maxHp;
    [SerializeField] int hp;
    public bool Mine { get; private set; }

    public int TeamCode { get; private set; }

    CameraController cameraController;
    PlayerInputController inputController;
    PlayerInput playerInput;
    CharacterTransformProcess moveProcess;
    InventoryController inventoryController;
    AnimationController animController;
    ProcessingController processingController;

    RequestController requestController;
    event Action Updates;

    //Iattackable[] iattackables;
    Iattackable currentAttackable;

    [SerializeField] KillLogPanel killLog; // 킬 로그 패널
    [SerializeField] Slider HpBar; // 플레이어의 Hp bar 연계
    [SerializeField] TapEntry tapEntry;

    [SerializeField] int playerNum;

    [SerializeField] SkinnedMeshRenderer[] renderers;
    [SerializeField] MeshRenderer[] mrenders;

    [SerializeField] PhotonView pv;

    private void Awake()
    {
        animController = gameObject.GetOrAddComponent<AnimationController>();
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }
    void Start()
    {
        if (PhotonNetwork.InRoom == false)
        {
            Destroy(gameObject);
            return;
        }

        killLog = GameObject.FindWithTag("KillLog")?.GetComponent<KillLogPanel>();
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
       
        tapEntry = FindObjectOfType<TapEntry>();

        Check();
        CallDefaultPose();

        if (photonView.IsMine)
        {
            pv = GameObject.FindWithTag("InGameManager")?.GetComponent<PhotonView>();
            HpBar = GameObject.FindWithTag("HpBar")?.GetComponent<Slider>();
            if (HpBar != null)
            {
                Debug.Log("Hp init");
                HpBar.value = maxHp; //hp가 SetData에서 maxHp로 할당되므로 
            }
        }
    }

    void Check()
    {
        CheckMine();
        CollidersSetting();
        SetKeyAction();
        MoveProcessInit();
        SetUpdateAction();
    }

    void CheckMine()
    {
        if (photonView.Controller.GetPhotonTeam() == null)
            teamCode = 0;
        else
            teamCode = photonView.Controller.GetPhotonTeam().Code;
        //minimapIcon_m.SetActive(true);
        Mine = photonView.IsMine;
        inputController = gameObject.GetOrAddComponent<PlayerInputController>();
        processingController = GetComponent<ProcessingController>();

        requestController = GetComponent<RequestController>();
        inventoryController = GetComponent<InventoryController>();
        cameraController = new CameraController(target, this, cam, cameraRoot, zoomIn, zoomOut);
        inventoryController.SetChangePose((_weaponType) =>
        {
            animController.ChangeWeapon(_weaponType, ref currentAttackable);
            inputController.SetWeaponType = _weaponType;
            cameraController.SetZoomPosition(inventoryController[_weaponType].ZoomPos);
        });
        minimapIcon_m?.SetActive(true);

        if (Mine == false)
        {
            Destroy(enemySearcher);
            minimapIcon_m?.SetActive(false);
            cam.gameObject.SetActive(false);
            Destroy(miniCam);
            Destroy(inputController);
            Destroy(processingController);
            if (TryGetComponent<PlayerInput>(out var input))
                Destroy(input);
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null)
            {
                if (teamCode == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
                {
                    Destroy(enemyIcon);
                    minimapIcon_Ally.SetActive(true);
                }
            }
            return;
        }

        //Cursor.lockState = CursorLockMode.Locked;
        cameraController.Init(ControllCharacterLayerChange, overlayCam, mouseSensitivity);
        inputController.Owner = this;
        sensor.StartInit();
        SetData();
    }

    void SetData()  // 이 부분 start 내부에서 이미 체크하고 있음 (체력 초기화 완료됨) 
    {
        maxHp = maxHp <= 0 ? 100 : maxHp;
        if (photonView.Controller.GetPhotonTeam() != null)
            teamCode = photonView.Controller.GetPhotonTeam().Code;
        else
            teamCode = 0;
        hp = maxHp;
    }

    public AnimationController.AnimatorWeapon GetCurrentWeapon()
    {
        return inventoryController[inputController.CurrentWeapon].weaponType;
    }

    void CollidersSetting() //하위 객체를 돌며 충돌체가 있다면 6번 레이어로 변경 및 히트박스 부착
    {
        Player pl = PhotonNetwork.LocalPlayer;

        Collider[] colliders = rootBone.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
            collider.gameObject.AddComponent<HitBox>().SetOwner(this, Mine);
    }
    public void SetZoomPosition(Transform _zoom)
        => cameraController.SetZoomPosition(_zoom);
    void Update()
        => Updates?.Invoke();
    void FixedUpdate()
        => moveProcess?.FixedUpdate();

    public void CallDefaultPose()
    {
        CallThree();
        inputController.SetWeaponType = AnimationController.AnimatorWeapon.Sword;
    }

    void CallPickUp()
    {
        Collider fronwWeapon = sensor.FrontObj;
        if(fronwWeapon != null)
        {
            if (fronwWeapon.TryGetComponent<IKWeapon>(out IKWeapon weapon))
            {
                ChangeWeaponState();

                inventoryController.AddItem(weapon);
                weapon.PickUp();
            }
        }
        else
        {
            if (inputController.CurrentWeapon == AnimationController.AnimatorWeapon.Sword || 
                inputController.CurrentWeapon == AnimationController.AnimatorWeapon.Throw)
                return;

            if (inventoryController[inputController.CurrentWeapon] != null)
            {
                inventoryController.Throw(inputController.CurrentWeapon);
                CallDefaultPose();
            }
        }
    }

    void CallFire()
    {

        if (currentAttackable != null && currentAttackable.Attack()&&!Manager.Game.onShop)
        {
            animController.Atck();
            cameraController.GetCamShakeRoutine();
        }
    }
    void CallReload()
    {
        if (currentAttackable != null && currentAttackable.Reload()&& !Manager.Game.onShop)
        {
            animController.Reload();
        }
    }

    bool buttonType = false;
    void CallChangeFireType()
    {
        if (buttonType)
            inputController.ChangeFireType = Define.FireType.Repeat;
        else
            inputController.ChangeFireType = Define.FireType.One;
        buttonType = !buttonType;
    }

    void CallOne()
    {
        if (inventoryController[AnimationController.AnimatorWeapon.Rifle] == null)
            return;

        if (animController.ChangeWeapon(AnimationController.AnimatorWeapon.Rifle, ref currentAttackable) == false)
            return;

        if (inputController.Zoom)
            cameraController.ZoomChange(false);
    }
    void CallTwo()
    {
        if (inventoryController[AnimationController.AnimatorWeapon.Pistol] == null)
            return;

        if (animController.ChangeWeapon(AnimationController.AnimatorWeapon.Pistol, ref currentAttackable) == false)
            return;

        ChangeWeaponState();
    }
    void CallThree()
    {
        if (animController.ChangeWeapon(AnimationController.AnimatorWeapon.Sword, ref currentAttackable) == false)
            return;

        ChangeWeaponState();
    }
    void CallFour()
    {
        //if (inventoryController[AnimationController.AnimatorWeapon.Throw] == null)
        //    return;
        if (inventoryController.BombUsePossible == false)
            return;
        if (animController.ChangeWeapon(AnimationController.AnimatorWeapon.Throw, ref currentAttackable) == false)
            return;

        ChangeWeaponState();
    }

    void ChangeWeaponState()
    {
        inputController.ChangeFireType = Define.FireType.One;
        if (inputController.Zoom)
            cameraController.ZoomChange(false);
    }

    void SetKeyAction()
    {
        if (Mine == false)
            return;
        inputController.Init();
        inputController.SetKey(CallReload, Define.Key.R);
        inputController.SetKey(CallPickUp, Define.Key.F);
        inputController.SetKey(CallOne, Define.Key.F1);
        inputController.SetKey(CallTwo, Define.Key.F2);
        inputController.SetKey(CallThree, Define.Key.F3);
        inputController.SetKey(CallFour, Define.Key.F4);
        inputController.SetZoomType(cameraController.ZoomChange);
        inputController.SetKey(CallFire, Define.Key.Press);
        inputController.SetKey(CallChangeFireType, Define.Key.V);
    }
    void MoveProcessInit()
    {
        if (Mine == false)
            return;
        moveProcess = new CharacterTransformProcess();
        moveProcess.Init(GetComponent<CharacterController>(), Mine);
        moveProcess.InitGroundCheckData(
            foot.transform, groundCheckLength, ignoreGroundCheckLength, groundLayer,
            jumpHeight, gravitySpeed);

        moveProcess.SetMotions(AnimationController.MoveType.Run, animController.MoveRun);
        moveProcess.SetMotions(AnimationController.MoveType.Walk, animController.MoveWalk);
        moveProcess.SetMotions(AnimationController.MoveType.Stop, animController.MoveStop);

        moveProcess.SetMotions(AnimationController.MoveType.JumpFinish, animController.JumpFinish);
        moveProcess.SetMotions(AnimationController.MoveType.Jump, animController.JumpStart);
        moveProcess.SetMotions(AnimationController.MoveType.JumpSlide, SlideJump);

        moveProcess.SetMotions(AnimationController.MoveType.Crouch, () => animController.Crouch(true));
        moveProcess.SetMotions(AnimationController.MoveType.Stand, () => animController.Crouch(false));

        moveProcess.SetMoveActionValue(v => animController.VelocityY = v);
        moveProcess.SetMoveActionValue(v => animController.MoveValue = v);

        inputController.SetRot(v => cameraController.InputDir = v);

        inputController.SetMoveKey(moveProcess.SetMoveValue);
        inputController.SetMoveType(moveProcess.SetMoveType);
        inputController.SetKey(moveProcess.Crouch, Define.Key.C);
        inputController.SetKey(moveProcess.Jump, Define.Key.Space);

        moveProcess.SetMoveSpeed(walkStandSpeed, runStandSpeed, walkCrouchSpeed, runCrouchSpeed);
        moveProcess.Start();
    }
    void SetUpdateAction()
    {
        if (Mine)
        {
            Updates -= cameraController.Update;
            Updates += cameraController.Update;

            Updates -= moveProcess.Update;
            Updates += moveProcess.Update;
        }
    }

    void ControllCharacterLayerChange(int bodyNum, int handNum)
    {
        foreach (GameObject childeGameObject in FPSIgnoreObject)
            childeGameObject.layer = bodyNum;
        foreach (GameObject childeGameObject in FPSHand)
            childeGameObject.layer = handNum;

        for (int i = 0; i < weaponObj.Length; i++)
        {
            if (weaponObj[i] != null)
            {
                Transform[] children = weaponObj[i].GetComponentsInChildren<Transform>();
                foreach (Transform chl in children)
                {
                    chl.gameObject.layer = handNum;
                }
            }
        }
    }

    void SlideJump()
    {
        float runCycle =
                   Mathf.Repeat(
                   animController.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1);
        float dir = runCycle < 0.5f ? 1 : -1;
        float jumpLeg = dir * inputController.MoveY;
        animController.VelocityY = jumpLeg;
    }
    #endregion
    public void Damage(int _damage) // 데미지 받으면 여기로 들어와짐. 
    {
        if (requestController.Hit() == false)
            return;

        //hp -= equipController.ShieldCheck(_damage); 실험 위한 주석처리 
        hp -= _damage;
        if (hp <= 0)
        {
            animController.Die();
            Cursor.lockState = CursorLockMode.None;
            ControllCharacterLayerChange(0, 0);
            cameraController.CameraPriority = 0;
            inputController.InputActive = false;
        }
    }
    [PunRPC]
    void CallDamage(int _damage,int _actorNumber)
    {
        if (photonView.Owner.GetProperty<bool>(DefinePropertyKey.DEAD))
            return;
        hp -= inventoryController.ShieldCheck(_damage);
        if (HpBar != null)
        {
            HpBar.value = Percent(hp, maxHp);
            pv.RPC("AttackedEffect", photonView.Owner);
        }

        if (hp <= 0)
        {
            Debug.Log("you killed ");
            Player deathPlayer = photonView.Owner;
            Player lastShooterPlayer = PhotonNetwork.CurrentRoom.GetPlayer(_actorNumber);
            deathPlayer.SetProperty(DefinePropertyKey.DEAD, true);

            photonView.RPC("LogMessage", RpcTarget.All, lastShooterPlayer.NickName,deathPlayer.NickName);

            pv.RPC("MessageUp", photonView.Owner, ($"당신이 {lastShooterPlayer.NickName}에게 사망하였습니다. "));
            pv.RPC("MessageUp", lastShooterPlayer, ($"당신이 {deathPlayer.NickName}를 처치했습니다. "));

            deathPlayer.SetProperty(DefinePropertyKey.DEATH, deathPlayer.GetProperty<int>(DefinePropertyKey.DEATH) + 1);
            lastShooterPlayer.SetProperty(DefinePropertyKey.KILL, lastShooterPlayer.GetProperty<int>(DefinePropertyKey.KILL) + 1);

            sensor.StopRoutine();
            animController.Die();
            Cursor.lockState = CursorLockMode.None;
            ControllCharacterLayerChange(0, 0);
            cameraController.CameraPriority = 0;
            inputController.InputActive = false;
        }
    }

    public void Damage(int _damage, int _actorNumber) // 총알의 주인 ActorNumber 
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        photonView.RPC("CallDamage", photonView.Owner, _damage, _actorNumber);
    }

    // slider bar 조정 용 
    public float Percent(float currentHp,float maxHp)
    {
        return currentHp!=0 && maxHp !=0 ? currentHp/maxHp : 0;
    }

    [PunRPC]
    void LogMessage(string killer,string dead/*,Gun Image?*/)
    {
        Debug.Log("kill Log Updata");
       
        KillLogEntry ins = Instantiate(killLog.killInfoEntry, killLog.entryPos);
        ins.deadName.text = dead;
        ins.killerName.text = killer;
        ScrollRect rect = killLog.GetComponent<ScrollRect>();
        rect.verticalScrollbar.value = 0;
        StartCoroutine(KillLogIns(ins));
    }
    IEnumerator KillLogIns(KillLogEntry ins)
    {
        yield return new WaitForSeconds(3f);
        Destroy(ins.gameObject);
    }





    [PunRPC] //체력 회복 동기화 필요 
    public void AddHp(int _healValue)
    {
        if(hp<=0) //죽은 상황을 체크할 수 있는 변수가 있으면 그 변수를 사용하자. 
        {
            return; 
        }
        int other = maxHp - hp;
        _healValue = other < _healValue ? other : _healValue;
        hp += _healValue;
        photonView.RPC("UpdateHp", RpcTarget.Others,hp);
        photonView.RPC("AddHp", RpcTarget.Others, _healValue);
    }

    [PunRPC]
    public void UpdateHp(int newHp)
    {
        hp = newHp;
    }





    public void StartCoroutined(IEnumerator routine, ref Coroutine co)
        => this.ReStartCoroutine(routine, ref co);
    public void StopCoroutined(ref Coroutine co)
    {
        if (co != null)
            StopCoroutine(co);
    }

    public void ResettingPlayer()
    {
        animController.Alive();
        SetData();
        photonView.Controller.SetProperty(DefinePropertyKey.DEAD, false);
        sensor.StartInit();
        cameraController.Reset();
        inputController.InputActive = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(TeamCode);
            stream.SendNext(hp);
        }
        else
        {
            TeamCode = (int)stream.ReceiveNext();
            hp = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void RewindEffectOn()
    {
        mrenders = GetComponentsInChildren<MeshRenderer>();
        Debug.Log("other Rewind");
        if (photonView.IsMine)
            return;
       Instantiate(rewindEff, transform.position,Quaternion.identity);
        foreach(SkinnedMeshRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }
        foreach (MeshRenderer renderer in mrenders)
        {
            renderer.enabled = false;
        }
    }
    [PunRPC]
    public void RewindEffectOff()
    {
        mrenders = GetComponentsInChildren<MeshRenderer>();
        if (photonView.IsMine)
            return;
        Instantiate(outRewindEff, transform.position, Quaternion.identity);
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.enabled = true;
        }
        foreach (MeshRenderer renderer in mrenders)
        {
            renderer.enabled = true;
        }

    }
}
