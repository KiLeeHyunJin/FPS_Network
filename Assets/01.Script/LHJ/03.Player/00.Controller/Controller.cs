using Cinemachine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviourPun, IPunObservable
{
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

    [SerializeField] GameObject rootBone;
    [SerializeField] GameObject foot;

    [SerializeField] float mouseSensitivity;
    [SerializeField] GameObject[] FPSIgnoreObject;
    [SerializeField] GameObject[] FPSHand;
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



    int maxHp;
    [SerializeField] int hp;
    public bool Mine { get; private set; }

    public int TeamCode { get; private set; }

    AttackProcess attackProcess;
    CameraController cameraController;
    PlayerInputController inputController;
    CharacterTransformProcess moveProcess;

    EquipController equipController;
    AnimationController animController;
    ProcessingController processingController;

    RequestController requestController;
    event Action Updates;
    private void Awake()
    {
        animController = gameObject.GetOrAddComponent<AnimationController>();
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }
    void Start()
    {
        if (PhotonNetwork.InRoom)
            Check();
        else
            Destroy(gameObject);
    }

    void Check()
    {
        CheckMine();
        CollidersSetting();
        SetKeyAction();
        MoveProcessInit();
        SetUpdateAction();
        CheckRig();
       
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

        equipController = GetComponent<EquipController>();
        requestController = GetComponent<RequestController>();
        cameraController = new CameraController(target, this, cam, cameraRoot);

        minimapIcon_m.SetActive(true);

        if (Mine == false)
        {
            Destroy(enemySearcher);
            minimapIcon_m.SetActive(false);
            cam.gameObject.SetActive(false);
            Destroy(miniCam);
            Destroy(inputController);
            Destroy(processingController);

            if (TryGetComponent<PlayerInput>(out var input))
                Destroy(input);
            if (teamCode == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            {
                Destroy(enemyIcon);
                minimapIcon_Ally.SetActive(true);
            }
                

            return;
        }
        cameraController.Init(ControllCharacterLayerChange, overlayCam, mouseSensitivity);

        attackProcess = new AttackProcess(this);
        inputController.Owner = this;
        SetData();
    }

    void SetData()
    {
        maxHp = maxHp <= 0 ? 100 : maxHp;
        if (photonView.Controller.GetPhotonTeam() != null)
            teamCode = photonView.Controller.GetPhotonTeam().Code;
        else
            teamCode = 0;
        hp = maxHp;
    }

    void CollidersSetting() //하위 객체를 돌며 충돌체가 있다면 6번 레이어로 변경 및 히트박스 부착
    {
        Player pl = PhotonNetwork.LocalPlayer;
        Debug.Log($"LocalPlayer : {pl.ActorNumber},Controller : {photonView.Controller.ActorNumber} ");

        Collider[] colliders = rootBone.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
            collider.gameObject.AddComponent<HitBox>().SetOwner(this, Mine);
    }

    void Update()
        => Updates?.Invoke();
    void FixedUpdate()
        => moveProcess?.FixedUpdate();

    void CallFire()
    {
        equipController.Fire();
        requestController.Fire();
        animController.Atck();
        cameraController.GetCamShakeRoutine();
        Controller hitTarget = attackProcess?.Attack();
    }
    void CallReload()
    {
        animController.Reload();
        equipController.Reload();
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
        animController.ChangeWeapon(AnimationController.AnimatorWeapon.Rifle);
    }
    void CallTwo()
    {
        animController.ChangeWeapon(AnimationController.AnimatorWeapon.Pistol);
        inputController.ChangeFireType = Define.FireType.One;
    }
    void CallThree()
    {
        animController.ChangeWeapon(AnimationController.AnimatorWeapon.Sword);
        inputController.ChangeFireType = Define.FireType.One;
    }
    void CheckRig()
    {

    }

    void SetKeyAction()
    {
        if (Mine == false)
            return;
        inputController.Init();
        inputController.SetKey(CallReload, Define.Key.R);
        inputController.SetKey(CallOne, Define.Key.F1);
        inputController.SetKey(CallTwo, Define.Key.F2);
        inputController.SetKey(CallThree, Define.Key.F3);
        inputController.SetKey(CallFire, Define.Key.Press);
        inputController.SetKey(CallChangeFireType, Define.Key.V);
        
    }
    void MoveProcessInit()
    {
        if (Mine == false)
            return;
        moveProcess = new CharacterTransformProcess();
        moveProcess.Init(GetComponent<CharacterController>(), Mine);
        moveProcess.InitGroundCheckData(foot.transform, groundCheckLength, ignoreGroundCheckLength, groundLayer, jumpHeight, gravitySpeed);

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

    public void Damage(int _damage)
    {
        if (requestController.Hit() == false)
            return;

        hp -= equipController.ShieldCheck(_damage);
        if (hp <= 0)
        {
            animController.Die();
            Cursor.lockState = CursorLockMode.None;
            ControllCharacterLayerChange(0,0);
            cameraController.CameraPriority = 0;
            inputController.InputActive = false;
        }
    }

    public void AddHp(int _healValue)
    {
        int other = maxHp - hp;
        _healValue = other < _healValue ? other : _healValue;
        hp += _healValue;
    }


    public void StartCoroutined(IEnumerator routine, ref Coroutine co)
        => this.ReStartCoroutine(routine, ref co);
    public void StopCoroutined(ref Coroutine co)
    {
        if (co != null)
            StopCoroutine(co);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(TeamCode);

        }
        else
        {
            TeamCode = (int)stream.ReceiveNext();
        }
    }
}
