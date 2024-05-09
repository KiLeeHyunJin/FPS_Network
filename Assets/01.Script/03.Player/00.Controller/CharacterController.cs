using Photon.Pun;
using UnityEngine;

public class CharacterController : MonoBehaviourPun
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
    bool isGround;
    bool isCrouch;
    bool isStop;
    public bool isJumping { get; private set; }
    public bool isRun { get; private set; }

    float capsuleHeight;

    const float Half = 0.5f;

    Vector3 capsuleCenter;

    CameraController cameraController;
    PlayerInputController inputController;
    CharacterAnimationController animController;
    ProcessingController processingController;
    EquipController equipController;

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

        if(groundLayer == 0)
            groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    
    void Start()
    {
        GetView();
        CheckMine();
        //InitState();
        SetData();
        CollidersSetting();
        SetKeyAction();
        SetSetting();
    }

    void SetData()
    {
        maxHp = maxHp <= 0 ? 100 : maxHp;
        hp = maxHp;
        isCrouch = false;
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
        if (mine)
        {
            Move();
        }
    }
    private void FixedUpdate()
    {
        GroundCheck();
    }

    void Move()
    {
        if (inputController.MoveValue.x != 0 || inputController.MoveValue.y != 0)
        {
            if (isStop)
                isStop = false;
            animController.MoveValue = inputController.MoveValue;
            if (inputController.Run)
                animController.MoveRun();
            else
                animController.MoveWalk();
        }
        else if (isStop == false)
        {
            animController.MoveStop();
            isStop = true;
        }
    }

    void GroundCheck()
    {
        isGround = Physics.Raycast(
            foot.transform.position + Vector3.up * 0.1f, Vector3.down,
            out RaycastHit hitInfo,
            groundCheckLength,
            groundLayer);

        if (isGround)
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            
            //Debug.DrawLine(foot.transform.position, foot.transform.position + Vector3.up, Color.red);
            //Debug.DrawLine(foot.transform.position, foot.transform.position + hitInfo.normal, Color.blue);
            if (isJumping)
            {
                if (angle < limitAngle)
                {
                    JumpFinish();
                }
                else
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
            }
            else if (angle > limitAngle)
            {
                Jump();
            }
        }
        else
        {
            if (isJumping == false)
            {
                if (isCrouch)
                    Crouch();
                Jump();
            }
        }
    }
    void JumpFinish()
    {
        animController.JumpFinish();
        isJumping = false;
    }
    void Jump()
    {
        animController.JumpStart();
        isJumping = true;
    }

    public void ScaleCapsule(bool state)
    {
        if (state)
        {
            capsule.height = capsule.height * 0.5f;
            capsule.center = capsule.center * 0.5f;
        }
        else
        {
            Ray crouchRay = new Ray(rigid.position + Vector3.up * capsule.radius * Half, Vector3.up);
            float crouchRayLength = capsuleHeight - capsule.radius * Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                return;
            capsule.height = capsuleHeight;
            capsule.center = capsuleCenter;
        }
    }
    void CallFire()
    {
        equipController.Fire();
    }
    void CallReload()
    {
        equipController.Reload();
    }
    void CallChangeFireType()
    {
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


    void CallCrouch()
    {
        if (isGround)
            Crouch();
    }
    void Crouch()
    {
        isCrouch = !isCrouch;
        animController.Crouch(isCrouch);
        if (mine)
            cameraController.CrouchState(isCrouch);
    }
    void CallJump()
    {
        if (isGround && isCrouch == false)
            rigid.velocity = new Vector3(rigid.velocity.x, jumpPower, rigid.velocity.z);
    }

    void CollidersSetting()
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
        mine = photonView.IsMine;//view.IsMine;
        inputController = gameObject.GetOrAddComponent<PlayerInputController>();
        processingController = GetComponent<ProcessingController>();
        cameraController = GetComponent<CameraController>();
        equipController = GetComponent<EquipController>();
        if (mine == false)
        {
            Destroy(inputController);
            Destroy(cameraController);
            Destroy(processingController);
        }
    }
    void SetSetting()
    {
        if (mine == false)
            return;
        cameraController.CrouchState(isCrouch);
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
