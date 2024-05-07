using Firebase;
using Firebase.Auth;
using System;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : PooledObject
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] Button closeButton;
    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public void ShowInfo(string message)
    {
        gameObject.SetActive(true);
        infoText.text = message;
    }
    //교수님 코드 확장
    public void ShowError(ReadOnlyCollection<Exception> exceptions, string str)
    {
        foreach (System.Exception innerException in exceptions)
        {
            if (innerException is FirebaseException authException)
            {
                //에러코드와 같이 출력 
                AuthError errorCode = (AuthError)authException.ErrorCode;
                ShowInfo($"{str}\n ErrorCode : {errorCode}");
            }
        }
    }
    public void Close()
    {
        Release();
    }
}
