using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;


public class AnimationController : MonoBehaviourPun
{
    IKAnimationController iKAnimation;
    Animator anim;
    public Animator Anim { get { return anim; } }
    Coroutine[] dampingCo;
    [SerializeField] float dampingSpeed;

    [SerializeField] Rig handRig;

    [SerializeField] MultiParentConstraint primaryParent;
    [SerializeField] MultiParentConstraint subParent;
    [SerializeField] MultiParentConstraint knifeParent;
    [SerializeField] MultiParentConstraint throwParent;

    [SerializeField] IKWeapon rifleWeapon;
    [SerializeField] IKWeapon pistolWeapon;
    [SerializeField] IKWeapon swordWeapon;
    [SerializeField] IKWeapon throwWeapon;

    [SerializeField] Transform[] currentWeapons;
    [SerializeField] Transform[] saveWeapons;

    [SerializeField] TwoBoneIKConstraint left;
    [SerializeField] TwoBoneIKConstraint right;
    int JumpEnterId;
    int StandId;
    int CrouchId;

    int ChangeWeaponId;
    int ReloadId;
    int AtckId;

    int[] layerId;
    int[] floatId;
    int[] weaponId;

    readonly string TRIGGER = "CallTriggerRPC";
    private void Start()
    {
        iKAnimation = new IKAnimationController
            (handRig, GetComponent<RigBuilder>(), left, right, 
            primaryParent, subParent, knifeParent, throwParent, 
            currentWeapons, saveWeapons, GetComponent<Controller>());
        
        //iKAnimation.ChangeWeapon(rifleWeapon);
        //iKAnimation.EquipWeapon();
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

    public void ChangeWeapon(AnimatorWeapon type)
    {
        if (anim.GetBool(weaponId[(int)type]) == false)
        {
            OnWeapon(type);
            photonView.RPC(TRIGGER, RpcTarget.All, ChangeWeaponId);
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
            bool state = anim.GetBool(weaponId[i]);
            IKWeapon weapon = null;
            if (i == 0)
            {
                weapon = pistolWeapon;
            }
            else if (i == 1)
            {
                weapon = rifleWeapon;
            }
            else if (i == 2)
            {
                weapon = swordWeapon;
            }
            else
                weapon = throwWeapon;
            weapon?.gameObject.SetActive(state);
            if (state)
                iKAnimation.ChangeWeapon(weapon);
        }
    }

    [PunRPC]
    void CallTriggerRPC(int triggerId)
    {
        anim.SetTrigger(triggerId);
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
