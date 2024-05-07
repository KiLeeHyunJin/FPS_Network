using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using System;

public class LoginPanel : MonoBehaviourShowInfo
{
    [SerializeField] LoginPanelController panelController; 

    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passInputField; 

    [SerializeField] Button signUpButton; //회원가입 버튼
    [SerializeField] Button loginButton; //로그인 버튼
    [SerializeField] Button resetPasswordButton; //비밀번호 재설정 버튼


    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(Login);
        resetPasswordButton.onClick.AddListener(ResetPassword);
    }
    private void OnEnable()
    {
        SetInteractable(true);
    }
    public void SignUp()
    {
        panelController.SetActivePanel(LoginPanelController.Panel.SignUp);
    }

    private void ResetPassword()
    {
        panelController.SetActivePanel(LoginPanelController.Panel.Reset);
    }

    public void Login()
    {
        string id = emailInputField.text;
        string pw = passInputField.text;
        passInputField.text = ""; //비밀번호 입력란 초기화

        SetInteractable(false);

        FireBaseManager.Auth.SignInWithEmailAndPasswordAsync(id, pw).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("접속 시도가 취소되었습니다.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "접속에 실패하였습니다.");
                SetInteractable(true);
                return;
            }
            SetInteractable(true);

            if(FireBaseManager.Auth.CurrentUser.IsEmailVerified)
            {
                if(FireBaseManager.Auth.CurrentUser.DisplayName.IsNullOrEmpty() == false)
                {
                    Photon.Pun.PhotonNetwork.ConnectUsingSettings();
                }
                else
                {
                    panelController.SetActivePanel(LoginPanelController.Panel.CreateNickName);
                }
            }
            else
            {
                panelController.SetActivePanel(LoginPanelController.Panel.Verify);
            }
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat($"User signed, in successfullt ; {0} ({1})", result.User.DisplayName, result.User.UserId);
        });
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passInputField.interactable = interactable;
        signUpButton.interactable = interactable;
        loginButton.interactable = interactable;
        resetPasswordButton.interactable = interactable;
    }
}
