using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EditUserDataPanel : MonoBehaviourShowInfo
{
    [SerializeField] TMP_InputField passInputField;
    [SerializeField] TMP_InputField confirmInputField;
    [SerializeField] TMP_InputField nickNameInputField;

    [SerializeField] TMP_Text NameText;


    [SerializeField] Button passApplyButton;  //비밀번호 재설정 버튼
    [SerializeField] Button nickApplyButton;
    [SerializeField] Button cancleButton; //창닫기 버튼

    private void Awake()
    {
        nickApplyButton.onClick.AddListener(NickApply);
        passApplyButton.onClick.AddListener(PassApply);
        cancleButton.onClick.AddListener(Cancel);
        nickNameInputField.characterLimit = 10;
    }
    void Cancel()
    {
        gameObject.SetActive(false);
    }
    private void NickApply()
    {
        SetInteractable(false);

        UserProfile userProfile = new UserProfile();
        userProfile.DisplayName = nickNameInputField.text;
        FireBaseManager.Auth.CurrentUser.UpdateUserProfileAsync(userProfile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("닉네임 변경이 취소되었습니다.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "닉네임변경이 실패하였습니다.");
                SetInteractable(true);
                return;
            }
            ShowInfo("닉네임 변경이 완료되었습니다.");
            SetInteractable(true);
            NameText.text = userProfile.DisplayName;
            PhotonNetwork.LocalPlayer.NickName = userProfile.DisplayName;
        });
    }
    private void PassApply()
    {
        SetInteractable(false);
        if (passInputField.text != confirmInputField.text)
        {
            ShowInfo("비밀번호가 일치하지 않습니다.");
            SetInteractable(true);
        }
        string newPassword = passInputField.text;
        FireBaseManager.Auth.CurrentUser.UpdatePasswordAsync(newPassword).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("비밀번호 재설정이 취소되었습니다.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "비밀번호 변경이 실패하였습니다.");
                SetInteractable(true);
                return;
            }
            ShowInfo("비밀번호 변경이 완료되었습니다.");
            SetInteractable(true);
        });
    }
    private void SetInteractable(bool interactable)
    {
        passInputField.interactable = interactable;
        confirmInputField.interactable = interactable;
        passApplyButton.interactable = interactable;
    }

}
