using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

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
    [Range(1f,3f)]
    [SerializeField] float gravitySpeed;
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
    [SerializeField] CharacterTransformProcess moveProcess;
    
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
        GetView(); //���� ������Ʈ ��������
        CheckMine(); //���� ��ü�� �ƴ϶�� �Է�Ű ����
        SetData(); //������ �ʱ�ȭ
        CollidersSetting(); //�浹���� ����
        SetKeyAction(); //�Է¿� ���� �Լ� ����
        MoveProcessInit();
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
    void SetData()
    {
        maxHp = maxHp <= 0 ? 100 : maxHp;
        hp = maxHp;
    }

    void CollidersSetting() //하위 객체를 돌며 충돌체가 있다면 6번 레이어로 변경 및 히트박스 부착
    {
        //PhotonNetwork.CurrentRoom.
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
        => moveProcess?.Update();
    void FixedUpdate() 
        => moveProcess?.FixedUpdate();

    void CallFire() 
        => equipController.Fire();
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
    }
    void CallOne()
    {
        animController.ChangeRifle();
    }
    void CallTwo()
    {
        animController.ChangePistol();
    }
    void CallThree()
    {
        animController.ChangeSword();
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
    void MoveProcessInit()
    {
        if (mine == false)
            return;
        moveProcess = new CharacterTransformProcess();
        moveProcess.Init(GetComponent<CharacterController>());
        moveProcess.InitGroundCheckData(foot.transform, groundCheckLength, ignoreGroundCheckLength, groundLayer, jumpHeight, gravitySpeed);

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
        inputController.SetMoveType(moveProcess.SetMoveType);

        inputController.SetKey(moveProcess.Crouch, Define.Key.C);
        inputController.SetKey(moveProcess.Jump, Define.Key.Space);

        moveProcess.SetMoveSpeed(walkStandSpeed, runStandSpeed, walkCrouchSpeed,runCrouchSpeed);
        moveProcess.Start();
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
            Destroy(inputController.gameObject);
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

}
