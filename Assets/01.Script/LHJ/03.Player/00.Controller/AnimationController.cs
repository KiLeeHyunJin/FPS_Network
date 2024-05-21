using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using static Define;


public class AnimationController : MonoBehaviourPun
{
    IKAnimationController iKAnimation;
    Animator anim;
    public Animator Anim { get { return anim; } }
    Coroutine[] dampingCo;
    [SerializeField] float dampingSpeed;

    [SerializeField] Rig handRig;


    [SerializeField] IKWeapon rifleWeapon;
    [SerializeField] IKWeapon pistolWeapon;
    [SerializeField] IKWeapon swordWeapon;
    [SerializeField] IKWeapon throwWeapon;

    [SerializeField] Transform[] currentWeapons;
    [SerializeField] Transform[] saveWeapons;

    [SerializeField] TwoBoneIKConstraint left;
    [SerializeField] TwoBoneIKConstraint right;
    [SerializeField] MultiParentConstraint[] weaponParents;

    int JumpEnterId;
    int StandId;
    int CrouchId;

    int ChangeWeaponId;
    int ReloadId;
    int AtckId;

    int[] layerId;
    int[] floatId;
    int[] weaponId;
    //IKWeapon[] weapons;

    readonly string TRIGGER = "CallTriggerRPC";
    readonly string RIGIK = "RigIK";
    private void Start()
    {
        iKAnimation = new IKAnimationController
            (handRig, GetComponent<RigBuilder>(), left, right,
            weaponParents, 
            currentWeapons, saveWeapons, GetComponent<Controller>());

       // weapons = new IKWeapon[] { pistolWeapon, rifleWeapon, swordWeapon, throwWeapon };
    }
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
        int standType = state ? CrouchId : StandId;
        photonView.RPC(TRIGGER, RpcTarget.AllViaServer, standType);
    }

    public void JumpStart()
    {
        SetState(AnimatorState.JumpFinish, false);
        photonView.RPC(TRIGGER, RpcTarget.AllViaServer, JumpEnterId);
    }

    public void Atck()
    {
        if (anim.GetBool(weaponId[(int)AnimatorWeapon.Sword]) ||
            anim.GetBool(weaponId[(int)AnimatorWeapon.Throw]) )
            photonView.RPC(TRIGGER, RpcTarget.AllViaServer, AtckId);
    }
    public void AddnewWeapon(IKWeapon newWeapon)
    {
        if (newWeapon == null || newWeapon.weaponType == AnimatorWeapon.END)
            return;

        switch (newWeapon.weaponType)
        {
            case AnimatorWeapon.Pistol:
                pistolWeapon = newWeapon;
                break;
            case AnimatorWeapon.Rifle:
                rifleWeapon = newWeapon;
                break;
            case AnimatorWeapon.Sword:
                swordWeapon = newWeapon;
                break;
            case AnimatorWeapon.Throw:
                throwWeapon = newWeapon;
                break;
            case AnimatorWeapon.END:
                break;
        }
    }
    public void ChangeWeapon(AnimatorWeapon type)
    {
        IKWeapon changeWeapon = GetIKWeapon(type);
        if (changeWeapon == null)
            return;
        
        if (anim.GetBool(weaponId[(int)type]) == false)
        {
            photonView.RPC(RIGIK, RpcTarget.Others, changeWeapon.name, ChangeWeaponId);

            OnWeapon(type);
            anim.SetTrigger(ChangeWeaponId);
        }
    }

    public void Reload()
    {
        if (anim.GetBool(weaponId[(int)AnimatorWeapon.Rifle]) ||
            anim.GetBool(weaponId[(int)AnimatorWeapon.Pistol]))
            photonView.RPC(TRIGGER, RpcTarget.AllViaServer, ReloadId);
    }

    public void Die()
    {
        int upper = anim.GetLayerIndex("Upper");
        anim.SetLayerWeight(upper, 0);
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

    public void TransitionWeapon()
    {
        iKAnimation?.HandOn();
    }
    public void AimWeapon()
    {
        iKAnimation?.AimOn();
    }

    public void EquipWeapon()
    {
        iKAnimation?.EquipWeapon();
    }

    public void DequipWeapon()
    {
        iKAnimation?.DequipWeapon();

        for (int i = 0; i < weaponId.Length; i++)
        {
            if(anim.GetBool(weaponId[i]))
            {
                IKWeapon weapon;
                switch (i)
                {
                    case 0:
                        weapon = pistolWeapon;
                        break;
                    case 1:
                        weapon = rifleWeapon;
                        break;
                    case 2:
                        weapon = swordWeapon;
                        break;
                    default:
                        weapon = throwWeapon;
                        break;
                }
                if (weapon != null)
                {
                    weapon.gameObject.SetActive(true);
                    iKAnimation.ChangeWeapon(weapon);
                }
                return;
            }
        }
    }
    IKWeapon GetIKWeapon(AnimatorWeapon type)
    {
        switch (type)
        {
            case AnimatorWeapon.Pistol:
                return pistolWeapon;
            case AnimatorWeapon.Rifle:
                return rifleWeapon;
            case AnimatorWeapon.Sword:
                return swordWeapon;
            case AnimatorWeapon.Throw:
                return throwWeapon;
        }
        return null;
    }

    [PunRPC]
    void CallTriggerRPC(int triggerId)
    {
        anim.SetTrigger(triggerId);
    }

    [PunRPC]
    void RigIK(string rigWeaponStr, int triggerId)
    {
        IKWeapon newWeapon = Manager.Resource.Load<IKWeapon>(ResourceManager.ResourceType.Weapon,rigWeaponStr);
        anim.SetTrigger(triggerId);
        iKAnimation.ChangeWeapon(newWeapon);
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
        AtckId = Animator.StringToHash("Atck");

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
        int throwId = Animator.StringToHash("Throw");

        weaponId = new int[] { pistolId, rifleId, swordId, throwId };

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
    public enum AnimatorWeapon
    {
        Pistol, Rifle, Sword, Throw ,END
    }

    enum AnimatorState
    {
        Move,
        Run,
        JumpFinish,
        END
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
