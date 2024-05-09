using Photon.Pun;
using System.Collections;
using UnityEngine;


public class AnimationController : MonoBehaviourPun, IPunObservable
{
    public enum MoveType
    { 
        Walk,
        Run,
        Stop,

        Crouch,
        Stand,

        Jump,
        JumpMove,
        JumpFinish,
        JumpSlide,
        END
    }
    public enum ValueType
    {
        Leg,
        Velocity,
        END
    }

    Animator anim;
    public Animator Anim { get { return anim; } }
    Controller controller;
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
    int JumpDirId;

    private Vector2 MoveValue { get; set; }
    private float JumpLeg { get; set ; }
    private float VelocityY { get; set; }
    public void SetMoveValue(Vector2 value)
    {
        MoveValue = value;
        anim.SetFloat(TurnId, MoveValue.x);
        anim.SetFloat(ForwardId, MoveValue.y);
    }
    public void SetJumpLeg(float value)
    {
        JumpLeg = value;
        anim.SetFloat(JumpDirId, JumpLeg);
    }
    public void SetVelocityY(float value)
    {
        VelocityY = value;
        anim.SetFloat(JumpId, VelocityY);
    }

    private void Awake()
    {
        SetAnimID();
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;
        controller = GetComponent<Controller>();
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
        JumpDirId = Animator.StringToHash("JumpDir");
    }
    private void Start()
    {
        capsuleRadius = controller.Capsule.radius;
    }
    public void JumpStart()
    {
        anim.SetBool(JumpFinishId, false);
        photonView.RPC("JumpStartRPC", RpcTarget.AllViaServer);
    }
    public void JumpMove()
    {
        photonView.RPC("JumpMoveRPC", RpcTarget.AllViaServer);
    }
    public void JumpFinish()
    {
        anim.SetBool(JumpFinishId, true);
        photonView.RPC("JumpFinishRPC", RpcTarget.AllViaServer);
    }
    public void Crouch()
    {
        anim.SetBool(CrouchId, false);
        photonView.RPC("CrouchRPC", RpcTarget.AllViaServer, false);
    }
    public void Stand()
    {
        anim.SetBool(CrouchId, true);
        photonView.RPC("CrouchRPC", RpcTarget.AllViaServer, true);
    }
    public void MoveWalk()
    {
        anim.SetBool(MoveId, true);
        anim.SetBool(RunId, false);
        photonView.RPC("MoveWalkRPC", RpcTarget.AllViaServer);
    }
    public void MoveRun()
    {
        anim.SetBool(MoveId, false);
        anim.SetBool(RunId, true);
        photonView.RPC("MoveRunRPC", RpcTarget.AllViaServer);
    }
    public void MoveStop()
    {
        anim.SetBool(MoveId, false);
        anim.SetBool(RunId, false);
        photonView.RPC("MoveStopRPC", RpcTarget.AllViaServer);
    }
    public void Die()
    {
         photonView.RPC("DieRPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void JumpStartRPC()
    {
        //anim.applyRootMotion = false;
        //
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
        //anim.applyRootMotion = true;
        //
        controller.ScaleCapsule(false);
        controller.Capsule.radius = capsuleRadius;
    }

    [PunRPC]
    void CrouchRPC(bool state)
    {
        anim.SetTrigger(ChangeEnterId);
        controller.ScaleCapsule(state);
    }

    [PunRPC]
    void MoveWalkRPC()
    {
        //if (isMove == false)
        //{
        //    isMove = true;
        //    anim.SetBool(MoveId, true);
        //}
        //if (isRun)
        //{
        //    isRun = false;
        //    anim.SetBool(RunId, false);
        //}
        //StartCoroutine(DampingAnimation(ForwardId, MoveValue.y, dampingValue));
        //StartCoroutine(DampingAnimation(TurnId, MoveValue.x, dampingValue));

        Debug.Log($"Forward {MoveValue.y}");
        Debug.Log($"Turn {MoveValue.x}");
    }

    [PunRPC]
    void MoveRunRPC()
    {
        //if (isMove == false)
        //{
        //    isMove = true;
        //    anim.SetBool(MoveId, true);
        //}
        //if (isRun == false)
        //{
        //    isRun = true;
        //    anim.SetBool(RunId, true);
        //}
        //StartCoroutine(DampingAnimation(ForwardId, MoveValue.y, dampingValue));
       //StartCoroutine(DampingAnimation(TurnId, MoveValue.x, dampingValue));

    }

    [PunRPC]
    void MoveStopRPC()
    {
        if (isMove)
        {
            isMove = false;
            //anim.SetBool(MoveId, false);
        }
        if (isRun)
        {
            isRun = false;
            //anim.SetBool(RunId, false);
        }
        //StartCoroutine(DampingAnimation(ForwardId,0, dampingValue * 0.5f));
        //StartCoroutine(DampingAnimation(TurnId, 0, dampingValue * 0.5f));
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
    IEnumerator DampingAnimation(int _id, float _value,float _dampingValue)
    {
        float time = 0;
        while(1 >= time)
        {
            time += Time.deltaTime;
            anim.SetFloat(_id, _value, _dampingValue, Time.deltaTime);
            yield return null;
        }
    }
}
