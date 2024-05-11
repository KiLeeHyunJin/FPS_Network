using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UIElements;

public struct CharacterStat
{
    public int maxHp;
    public float moveSpeed;
    public float jumpPower;
}
[Serializable]
public class CharacterTransformProcess
{
    CharacterController controller;
    Transform foot;

    Action[] motions;
    Action<Vector2> moveActionValue;
    Action<float> jumpActionValue;
    Action walkMotion;

    int groundLayer;

    float groundCheckLength;
    float limitAngle;
    float velocity;

    float currentSpeed;
    float walkSpeed;
    float runSpeed;
    float crouchSpeed;
    float jumpHeight;
    float checkRadius;
    const float gravity = -9.81f;

    Vector3 moveDirection;
    Vector2 moveValue;
    [SerializeField] Vector2 slideValue;

    bool IsGround;
    bool IsJumping;
    bool IsCrouch;
    bool IsMove;
    bool IsRun;
    bool IsIgnoreSpace;
    public void Init(CharacterController characterController)
    {
        controller = characterController;
        motions = new Action[(int)AnimationController.MoveType.END];
    }

    public void InitGroundCheckData(Transform _foot, float _groundCheckLenth, int _groundLayer, float _jumpHeight)
    {
        foot = _foot;
        groundCheckLength = _groundCheckLenth;
        groundLayer = _groundLayer;
        jumpHeight = _jumpHeight;
    }
    public void Start()
    {
        IsCrouch = false;
        IsJumping = false;
        walkMotion = motions[(int)AnimationController.MoveType.Walk];
        currentSpeed = walkSpeed;
        limitAngle = controller.slopeLimit;
        checkRadius = controller.radius * 0.9f;
    }
    public void SetMoveType(bool state)
    {
        IsRun = state;
        walkMotion = state ? motions[(int)AnimationController.MoveType.Run] : motions[(int)AnimationController.MoveType.Walk];
        if (IsCrouch)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = state ? runSpeed : walkSpeed;
        }
    }

    public void SetMoveSpeed(float _walkSpeed, float _runSpeed, float _crouchSpeed)
    {
        walkSpeed = _walkSpeed;
        runSpeed = _runSpeed;
        crouchSpeed = _crouchSpeed;
    }
    public void SetMotions(AnimationController.MoveType type, Action action)
    {
        motions[(int)type] = action;
    }
    public void SetMoveActionValue(Action<float> action)
    {
        jumpActionValue = action;
    }
    public void SetMoveActionValue(Action<Vector2> action)
    {
        moveActionValue = action;
    }

    public void SetMoveValue(Vector2 _moveValue)
    {
        moveValue = _moveValue;
        moveActionValue?.Invoke(moveValue);
        if (moveValue.x == 0 && moveValue.y == 0)
        {
            motions[(int)AnimationController.MoveType.Stop]?.Invoke();
            IsMove = false;
            return;
        }
        if(IsMove == false)
            IsMove = true;
    }
    public void Crouch()
    {
        SetMoveType(IsRun);
        IsCrouch = !IsCrouch;
        if (IsCrouch)
            motions[(int)AnimationController.MoveType.Crouch]?.Invoke();
        else
            motions[(int)AnimationController.MoveType.Stand]?.Invoke();
    }
    public void Jump()
    {
        if (IsIgnoreSpace && IsJumping == false)
        {
            velocity = Mathf.Sqrt(2f * (-gravity) * jumpHeight); //자유낙하 공식
            JumpMotion(true);
        }
    }
    public void Update()
    {
        if (Physics.SphereCast(foot.position + new Vector3(0,0.1f,0), controller.radius + 2, Vector3.down, out RaycastHit hitInfo, 0, groundLayer))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
        }
        else
            Debug.Log("Non");
        Gravity();
        Direction();
        controller.Move(moveDirection);
    }
    public void FixedUpdate()
    {
        GroundCheck();
    }

    void GroundCheck()
    {
        IsGround = Physics.CheckSphere(
            foot.position, checkRadius,
            groundLayer);

        (Vector3 startPos, float checkLength) = IsGround ?
            (foot.position + Vector3.up , groundCheckLength) : 
            (foot.position              , groundCheckLength - 0.7f);

        IsIgnoreSpace = Physics.Raycast(
            startPos, Vector3.down,
            out RaycastHit hitInfo,
            checkLength,
            groundLayer);
        bool check = IsGround;
        if (velocity < 0)
            check |= IsIgnoreSpace;

        SlideCheck(hitInfo, check);
    }
    void SlideCheck(RaycastHit hitInfo,bool ignoreCheck)
    {
        if(hitInfo.collider == null)
        {
            SlopeCheck();
            return;
        }
        slideValue = Vector2.zero;

        if (ignoreCheck) //높이 상태를 무시해도 되거나 땅이라면
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            if (angle < limitAngle) //미끄러지는 각도가 아니라면
            {
                if (IsJumping)
                {
                    JumpMotion(false);
                    Debug.Log(1);
                }
                return;
            }
            else
            {
                SlopeCheck();
                JumpMotion(true);
            }
        }
        else
        {
            JumpMotion(true);
            Debug.Log(2);
        }
    }
    void SlopeCheck()
    {
        if (Physics.SphereCast(foot.position , 3, Vector3.zero, out RaycastHit hitInfo,0,groundLayer))
        {
            //Vector3 normal = Vector3.Cross(hitInfo.normal, Vector3.up);
            //Debug.DrawLine(foot.position, foot.position + normal, Color.cyan);
            //Debug.Log(hitInfo.normal);
            //// 경사면의 방향을 반환합니다.
            //Vector3 slopeDirection = Vector3.Cross(Vector3.down, normal);
            //Debug.DrawLine(foot.position, foot.position + hitInfo.normal, Color.green);
            //Debug.Log(hitInfo.collider.gameObject.name);
            // 미끄러져야 하는 방향을 반환합니다.
            //slideValue = new Vector2(hitInfo.normal.x, hitInfo.normal.z);
            return;
        }
       // Debug.Log(-1);
    }
    void JumpMotion(bool state)
    {
        if(IsJumping != state) 
        {
            IsJumping = state;
            if(IsJumping)
            {
                if (IsCrouch) //앉은 상태라면
                {
                    Crouch(); //일어서게 하고
                }
                motions[(int)AnimationController.MoveType.Jump]?.Invoke();
            }
            else
            {
                motions[(int)AnimationController.MoveType.JumpFinish]?.Invoke();
            }
        }
    }

    void Direction()
    {
        moveDirection = new Vector3(slideValue.x, velocity, slideValue.y);
        if (IsMove)
        {
            if(IsJumping == false)
                walkMotion?.Invoke();
            Vector2 value = currentSpeed * moveValue; ;
            moveDirection += ((value.x * controller.transform.right) + (value.y * controller.transform.forward));
        }
        moveDirection *= Time.deltaTime;
    }

    void Gravity()
    {
        if (IsGround && IsJumping == false)
        {
            if(velocity == 0)
                return;
            velocity = 0f;
        }
        else
        {
            velocity += gravity * Time.deltaTime;
        }

        jumpActionValue?.Invoke(velocity);
    }
}
