using Photon.Pun;
using System.Collections;
using UnityEngine;


public class AnimationController : MonoBehaviourPun
{
    Animator anim;
    public Animator Anim { get { return anim; } }
    [SerializeField] float dampingSpeed;

    int JumpEnterId;
    int StandId;
    int CrouchId;
    int ChangeWeaponId;
    int ReloadId;
    //int FireId;

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

    public Vector2 MoveValue
    {
        set
        {
            this.ReStartCoroutine(DampingAnimationRoutine(value.x, dampingSpeed, AnimatorFloatValue.X), 
                ref dampingCo[(int)AnimatorFloatValue.X]);
            this.ReStartCoroutine(DampingAnimationRoutine(value.y, dampingSpeed, AnimatorFloatValue.Z), 
                ref dampingCo[(int)AnimatorFloatValue.Z]);
        }
    }
    public float VelocityY
    {
        set
        {
            this.ReStartCoroutine(DampingAnimationRoutine(value, dampingSpeed, AnimatorFloatValue.Y), 
                ref dampingCo[(int)AnimatorFloatValue.Y]);
        }
    }
    public void Crouch(bool state)
    {
        string standType = state ? CROUCH : STAND;
        photonView.RPC(standType, RpcTarget.AllViaServer);
    }

    public void JumpStart()
    {
        SetState(AnimatorState.JumpFinish, false);
        photonView.RPC(JUMP, RpcTarget.AllViaServer);
    }
    //public void Fire()
    //{
    //    photonView.RPC(FIRE, RpcTarget.AllViaServer);
    //}
    public void ChangePistol()
    {
        ChangeWeapon(AnimatorWeapon.Pistol);
    }
    public void ChangeRifle()
    {
        ChangeWeapon(AnimatorWeapon.Rifle);
    }
    public void ChangeSword()
    {
        ChangeWeapon(AnimatorWeapon.Sword);
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
    //[PunRPC]
    //void FireRPC()
    //{
    //    anim.SetTrigger(FireId);
    //}
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
    void ChangeWeapon(AnimatorWeapon type)
    {
        if (anim.GetBool(weaponId[(int)type]) == false)
            Equip(type);
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
            bool state = i == (int)type;
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
        //FireId = Animator.StringToHash("Fire");

        int ForwardId = Animator.StringToHash("Forward");
        int TurnId = Animator.StringToHash("Turn");
        int JumpId = Animator.StringToHash("Jump");

        floatId = new int[] { TurnId, JumpId, ForwardId };

        int JumpFinishId = Animator.StringToHash("JumpFinish");
        int MoveId = Animator.StringToHash("Move");
        int RunId = Animator.StringToHash("Run");

        layerId = new int[] { MoveId, RunId, JumpFinishId };

        int rifleId = Animator.StringToHash("Rifle");
        int pistolId = Animator.StringToHash("Pistol");
        int swordId = Animator.StringToHash("Sword");

        weaponId = new int[] { pistolId, rifleId, swordId };

        dampingCo = new Coroutine[(int)AnimatorFloatValue.END];
        anim.SetBool(pistolId, true);
    }

    IEnumerator DampingAnimationRoutine(float value, float dampingValue, AnimatorFloatValue type)
    {
        float startValue = anim.GetFloat(floatId[(int)type]);
        float time = 0;
        float currentValue;
        while (time <= 1)
        {
            time += Time.deltaTime * dampingValue;
            currentValue = Mathf.Lerp(startValue, value, time);
            anim.SetFloat(floatId[(int)type], currentValue);
            yield return null;
        }
    }
    enum AnimatorState
    {
        Move,
        Run,
        JumpFinish,
        END
    }
    enum AnimatorWeapon
    {
        Pistol, Rifle, Sword, END
    }

    enum AnimatorFloatValue
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
