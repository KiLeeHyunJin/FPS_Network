using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviourPun
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
    [SerializeField] Transform target;
    [SerializeField] FPSCameraPosition cameraRoot;
    [SerializeField] CinemachineVirtualCamera cam;

    int maxHp;
    int hp;
    bool mine;
    AttackProcess attackProcess;
    CameraController cameraController;
    PlayerInputController inputController;

    AnimationController animController;
    ProcessingController processingController;
    EquipController equipController;
    CharacterTransformProcess moveProcess;
    RequestController requestController;
    IKAnimationController IKAnimationController;
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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(foot.transform.position, GetComponent<CharacterController>().radius);
    }
    void Check()
    {
        GetView();
        CheckMine();
        SetData();
        CollidersSetting();
        SetKeyAction();
        MoveProcessInit();
        //SetRequest();
        if(mine)
        {
            Updates -= cameraController.Update;
            Updates += cameraController.Update;
            Updates -= moveProcess.Update;
            Updates += moveProcess.Update;
        }
    }

    void CheckMine()
    {
        mine = photonView.IsMine;
        inputController = gameObject.GetOrAddComponent<PlayerInputController>();
        processingController = GetComponent<ProcessingController>();

        equipController = GetComponent<EquipController>();
        requestController = GetComponent<RequestController>();

        if (mine == false)
        {
            Destroy(inputController);
            Destroy(processingController);
            IKAnimationController = new IKAnimationController();
            if (TryGetComponent<PlayerInput>(out var input))
                Destroy(input);
            return;
        }
        cameraController = new CameraController(target, this, cameraRoot, cam, mouseSensitivity);
        cameraController.Init(ControllCharacterLayerChange);

        attackProcess = new AttackProcess(this);
        inputController.Owner = this;
    }

    void SetData()
    {
        maxHp = maxHp <= 0 ? 100 : maxHp;
        hp = maxHp;
    }

    void CollidersSetting() //하위 객체를 돌며 충돌체가 있다면 6번 레이어로 변경 및 히트박스 부착
    {
        Player pl = PhotonNetwork.LocalPlayer;
        Debug.Log($"LocalPlayer : {pl.ActorNumber},Controller : {photonView.Controller.ActorNumber} ");
        Collider[] colliders = rootBone.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.layer = 6;
            colliders[i].gameObject.AddComponent<HitBox>().SetOwner(this);
        }
    }

    void Update()
    {
        Updates?.Invoke();
    }
    void FixedUpdate()
    {
        moveProcess?.FixedUpdate();
    }

    void CallFire()
    {
        equipController.Fire();
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

    void SetRequest()
    {
        requestController.SetMove(moveProcess.SetMoveValue);
        requestController.SetJump(moveProcess.Jump);
    }

    void SetKeyAction()
    {
        if (mine == false)
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
        if (mine == false)
            return;
        moveProcess = new CharacterTransformProcess();
        moveProcess.Init(GetComponent<CharacterController>(), mine);
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
        inputController.SetKey(moveProcess.Jump, Define.Key.Space);
        //inputController.SetKey(() => { requestController.photonView.RPC("RequestJump", RpcTarget.MasterClient); }, Define.Key.Space);
        //inputController.SetMoveKey(v => requestController.photonView.RPC("RequestMove",RpcTarget.MasterClient, v.x, v.y));
        inputController.SetMoveType(moveProcess.SetMoveType);
        inputController.SetKey(moveProcess.Crouch, Define.Key.C);

        moveProcess.SetMoveSpeed(walkStandSpeed, runStandSpeed, walkCrouchSpeed, runCrouchSpeed);
        moveProcess.Start();
    }

    void ControllCharacterLayerChange(int layerNum)
    {
        for (int i = 0; i < FPSIgnoreObject.Length; i++) //레이어 재설정
        {
            if (FPSIgnoreObject[i] == null)
                continue;
            //하위 객체들 또한 전부 레이어 재설정
            foreach (Transform childeGameObject in FPSIgnoreObject[i].GetComponentsInChildren<Transform>())
                childeGameObject.gameObject.layer = layerNum;
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

    public void Damage(int _damage)
    {
        hp -= equipController.ShieldCheck(_damage);
        if (hp <= 0)
        {
            animController.Die();
            Cursor.lockState = CursorLockMode.None;
            ControllCharacterLayerChange(0);
            Destroy(cameraRoot.gameObject); //컨트롤 파괴시 시네머신 카메라도 같이 파괴
            Destroy(inputController.gameObject);
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
        if(co != null)
            StopCoroutine(co);
    }


    PhotonView view;
    PhotonAnimatorView animatorView;
    PhotonTransformView transformView;
    void GetView()
    {
        view = gameObject.GetOrAddComponent<PhotonView>();
        animatorView = gameObject.GetOrAddComponent<PhotonAnimatorView>();
        transformView = gameObject.GetOrAddComponent<PhotonTransformView>();
    }

}
