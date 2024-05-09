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
    float limitAngle;
    int groundLayer;
    float groundCheckLength;
    Vector3 moveValue;

    const float Half = 0.5f;
    public bool isGround { get; private set; }
    public bool isStop { get; private set; }
    public bool isJumping { get; private set; }
    public bool isRun { get; private set; }
    public bool isCrouch { get; private set; }
    public bool SetCrouch { set { isCrouch = value; } }

    Action JumpFinish;
    Action Jump;
    Action Crouch;
    Action SlideJump;


    public CharacterTransformProcess(CharacterController characterController)
    {
        controller = characterController;
    }
    public void SetActions(Action _jumpFinish, Action _jump, Action _crouch, Action _slideJump)
    {
        JumpFinish = _jumpFinish;
        Jump = _jump;
        Crouch = _crouch;
        SlideJump = _slideJump;
    }
    public void Init(GameObject _foot,float _groundCheckLenth, int _groundLayer,float _limitAngle)
    {
        foot = _foot;
        groundCheckLength = _groundCheckLenth;
        groundLayer = _groundLayer;
        limitAngle = _limitAngle;
    }
    public void Move(Vector3 _moveValue)
    {
        moveValue = _moveValue;
    }

    public void Update()
    {
        controller.Move(moveValue * Time.deltaTime);    
    }
    public void FixedUpdate()
    {
        GroundCheck();
    }
    void Crouching()
    {

    }
    void GroundCheck()
    {
        //하단에 레이 충돌 검사
        isGround = Physics.Raycast(
            foot.transform.position + Vector3.up * 0.1f, Vector3.down,
            out RaycastHit hitInfo,
            groundCheckLength,
            groundLayer);

        if (isGround)
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

            //Debug.DrawLine(foot.transform.position, foot.transform.position + Vector3.up, Color.red);
            //Debug.DrawLine(foot.transform.position, foot.transform.position + hitInfo.normal, Color.blue);
            if (isJumping) //떨어지고 있는 상태라면
            {
                if (angle < limitAngle) //미끄러지는 각도인지 확인
                {
                    JumpFinish(); //미끄러지는 각도가 아니라면 착지
                    isJumping = false;
                }
                else //점프 모션 발동
                {
                    SlideJump?.Invoke();
                }
            }
            else if (angle > limitAngle) //미끄러지는 각도라면
            {
                Jump?.Invoke();
                isJumping = true;
            }
        }
        else //땅이 아니라면
        {
            if (isJumping == false) //점프상태가 아니고 
            {
                if (isCrouch) //앉은 상태라면
                    Crouch(); //일어서게 하고
                Jump?.Invoke(); //점프 실행
                isJumping = true;
            }
        }
    }

}
