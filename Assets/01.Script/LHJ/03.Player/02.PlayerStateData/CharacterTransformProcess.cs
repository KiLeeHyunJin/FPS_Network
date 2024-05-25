using System;
using System.Collections;
using UnityEngine;
public class CharacterTransformProcess
{
    CharacterController controller;
    AudioController audio;
    Transform foot;

    Action[] motions;
    Action<Vector2> moveActionValue;
    Action<float> jumpActionValue;
    Action walkMotion;

    int groundLayer;

    float groundCheckLength;
    float ignoreGroundCheckLenth;
    float limitAngle;
    float velocity;

    float currentSpeed;
    float walkStandSpeed;
    float runStandSpeed;
    float walkCrouchSpeed;
    float runCrouchSpeed;

    float jumpHeight;
    float checkRadius;
    float gravity;

    const float defaultGravity = -9.81f;

    Vector3 moveDirection;
    Vector2 moveValue;
    Vector2 slideValue;

    bool IsGround;
    bool IsJumping;
    bool IsCrouch;
    bool IsMove;
    bool IsRun;
    bool IsIgnoreSpace;

    public void Init(CharacterController characterController)
    {
        controller = characterController;
        limitAngle = controller.slopeLimit;
        checkRadius = controller.radius * 0.9f;
        motions = new Action[(int)AnimationController.MoveType.END];
        audio = characterController.gameObject.GetComponent<AudioController>();
    }

    public void InitGroundCheckData(Transform _foot, float _groundCheckLenth, float _ignoreGroundCheckLenth, int _groundLayer, float _jumpHeight, float _gravitySpeed)
    {
        foot = _foot;
        groundCheckLength = _groundCheckLenth;
        ignoreGroundCheckLenth = _ignoreGroundCheckLenth;
        groundLayer = _groundLayer;
        jumpHeight = _jumpHeight;
        gravity = _gravitySpeed * defaultGravity;
    }
    public void Start()
    {
        IsCrouch = false;
        IsJumping = false;
        walkMotion = motions[(int)AnimationController.MoveType.Walk];
        motions[(int)AnimationController.MoveType.Stand]?.Invoke();
    }
    public void SetMoveType(bool state)
    {
        IsRun = state;
        if(state)
        {
            audio.PlayMoveSound(AudioController.ClipType.Run);
            walkMotion = motions[(int)AnimationController.MoveType.Run];
        }
        else
        {
            audio.PlayMoveSound(AudioController.ClipType.Walk);
            walkMotion = motions[(int)AnimationController.MoveType.Walk];
        }
        if (IsCrouch)
        {
            currentSpeed = state ? runCrouchSpeed : walkCrouchSpeed;
        }
        else
        {
            currentSpeed = state ? runStandSpeed : walkStandSpeed;
        }
    }

    public void SetMoveSpeed(float _walkStandSpeed, float _runStandSpeed, float _walkCrouchSpeed, float _runCrouchSpeed)
    {
        walkStandSpeed = _walkStandSpeed;
        runStandSpeed = _runStandSpeed;
        walkCrouchSpeed = _walkCrouchSpeed;
        runCrouchSpeed = _runCrouchSpeed;
        currentSpeed = walkStandSpeed;
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
            audio.PlayMoveSound(AudioController.ClipType.Stop);
            return;
        }
        if (IsMove == false)
            IsMove = true;
        audio.PlayMoveSound(AudioController.ClipType.Walk);
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
        if (IsIgnoreSpace && IsJumping == false)
        {
            velocity = Mathf.Sqrt(2f * (-gravity) * jumpHeight); //자유낙하 공식
            JumpMotion(true);
        }
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

    void GroundCheck()
    {
        IsGround = Physics.CheckSphere(
            foot.position, checkRadius,
            groundLayer);

        (Vector3 startPos, float checkLength) = IsGround ?
            (foot.position + Vector3.up, groundCheckLength) :
            (foot.position, ignoreGroundCheckLenth);

        IsIgnoreSpace = Physics.Raycast(
            startPos, Vector3.down,
            out RaycastHit hitInfo,
            checkLength,
            groundLayer);


        bool check = IsGround;
        if (velocity <= 0)
            check |= IsIgnoreSpace;

        SlideCheck(hitInfo, check);
    }
    void SlideCheck(RaycastHit hitInfo, bool ignoreCheck)
    {
        //if(hitInfo.collider == null)
        //{
        //    SlopeCheck();
        //    return;
        //}
        //slideValue = Vector2.zero;

        if (ignoreCheck) //높이 상태를 무시해도 되거나 땅이라면
        {
            //Slide(hitInfo);
            if (velocity < 0)
            {
                JumpMotion(false);
            }
        }
        else
        {
            JumpMotion(true);
        }
    }
    void Slide(RaycastHit hitInfo)
    {
        float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
        if (angle < limitAngle) //미끄러지는 각도가 아니라면
        {
            if (IsJumping)
            {
                JumpMotion(false);
            }
            return;
        }
        else
        {
            SlopeCheck();
            JumpMotion(true);
        }
    }
    void SlopeCheck()
    {
        if (Physics.SphereCast(foot.position, 3, Vector3.zero, out RaycastHit hitInfo, 0, groundLayer))
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
        if (IsJumping != state)
        {
            IsJumping = state;
            if (IsJumping)
            {
                if (IsCrouch) //앉은 상태라면
                {
                    Crouch(); //일어서게 하고
                }
                motions[(int)AnimationController.MoveType.Jump]?.Invoke();
                audio.PlayMoveSound(AudioController.ClipType.Jump);
            }
            else
            {
                motions[(int)AnimationController.MoveType.JumpFinish]?.Invoke();
                audio.PlayMoveSound(AudioController.ClipType.Land);
                if(IsMove)
                {
                    if(IsRun)
                        audio.PlayMoveSound(AudioController.ClipType.Run);
                    else
                        audio.PlayMoveSound(AudioController.ClipType.Walk);
                }
            }
        }
        else if (state)
        {
            motions[(int)AnimationController.MoveType.JumpSlide]?.Invoke();
        }
    }

    void Direction()
    {
        moveDirection = new Vector3(slideValue.x, velocity, slideValue.y);
        if (IsMove)
        {
            if (IsJumping == false)
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
            if (velocity == 0)
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
