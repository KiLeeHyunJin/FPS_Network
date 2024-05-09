using Photon.Pun;
using UnityEngine;

public class Controller : MonoBehaviourPun
{
    [Range(10, 90)]
    [SerializeField] int limitAngle;
    [Range(0.05f, 0.5f)]
    [SerializeField] float groundCheckLength;
    [Range(2, 10f)]
    [SerializeField] float jumpPower;
    [SerializeField] LayerMask groundLayer;


    [SerializeField] GameObject rootBone;
    [SerializeField] GameObject foot;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;

    int maxHp;
    int hp;

    bool mine;


    float capsuleHeight;

    

    Vector3 capsuleCenter;

    CameraController cameraController;
    PlayerInputController inputController;
    CharacterAnimationController animController;
    ProcessingController processingController;
    EquipController equipController;
    CharacterTransformProcess moveProcess;
    Rigidbody rigid;

    CapsuleCollider capsule { get; set; }
    public CapsuleCollider Capsule { get { return capsule; } }

    PhotonView view;
    PhotonAnimatorView animView;
    PhotonTransformView transformView;

    [SerializeField] CharacterDeath death;
    [SerializeField] CharacterIdle idle;
    [SerializeField] CharacterWalk walk;
    [SerializeField] CharacterRun run;
    [SerializeField] CharacterCrouch crouch;
    [SerializeField] CharacterClim clim;
    [SerializeField] CharacterFire fire;
    [SerializeField] CharacterReload reload;
    [SerializeField] CharacterPickUp pick;
    [SerializeField] CharacterJump jump;
    [SerializeField] CharacterInteraction interaction;
    public BaseStateMachine<Define.State> FSM
    { get { return mine ? fsm : null; } }
    private BaseStateMachine<Define.State> fsm
    { get; set; }

    private void Awake()
    {
        animController = gameObject.GetOrAddComponent<CharacterAnimationController>();
        rigid = gameObject.GetOrAddComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        moveProcess = new CharacterTransformProcess(GetComponent<CharacterController>());
        moveProcess.SetActions(JumpFinish, Jump, Crouch, SlideJump);
        moveProcess.Init(foot, groundCheckLength, groundLayer, limitAngle);

        groundLayer = groundLayer == 0 ? 1 << LayerMask.NameToLayer("Ground") : groundLayer;
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
        GetView(); //포톤 컴포넌트 가져오기
        CheckMine(); //본인 객체가 아니라면 입력키 삭제
        SetData(); //데이터 초기화
        CollidersSetting(); //충돌범위 지정
        SetKeyAction(); //입력에 따른 함수 연결
    }

    void SetData()
    {
        maxHp = maxHp <= 0 ? 100 : maxHp;
        hp = maxHp;
        //isCrouch = false;
        capsuleHeight = capsule.height;
        capsuleCenter = capsule.center;

        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void InitState()
    {
        if (mine == false)
            return;
        fsm = new BaseStateMachine<Define.State>();

        SetState(idle, Define.State.Idle);
        SetState(death, Define.State.Death);
        SetState(walk, Define.State.Walk);
        SetState(run, Define.State.Run);
        SetState(crouch, Define.State.Crouch);
        SetState(clim, Define.State.Clim);
        SetState(fire, Define.State.Fire);
        SetState(reload, Define.State.Reload);
        SetState(pick, Define.State.Pick);
        SetState(jump, Define.State.Jump);
        SetState(interaction, Define.State.Interaction);

        FSM.Start(Define.State.Idle);
    }

    void SetState(CharacterStateBase stateBase, Define.State stateEnum)
    {
        stateBase.Controller = this;
        FSM.AddState(stateEnum, stateBase);
    }

    void Update()
    {
        moveProcess.Update();
    }
    private void FixedUpdate()
    {
        moveProcess.FixedUpdate();
    }

    public void ScaleCapsule(bool state)
    {
        if (state) //앉기
        {
            capsule.height = capsuleHeight * 0.5f;
            capsule.center = capsuleCenter * 0.5f;
        }
        else //일어서기
        {
            capsule.height = capsuleHeight;
            capsule.center = capsuleCenter;
        }
    }

    void JumpFinish()
    {
        animController.JumpFinish();
    }
    void Jump()
    {
        animController.JumpStart();
    }
    void SlideJump()
    {
        float runCycle =
                   Mathf.Repeat(
                   animController.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1);
        float dir = runCycle < 0.5f ? 1 : -1;
        float jumpLeg = dir * inputController.MoveValue.y;
        animController.JumpLeg = jumpLeg;
        animController.VelocityY = rigid.velocity.y;
        animController.JumpMove();
    }


    void CallCrouch()
    {
        if (moveProcess.isGround)
            Crouch();
    }
    void Crouch()
    {
        moveProcess.SetCrouch = !moveProcess.isCrouch;
        animController.Crouch(moveProcess.isCrouch);
    }
    void CallJump()
    {
        if (moveProcess.isGround && moveProcess.isCrouch == false)
            rigid.velocity = new Vector3(rigid.velocity.x, jumpPower, rigid.velocity.z);
    }
    void CallFire()
    {
        equipController?.Fire();
    }
    void CallReload()
    {
        equipController?.Reload();
    }
    void CallChangeFireType()
    {
        if (equipController != null)
            inputController.ChangeFireType = equipController.FireTypeChange();
    }
    void CallOne()
    {
    }
    void CallTwo()
    {
    }
    void CallThree()
    {
    }
    void CollidersSetting() //하위 객체를 돌며 충돌체가 있다면 6번 레이어로 변경 및 히트박스 부착
    {
        Collider[] colliders = rootBone.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.layer = 6;
            colliders[i].gameObject.AddComponent<HitBox>().SetOwner(this);
        }
    }
    void GetView()
    {
        view = gameObject.GetOrAddComponent<PhotonView>();
        animView = gameObject.GetOrAddComponent<PhotonAnimatorView>();
        transformView = gameObject.GetOrAddComponent<PhotonTransformView>();
    }

    void CheckMine()
    {
        mine = photonView.IsMine;
        inputController = gameObject.GetOrAddComponent<PlayerInputController>();
        processingController = GetComponent<ProcessingController>();
        cameraController = GetComponent<CameraController>();
        equipController = GetComponent<EquipController>();
        if (mine == false)
        {
            Destroy(inputController);
            Destroy(cameraController);
            Destroy(processingController);
            return;
        }
        else
        {
            cameraController.Init();
        }
    }

    void SetKeyAction()
    {
        if (mine == false)
            return;

        inputController.SetKey(CallCrouch, Define.Key.C);
        inputController.SetKey(CallJump, Define.Key.Space);
        inputController.SetKey(CallReload, Define.Key.R);
        inputController.SetKey(CallOne, Define.Key.F1);
        inputController.SetKey(CallTwo, Define.Key.F2);
        inputController.SetKey(CallThree, Define.Key.F3);
        inputController.SetKey(CallFire, Define.Key.Press);
        inputController.SetKey(CallChangeFireType, Define.Key.V);
        inputController.SetMoveKey(moveProcess.Move);
    }
    public void Damage(int _damage)
    {
        hp -= equipController.ShieldCheck(_damage);
        if (hp <= 0)
        {
            animController.Die();
            Destroy(inputController.gameObject);
            FSM.ChangeState(Define.State.Death);
        }
    }
}
