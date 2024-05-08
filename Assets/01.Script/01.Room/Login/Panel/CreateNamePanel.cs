using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class CreateNamePanel : MonoBehaviourShowInfo
{
    [SerializeField] LoginPanelController panelController;
    [SerializeField] TMP_InputField nameInputField;

    [SerializeField] Button nameApplyButton;
    [SerializeField] Button backButton;

    private void Awake()
    {
        nameApplyButton.onClick.AddListener(NameApply); //닉네임 설정 버튼에 닉네임 변경 시도 함수를 연결
        backButton.onClick.AddListener(Back); //나가기 버튼에 나가기 함수를 연결
    }
    private void NameApply()
    {
        SetInteractable(false);
        UserProfile profile = new UserProfile();
        string nickName = nameInputField.text;
        profile.DisplayName = nickName;
        profile.PhotoUrl = FireBaseManager.Auth.CurrentUser.PhotoUrl;


        FireBaseManager.DB
            .GetReference("NickNames")
            .Child("name")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if(task.IsCanceled)
                {
                    return;
                }
                else if(task.IsFaulted)
                {
                    return;
                }
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                Debug.Log(json);
            });



        FireBaseManager.Auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task => 
        {
            if(task.IsCanceled)
            {
                ShowInfo("닉네임 설정이 취소되었습니다.");
                SetInteractable(true);
                return;
            }
            if(task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "닉네임 설정이 실패하였습니다.");
                SetInteractable(true);
                return;
            }
            FireBaseManager.DB
            .GetReference("NickNames")
            .Child("name")
            .SetRawJsonValueAsync(JsonUtility.ToJson(nickName));

            ShowInfo("닉네임 설정이 성공되었습니다.");
            SetInteractable(true);
        });
    }


    private void Back()
    {
        if(FireBaseManager.Auth.CurrentUser.DisplayName.IsNullOrEmpty())
            ShowInfo("닉네임을 입력해주세요.");
        else
        {
            gameObject.SetActive(false);
            Photon.Pun.PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Delete()
    {
        SetInteractable(false);
        FireBaseManager.Auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("계정 삭제가 취소되었습니다.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "계정 삭제가 실패하였습니다..");
                SetInteractable(true);
                return;
            }ShowInfo("계정 삭제가 완료되었습니다.");
            SetInteractable(true);
            FireBaseManager.Auth.SignOut();
            panelController.SetActivePanel(LoginPanelController.Panel.Login);
        });
    }

    private void SetInteractable(bool interactable)
    {
        nameInputField.interactable = interactable;
        nameApplyButton.interactable = interactable;
        backButton.interactable = interactable;
    }

}
