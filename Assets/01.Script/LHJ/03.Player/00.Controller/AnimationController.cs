using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions.Must;
using UnityEngine.SocialPlatforms.Impl;
using static Define;
using static SoundManager;


public class AnimationController : MonoBehaviourPun
{
    IKAnimationController iKAnimation;
    Animator anim;
    public Animator Anim { get { return anim; } }
    Coroutine[] dampingCo;
    [SerializeField] float dampingSpeed;

    [SerializeField] Rig handRig;

    [SerializeField] TwoBoneIKConstraint left;
    [SerializeField] TwoBoneIKConstraint right;
    [SerializeField] MultiParentConstraint[] weaponParents;
    Transform[] currentWeapons;
    InventoryController inventoryController;
    int JumpEnterId;
    int StandId;
    int CrouchId;

    int ChangeWeaponId;
    int ReloadId;
    int AtckId;
    int DieId;
    int LiveId;

    int[] layerId;
    int[] floatId;
    int[] weaponId;

    readonly string TRIGGER = "CallTriggerRPC";
    readonly string RIGIK = "RigIK";

    private void Awake()
    {
        SetAnimID();
        dampingSpeed = dampingSpeed <= 0 ? 0.15f : dampingSpeed;
        dampingSpeed = 1 / dampingSpeed;
        int length = weaponParents.Length;
        currentWeapons = new Transform[length];
        for (int i = 0; i < length; i++)
            currentWeapons[i] = weaponParents[i].data.constrainedObject;
    }

    private void Start()
    {
        iKAnimation = new IKAnimationController
            (handRig, GetComponent<RigBuilder>(), left, right,
            weaponParents,
            GetComponent<Controller>());
        inventoryController = GetComponent<InventoryController>();
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

    public bool ChangeWeapon(AnimatorWeapon type, ref Iattackable atckable )
    {
        if (currentWeapons[(int)type].childCount == 0)
        {
            return false;
        }

        if (anim.GetBool(weaponId[(int)type]))
        {
            return false;
        }

        photonView.RPC(RIGIK, RpcTarget.Others,
            currentWeapons[(int)type].GetChild(0).GetComponent<IKWeapon>().weaponType,
            currentWeapons[(int)type].GetChild(0).GetComponent<IKWeapon>().InstanceId, 
            ChangeWeaponId);

        OnWeaponLayer(type);
        anim.SetTrigger(ChangeWeaponId);
        atckable = currentWeapons[(int)type].GetComponent<Iattackable>();
        return true;
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
        photonView.RPC(TRIGGER, RpcTarget.AllViaServer, DieId);
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

    public void Alive()
    {
        photonView.RPC(TRIGGER, RpcTarget.AllViaServer, LiveId);
    }

    public void DequipWeapon()
    {
        iKAnimation?.DequipWeapon();

        for (int i = 0; i < weaponId.Length; i++)
        {
            if(anim.GetBool(weaponId[i]))
            {
                AnimatorWeapon weapon = (AnimatorWeapon)i;
                currentWeapons[i].gameObject.SetActive(true);
                iKAnimation.ChangeWeapon(weapon);
                inventoryController?.ChangeWeaponUpdate(weapon);
                return;
            }
        }
    }

    public IKWeapon ChangedWeapon(AnimatorWeapon weapon)
    {
        return inventoryController[weapon];
    }
    [PunRPC]
    void CallTriggerRPC(int triggerId)
    {
        anim.SetTrigger(triggerId);
    }

    [PunRPC]
    void RigIK(int type,int _instanceId, int triggerId)
    {
        inventoryController.AddItem((AnimatorWeapon)type, _instanceId);
        anim.SetTrigger(triggerId);
        iKAnimation.ChangeWeapon((AnimatorWeapon)type);
    }


    void SetState(AnimatorState type, bool state)
    {
        anim.SetBool(layerId[(int)type], state);
    }
   


    void OnWeaponLayer(AnimatorWeapon type)
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
        DieId = Animator.StringToHash("Die");
        LiveId = Animator.StringToHash("Reset");

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
        //anim.SetBool(pistolId, true);
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
