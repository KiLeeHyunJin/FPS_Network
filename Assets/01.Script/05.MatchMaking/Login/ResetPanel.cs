using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResetPanel : MonoBehaviourShowInfo
{
    [SerializeField] LoginPanelController panelController;
    [SerializeField] TMP_InputField emailInputField;

    [SerializeField] Button sendButton;
    [SerializeField] Button cancelButton;

    private void Awake()
    {
        sendButton.onClick.AddListener(SendResetMail);
        cancelButton.onClick.AddListener(Cancel);
    }
    private void SendResetMail()
    {
        SetInteractable(false);
        string email = emailInputField.text;
        FireBaseManager.Auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("비밀번호 초기화 메일 전송이 취소되었습니다.");
                SetInteractable(true);
            }
            else if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "인증 메일 전송에 실패하였습니다.");
                SetInteractable(true);
            }

            panelController.SetActivePanel(LoginPanelController.Panel.Login);
            ShowInfo("인증 메일이 전송되었습니다.");
            SetInteractable(true);
        });
    }

    private void Cancel()
    {
        panelController.SetActivePanel(LoginPanelController.Panel.Login);
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        sendButton.interactable = interactable;
        cancelButton.interactable = interactable;
    }

}
