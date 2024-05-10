using Photon.Pun;
using System;
using UnityEngine;

public class Controller : MonoBehaviourPun
{
    [Range(10, 90)]
    [SerializeField] int limitAngle;
    [Range(0.05f, 0.5f)]
    [SerializeField] float groundCheckLength;
    [Range(2, 10f)]
    [SerializeField] float jumpPower;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] GameObject rootBone;
    [SerializeField] GameObject foot;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;

    int maxHp;
    int hp;
    bool mine;

    CameraController cameraController;
    PlayerInputController inputController;
    AnimationController animController;
    ProcessingController processingController;
    EquipController equipController;
    CharacterTransformProcess moveProcess;

    public BaseStateMachine<Define.State> FSM
    { get { return mine ? fsm : null; } }
    private BaseStateMachine<Define.State> fsm
    { get; set; }

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
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 start = foot.transform.position + new Vector3(0,1,0) * 0.1f;
        Vector3 end = start + new Vector3(0, -groundCheckLength, 0);

        Gizmos.DrawLine(start, end);
    }
    void Check()
    {
        GetView(); //���� ������Ʈ ��������
        CheckMine(); //���� ��ü�� �ƴ϶�� �Է�Ű ����
        SetData(); //������ �ʱ�ȭ
        CollidersSetting(); //�浹���� ����
        SetKeyAction(); //�Է¿� ���� �Լ� ����
        MoveProcessInit();
    }

    void SetData()
    {
        maxHp = maxHp <= 0 ? 100 : maxHp;
        hp = maxHp;
    }

    void SetState(CharacterStateBase stateBase, Define.State stateEnum)
    {
        stateBase.Controller = this;
        FSM.AddState(stateEnum, stateBase);
    }

    void Update()
    {
        moveProcess?.Update();
    }
    private void FixedUpdate()
    {
        moveProcess?.FixedUpdate();
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
        cameraController.Init();
    }
    void MoveProcessInit()
    {
        moveProcess = new CharacterTransformProcess(GetComponent<CharacterController>());

        moveProcess.Init(foot, groundCheckLength, groundLayer, limitAngle);

        moveProcess.SetMotions(AnimationController.MoveType.Run, animController.MoveRun);
        moveProcess.SetMotions(AnimationController.MoveType.Walk, animController.MoveWalk);
        moveProcess.SetMotions(AnimationController.MoveType.Stop, animController.MoveStop);

        moveProcess.SetMotions(AnimationController.MoveType.JumpFinish, animController.JumpFinish);
        moveProcess.SetMotions(AnimationController.MoveType.Jump, animController.JumpStart);
        moveProcess.SetMotions(AnimationController.MoveType.JumpSlide, SlideJump);

        moveProcess.SetMotions(AnimationController.MoveType.Crouch,() => animController.Crouch(true));
        moveProcess.SetMotions(AnimationController.MoveType.Stand, () => animController.Crouch(false));

        moveProcess.SetMoveActionValue( v => animController.VelocityY = v);
        moveProcess.SetMoveActionValue( v => animController.MoveValue = v);

        inputController.SetMoveKey(moveProcess.SetMoveValue);
        inputController.SetKey(moveProcess.Crouch, Define.Key.C);
        inputController.SetKey(moveProcess.Jump, Define.Key.Space);
        inputController.SetMoveType(moveProcess.SetMoveType);

        moveProcess.SetMoveSpeed(walkSpeed, runSpeed, crouchSpeed);
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

    void SetKeyAction()
    {
        if (mine == false)
            return;

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
    public void AddHp(int _healValue)
    {
        int other = maxHp - hp;
        _healValue = other < _healValue ? other : _healValue;
        hp += _healValue;
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
}
