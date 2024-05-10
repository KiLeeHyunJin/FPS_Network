using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem.XInput;

public struct CharacterStat
{
    public int maxHp;
    public float moveSpeed;
    public float jumpPower;
}

public class CharacterTransformProcess
{
    CharacterController controller;
    GameObject foot;
    int groundLayer;
    float groundCheckLength;
    float limitAngle;
    float velocity;

    float currentSpeed;
    float walkSpeed;
    float runSpeed;
    float crouchSpeed;

    public float jumpHeight = 1f;
    public const float gravity = -9.81f;


    Vector3 moveDirection;
    Vector2 moveValue;

    bool IsGround;
    bool IsJumping;
    bool IsCrouch;
    bool IsMove;
    bool IsRun;
    Action[] motions;
    Action<Vector2> moveActionValue;
    Action<float> jumpActionValue;
    Action walkMotion;
    public CharacterTransformProcess(CharacterController characterController)
    {
        controller = characterController;
        motions = new Action[(int)AnimationController.MoveType.END];
        IsCrouch = false;
        IsJumping = false;
    }
    public void SetMotions(AnimationController.MoveType type, Action action)
    {
        motions[(int)type] = action;
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
        Debug.Log(currentSpeed);
    }


    public void SetMoveActionValue(Action<float> action)
    {
        jumpActionValue = action;
    }
    public void SetMoveActionValue(Action<Vector2> action)
    {
        moveActionValue = action;
    }
    public void Init(GameObject _foot,float _groundCheckLenth, int _groundLayer,float _limitAngle)
    {
        foot = _foot;
        groundCheckLength = _groundCheckLenth;
        groundLayer = _groundLayer;
        limitAngle = _limitAngle;
    }
    public void SetMoveSpeed(float _walkSpeed, float _runSpeed, float _crouchSpeed)
    {
        walkSpeed = _walkSpeed;
        runSpeed = _runSpeed;
        crouchSpeed = _crouchSpeed;
        currentSpeed = walkSpeed;
    }
    public void SetMoveValue(Vector2 _moveValue)
    {
        moveActionValue?.Invoke(_moveValue);
        if (_moveValue.x == 0 && _moveValue.y == 0)
        {
            if(IsJumping == false)
                motions[(int)AnimationController.MoveType.Stop]?.Invoke();
            IsMove = false;
            moveValue = Vector2.zero;
            moveDirection = Vector3.zero;
            return;
        }
        IsMove = true;
        moveValue = _moveValue;
    }

    public void Update()
    {
        Gravity();
        Direction();
        controller.Move(moveDirection);
    }
    public void FixedUpdate()
    {
        GroundCheck();
    }

    void Direction()
    {
        moveDirection = new Vector3(0, velocity, 0);
        if (IsMove == false)
            return;

        walkMotion?.Invoke();
        Vector2 value = new Vector2(moveValue.x, moveValue.y) * currentSpeed * Time.deltaTime; ;
        moveDirection += ((value.x * controller.transform.right) + (value.y * controller.transform.forward));
    }

    void GroundCheck()
    {
        //하단에 레이 충돌 검사
        IsGround = Physics.Raycast(
            foot.transform.position + Vector3.up * 0.1f, Vector3.down,
            out RaycastHit hitInfo,
            groundCheckLength,
            groundLayer);

        if (IsGround)
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            //Debug.DrawLine(foot.transform.position, foot.transform.position + Vector3.up, Color.red);
            //Debug.DrawLine(foot.transform.position, foot.transform.position + hitInfo.normal, Color.blue);
            if (IsJumping) //떨어지고 있는 상태라면
            {
                if (angle < limitAngle) //미끄러지는 각도인지 확인
                {
                    motions[(int)AnimationController.MoveType.JumpFinish]?.Invoke(); //미끄러지는 각도가 아니라면 착지
                    IsJumping = false;
                    Debug.Log("Jump slide Angle");
                }
            }
            else if (angle > limitAngle) //미끄러지는 각도라면
            {
                motions[(int)AnimationController.MoveType.JumpSlide]?.Invoke();
                IsJumping = true;
                Debug.Log("Slide Angle");
            }
        }
        else //땅이 아니라면
        {
            if (IsJumping == false) //점프상태가 아니고 
            {
                if (IsCrouch) //앉은 상태라면
                    Crouch(); //일어서게 하고
                motions[(int)AnimationController.MoveType.Jump]?.Invoke();
                IsJumping = true;
                Debug.Log("Downing");
            }
        }
    }
    public void Crouch()
    {
        IsCrouch = !IsCrouch;
        if (IsCrouch)
        {
            motions[(int)AnimationController.MoveType.Crouch]?.Invoke();
        }
        else
        {
            motions[(int)AnimationController.MoveType.Stand]?.Invoke();
        }
        SetMoveType(IsRun);
    }

    public void Jump()
    {
        if (IsGround && IsCrouch == false)
        {
            velocity = Mathf.Sqrt(-1f * gravity);
            motions[(int)AnimationController.MoveType.Jump]?.Invoke();
            IsJumping = true;
        }
    }
    float jumpTime;
    void Gravity()
    {
        jumpTime += Time.deltaTime;
        if (IsJumping)
        {
            SlerpGravity();
        }
        else
        {
            if(jumpTime != 0)
                jumpTime = 0;
            if (velocity < 0)
                velocity = -2f; // Grounded, so reset velocity
        }
        jumpActionValue?.Invoke(velocity);
        moveDirection.y = velocity;
    }

    void SlerpGravity()
    {
        if (velocity > -2)
        {
            if (velocity > 0)
            {
                velocity += gravity * jumpTime;
            }
            else
            {
                velocity += gravity * Time.deltaTime;
            }
        }
    }
}
