using Photon.Pun;
using System.Collections;
using UnityEngine;


public class AnimationController : MonoBehaviourPun//, IPunObservable
{
    Animator anim;
    public Animator Anim { get { return anim; } }
    [SerializeField] float dampingValue;

    int JumpEnterId;
    int ChangeEnterId;

    //byte[] Pos;
    //byte layer;
    Vector3 motionValue;

    short[] changedBitPos;
    int[] layerId;
    int[] floatId;
    Coroutine[] dampingCo;
    //void SetState(AnimatorState type, bool state)
    //{
    //    int idx = (int)type;
    //    if (state)
    //        layer |= (byte)(1 << idx);
    //    else
    //        layer &= (byte)~(1 << idx);
    //    anim.SetBool(layerId[(int)type], state);
    //}
    public Vector2 MoveValue
    {
        set
        {
            //SetValue(AnimatorFloatValue.X, value.x);
            //SetValue(AnimatorFloatValue.Z, value.y);
            this.ReStartCoroutine(DampingAnimationRoutine(value.x, AnimatorFloatValue.X), ref dampingCo[(int)AnimatorFloatValue.X]);
            this.ReStartCoroutine(DampingAnimationRoutine(value.y, AnimatorFloatValue.Z), ref dampingCo[(int)AnimatorFloatValue.Z]);
        }
    }
    public float VelocityY
    {
        set
        {
            //SetValue(AnimatorFloatValue.Y, value);
            this.ReStartCoroutine(DampingAnimationRoutine(value, AnimatorFloatValue.Y), ref dampingCo[(int)AnimatorFloatValue.Y]);
        }
    }
    //void SaveValue(AnimatorFloatValue type, float _value)
    //{
    //    byte answard;
    //    byte lockValue = AnimatorFloatValue.Y == type ? (byte)10 : (byte)100;
    //    if (_value < 0)
    //    {
    //        answard = byte.MaxValue;
    //        answard += _value < -1 ? (byte)-lockValue : (byte)(_value * lockValue);
    //    }
    //    else
    //    {
    //        answard = _value > 1 ? lockValue : (byte)(_value * 100);
    //    }
    //    Pos[(int)type] = answard;
    //}
    //void SetValue(AnimatorFloatValue type, float _value)
    //{
    //    this.ReStartCoroutine(DampingAnimationRoutine(_value, floatId[(int)type], type), ref dampingCo[(int)type]);
    //    SaveValue(type, _value);
    //}
    public void Crouch(bool state)
    {
        SetState(AnimatorState.Crouch, state);
        photonView.RPC("CrouchRPC", RpcTarget.AllViaServer);
    }

    public void JumpStart()
    {
        SetState(AnimatorState.JumpFinish, false);
        photonView.RPC("JumpRPC", RpcTarget.AllViaServer);
    }

    public void Die()
    {
    }
    public void MoveRun()
    {
        SetState(AnimatorState.Move, true);
        SetState(AnimatorState.Run, true);
    }
    public void MoveWalk()
    {
        SetState(AnimatorState.Move, true);
        SetState(AnimatorState.Run, false);
    }
    public void MoveStop()
    {
        SetState(AnimatorState.Move, false);
        SetState(AnimatorState.Run, false);
    }
    public void JumpFinish()
    {
        SetState(AnimatorState.JumpFinish, true);
    }
    void SetState(AnimatorState type, bool state)
    {
        anim.SetBool(layerId[(int)type], state);
    }
    private void Awake()
    {
        SetAnimID();
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;
        dampingValue = dampingValue <= 0 ? 0.15f : dampingValue;
        dampingValue = 1 / dampingValue;
    }
    void SetAnimID()
    {
        JumpEnterId = Animator.StringToHash("JumpEnter");
        ChangeEnterId = Animator.StringToHash("ChangeEnter");

        int JumpFinishId = Animator.StringToHash("JumpFinish");
        int ForwardId = Animator.StringToHash("Forward");
        int JumpId = Animator.StringToHash("Jump");
        int CrouchId = Animator.StringToHash("Crouch");
        int MoveId = Animator.StringToHash("Move");
        int RunId = Animator.StringToHash("Run");
        int TurnId = Animator.StringToHash("Turn");

        floatId = new int[] { TurnId, JumpId, ForwardId };
        layerId = new int[] { MoveId, RunId, CrouchId, JumpFinishId };
        dampingCo = new Coroutine[(int)AnimatorFloatValue.END];
    }
    //private void Start()
    //{
    //    Pos = new byte[(int)AnimatorFloatValue.END];
    //    changedBitPos = new short[9];
    //    layer = 0;
    //}

    //void UpdateState(byte _inputValue)
    //{
    //    for (int i = 0; i < Comparison(_inputValue); i++)
    //        anim.SetBool(layerId[changedBitPos[i]], !GetState(changedBitPos[i]));
    //    layer = _inputValue;
    //}

    //int Comparison(byte _inputValue)
    //{
    //    byte answard = (byte)(layer ^ _inputValue);
    //    int count = 0;
    //    for (short i = 0; i < 8; i++)
    //    {
    //        if ((answard & (1 << i)) != 0)
    //        {
    //            changedBitPos[count] = i;
    //            count++;
    //        }
    //    }
    //    return count;
    //}
    //bool GetState(short idx)
    //{
    //    byte sour = layer;
    //    sour &= (byte)(1 << idx);
    //    return sour != 0;
    //}

    //void UpdatePos(AnimatorFloatValue type, byte value)
    //{
    //    Pos[(int)type] = value;
    //    float hashValue = GetValue(type);
    //    anim.SetFloat(floatId[(int)type], hashValue);
    //}
    //float GetValue(AnimatorFloatValue type)
    //{
    //    float value = Pos[(int)type];
    //    if(value > 154)
    //        value = ((byte.MaxValue - value) * -1);
    //    if (type == AnimatorFloatValue.Y)
    //        return value * 0.1f;
    //    return value * 0.01f;
    //}

    [PunRPC]
    void CrouchRPC()
    {
        anim.SetTrigger(ChangeEnterId);
    }
    [PunRPC]
    void JumpRPC()
    {
        anim.SetTrigger(JumpEnterId);
    }
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(Pos[(int)AnimatorFloatValue.X]);
    //        stream.SendNext(Pos[(int)AnimatorFloatValue.Y]);
    //        stream.SendNext(Pos[(int)AnimatorFloatValue.Z]);
    //        stream.SendNext(layer);
    //    }
    //    else
    //    {
    //        UpdatePos(AnimatorFloatValue.X, (byte)stream.ReceiveNext());
    //        UpdatePos(AnimatorFloatValue.Y, (byte)stream.ReceiveNext());
    //        UpdatePos(AnimatorFloatValue.Z, (byte)stream.ReceiveNext());
    //        UpdateState((byte)stream.ReceiveNext());
    //    }
    //}

    IEnumerator DampingAnimationRoutine(float value, AnimatorFloatValue type)
    {
        float startValue = anim.GetFloat((int)type);
        float time = 0;
        float currentValue;
        while (time <= 1)
        {
            time += Time.deltaTime * dampingValue;
            currentValue = Mathf.Lerp(startValue, value, time);
            anim.SetFloat(floatId[(int)type], currentValue);
            //SaveValue(type, currentValue);
            yield return null;
        }
    }
    public enum AnimatorState
    {
        Move,
        Run,
        Crouch,
        JumpFinish,
        END
    }

    public enum AnimatorFloatValue
    { X, Y, Z, END }
    public enum MoveType
    {
        Walk,
        Run,
        Stop,

        Crouch,
        Stand,

        Jump,
        JumpFinish,
        JumpSlide,
        END
    }
}
