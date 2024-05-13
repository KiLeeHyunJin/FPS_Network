using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using static Define;


public class AnimationController : MonoBehaviourPun//, IPunObservable
{
    Animator anim;
    public Animator Anim { get { return anim; } }
    [SerializeField] float dampingSpeed;

    int JumpEnterId;
    int StandId;
    int CrouchId;
    int ChangeWeaponId;
    int ReloadId;
    int FireId;
    //byte[] Pos;
    //byte layer;
    //short[] changedBitPos;

    int[] layerId;
    int[] floatId;
    int[] weaponId;
    readonly string STAND = "StandRPC";
    readonly string CROUCH = "CrouchRPC";
    readonly string JUMP = "JumpRPC";
    readonly string CHANGEWEAPON = "ChangeWeaponRPC";
    readonly string RELOAD = "ReloadRPC";
    readonly string FIRE = "FireRPC";
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
            this.ReStartCoroutine(DampingAnimationRoutine(value.x, dampingSpeed, AnimatorFloatValue.X), ref dampingCo[(int)AnimatorFloatValue.X]);
            this.ReStartCoroutine(DampingAnimationRoutine(value.y, dampingSpeed, AnimatorFloatValue.Z), ref dampingCo[(int)AnimatorFloatValue.Z]);
        }
    }
    public float VelocityY
    {
        set
        {
            //SetValue(AnimatorFloatValue.Y, value);
            this.ReStartCoroutine(DampingAnimationRoutine(value, dampingSpeed, AnimatorFloatValue.Y), ref dampingCo[(int)AnimatorFloatValue.Y]);
        }
    }
    #region
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
    #endregion
    public void Crouch(bool state)
    {
        if(state)
            photonView.RPC(CROUCH, RpcTarget.AllViaServer);
        else
            photonView.RPC(STAND, RpcTarget.AllViaServer);
    }

    public void JumpStart()
    {
        SetState(AnimatorState.JumpFinish, false);
        photonView.RPC(JUMP, RpcTarget.AllViaServer);
    }
    public void Fire()
    {
        photonView.RPC(FIRE, RpcTarget.AllViaServer);
    }
    public void ChangePistol()
    {
        if (anim.GetBool(weaponId[(int)AnimatorWeapon.Pistol]) == false)
            Equip(AnimatorWeapon.Pistol);
    }
    public void ChangeRifle()
    {
        if (anim.GetBool(weaponId[(int)AnimatorWeapon.Rifle]) == false)
            Equip(AnimatorWeapon.Rifle);
    }
    public void ChangeSword()
    {
        if (anim.GetBool(weaponId[(int)AnimatorWeapon.Sword]) == false)
            Equip(AnimatorWeapon.Sword);
    }
    public void Reload()
    {
        if (anim.GetBool(weaponId[(int)AnimatorWeapon.Rifle]) ||
            anim.GetBool(weaponId[(int)AnimatorWeapon.Pistol]))
            photonView.RPC(RELOAD, RpcTarget.AllViaServer);
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
    [PunRPC]
    void FireRPC()
    {
        anim.SetTrigger(FireId);
    }
    [PunRPC]
    void ReloadRPC()
    {
        anim.SetTrigger(ReloadId);
    }
    [PunRPC]
    void ChangeWeaponRPC()
    {
        anim.SetTrigger(ChangeWeaponId);
    }
    [PunRPC]
    void StandRPC()
    {
        anim.SetTrigger(StandId);
    }
    [PunRPC]
    void CrouchRPC()
    {
        anim.SetTrigger(CrouchId);
    }
    [PunRPC]
    void JumpRPC()
    {
        anim.SetTrigger(JumpEnterId);
    }
    void SetState(AnimatorState type, bool state)
    {
        anim.SetBool(layerId[(int)type], state);
    }
    private void Awake()
    {
        SetAnimID();
        dampingSpeed = dampingSpeed <= 0 ? 0.15f : dampingSpeed;
        dampingSpeed = 1 / dampingSpeed;
    }
    void Equip(AnimatorWeapon type)
    {
        OnWeapon(type);
        photonView.RPC(CHANGEWEAPON, RpcTarget.All);
    }
    void OnWeapon(AnimatorWeapon type)
    {
        for (int i = 0; i < weaponId.Length; i++)
        {
            bool state = i == (int)type ? true : false;
            anim.SetBool(weaponId[i], state);
        }
    }
    void SetAnimID()
    {
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;

        JumpEnterId = Animator.StringToHash("JumpEnter");
        StandId = Animator.StringToHash("Standing");
        CrouchId = Animator.StringToHash("Crouching");
        ChangeWeaponId = Animator.StringToHash("ChangeWeapon");
        ReloadId = Animator.StringToHash("Reload");
        FireId = Animator.StringToHash("Fire");
        
        int ForwardId = Animator.StringToHash("Forward");
        int TurnId = Animator.StringToHash("Turn");
        int JumpId = Animator.StringToHash("Jump");

        //int CrouchId = Animator.StringToHash("Crouch");
        int JumpFinishId = Animator.StringToHash("JumpFinish");
        int MoveId = Animator.StringToHash("Move");
        int RunId = Animator.StringToHash("Run");
        int rifleId = Animator.StringToHash("Rifle");
        int pistolId = Animator.StringToHash("Pistol");
        int swordId = Animator.StringToHash("Sword");

        floatId = new int[] { TurnId, JumpId, ForwardId };
        layerId = new int[] { MoveId, RunId,/* CrouchId,*/ JumpFinishId };
        weaponId = new int[] { pistolId, rifleId , swordId };
        dampingCo = new Coroutine[(int)AnimatorFloatValue.END];

        anim.SetBool(pistolId, true);
    }
    #region
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
    #endregion


    IEnumerator DampingAnimationRoutine(float value,float dampingValue, AnimatorFloatValue type)
    {
        float startValue = anim.GetFloat(floatId[(int)type]);
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
       // Crouch,
        JumpFinish,
        END
    }
    public enum AnimatorWeapon
    {
        Pistol, Rifle, Sword, END
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
