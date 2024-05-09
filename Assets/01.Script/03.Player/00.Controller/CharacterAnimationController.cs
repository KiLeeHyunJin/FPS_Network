using Photon.Pun;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviourPun,IPunObservable
{
    Animator anim;
    public Animator Anim { get { return anim; } }
    CharacterController controller;
    [SerializeField] float dampingValue;

    float capsuleRadius;

    bool isRun;
    bool isMove;
    int JumpFinishId;
    int JumpEnterId;
    int ForwardId;
    int JumpId;
    int CrouchId;
    int ChangeEnterId;
    int MoveId;
    int RunId;
    int TurnId;

    public Vector2 MoveValue { get; set; }
    public float JumpLeg { get; set; }
    public float VelocityY { get; set; }

    private void Awake()
    {
        SetAnimID();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        dampingValue = dampingValue <= 0 ? 0.15f : dampingValue;
    }
    void SetAnimID()
    {
        JumpFinishId = Animator.StringToHash("JumpFinish");
        JumpEnterId = Animator.StringToHash("JumpEnter");
        ForwardId = Animator.StringToHash("Forward");
        JumpId = Animator.StringToHash("Jump");
        CrouchId = Animator.StringToHash("Crouch");
        ChangeEnterId = Animator.StringToHash("ChangeEnter");
        MoveId = Animator.StringToHash("Move");
        RunId = Animator.StringToHash("Run");
        TurnId = Animator.StringToHash("Turn");
    }
    private void Start()
    {
        capsuleRadius = controller.Capsule.radius;
        anim.applyRootMotion = photonView.IsMine;
    }
    public void JumpStart()
    {
        photonView.RPC("JumpStartRPC",RpcTarget.AllViaServer);
    }
    public void JumpMove()
    {
        photonView.RPC("JumpMoveRPC", RpcTarget.AllViaServer);
    }
    public void JumpFinish()
    {
        photonView.RPC("JumpFinishRPC", RpcTarget.AllViaServer);
    }
    public void Crouch(bool state)
    {
        photonView.RPC("CrouchRPC", RpcTarget.AllViaServer, state);
    }
    public void MoveWalk()
    {
        photonView.RPC("MoveWalkRPC", RpcTarget.AllViaServer);
    }
    public void MoveRun()
    {
        photonView.RPC("MoveRunRPC", RpcTarget.AllViaServer);
    }
    public void MoveStop()
    {
        photonView.RPC("MoveStopRPC", RpcTarget.AllViaServer);
    }
    public void Die()
    {
        photonView.RPC("DieRPC", RpcTarget.AllViaServer);
    }
    [PunRPC]
    void JumpStartRPC()
    {
        anim.applyRootMotion = false;

        anim.SetBool(JumpFinishId, false);
        anim.SetTrigger(JumpEnterId);
        controller.ScaleCapsule(true);
        controller.Capsule.radius = 0.05f;
    }

    [PunRPC]
    void JumpMoveRPC()
    {
        anim.SetFloat(ForwardId, JumpLeg);
        anim.SetFloat(JumpId, VelocityY);
    }

    [PunRPC]
    void JumpFinishRPC()
    {
        anim.applyRootMotion = true;
        anim.SetBool(JumpFinishId, true);
        controller.ScaleCapsule(false);
        controller.Capsule.radius = capsuleRadius;
    }

    [PunRPC]
    void CrouchRPC(bool state)
    {
        anim.SetBool(CrouchId, state);
        anim.SetTrigger(ChangeEnterId);
        controller.ScaleCapsule(state);
    }

    [PunRPC]
    void MoveWalkRPC()
    {
        if (isMove == false)
        {
            isMove = true;
            anim.SetBool(MoveId, true);
        }
        if (isRun)
        {
            isRun = false;
            anim.SetBool(RunId, false);
        }
        anim.SetFloat(ForwardId, MoveValue.y, dampingValue, Time.deltaTime);
        anim.SetFloat(TurnId, MoveValue.x, dampingValue, Time.deltaTime);
    }

    [PunRPC]
    void MoveRunRPC()
    {
        if (isMove == false)
        {
            isMove = true;
            anim.SetBool(MoveId, true);
        }
        if (isRun == false)
        {
            isRun = true;
            anim.SetBool(RunId, true);
        }
        anim.SetFloat(ForwardId, MoveValue.y, dampingValue, Time.deltaTime);
        anim.SetFloat(TurnId, MoveValue.x, dampingValue, Time.deltaTime);
    }

    [PunRPC]
    void MoveStopRPC()
    {
        if (isMove)
        {
            isMove = false;
            anim.SetBool(MoveId, false);
        }
        if (isRun)
        {
            isRun = false;
            anim.SetBool(RunId, false);
        }
        anim.SetFloat(ForwardId, 0, dampingValue * 0.5f, Time.deltaTime);
        anim.SetFloat(TurnId, 0, dampingValue * 0.5f, Time.deltaTime);
    }

    [PunRPC]
    void DieRPC()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(MoveValue);
            stream.SendNext(JumpLeg);
            stream.SendNext(VelocityY);
        }
        else
        {
            MoveValue = (Vector2)stream.ReceiveNext();
            JumpLeg = (float)stream.ReceiveNext();
            VelocityY = (float)stream.ReceiveNext();
        }
    }
}
