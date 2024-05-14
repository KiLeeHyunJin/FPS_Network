using Firebase;
using Firebase.Auth;
using System;
using UnityEngine;

public class MonoBehaviourShowInfo : MonoBehaviour
{
    Canvas infoParent;
    [SerializeField] InfoPanel info;
    protected virtual void Start()
    {
        infoParent = GetComponentInParent<Canvas>(); //최 상위 부모 객체를 가져온다.(UI는 Canvas안에서만 출력 가능하기때문)
    }
    InfoPanel GetInfo()
    {
        PooledObject obj = Manager.Pool.GetPool(info, Vector3.zero, Quaternion.identity);//객체를 가져온다.
        obj.transform.SetParent(infoParent.transform, true); //Canvas의 자식으로 설정
        RectTransform rect = obj.transform as RectTransform; 
        if (rect != null)
            rect.offsetMin = rect.offsetMax = Vector2.zero; //최대 사이즈로 설정
        InfoPanel infoPanel = obj as InfoPanel;//언박싱 후 반환
        infoPanel.transform.localScale = Vector3.one;
        return infoPanel;
    }
    protected void ShowInfo(string str)
    {
        Info(GetInfo(), str);
    }
    protected void ShowError(System.Collections.ObjectModel.ReadOnlyCollection<Exception> exceptions, string str)
    {
        InfoPanel infoPanel = GetInfo();
        if (infoPanel != null)
        {
            foreach (System.Exception innerException in exceptions)
            {
                if (innerException is FirebaseException authException)
                {
                    AuthError errorCode = (AuthError)authException.ErrorCode;
                    Info(infoPanel, $"{str}\n ErrorCode : {errorCode}");
                }
            }
        }
    }
    void Info(InfoPanel infoPanel, string str)
    {
        if (infoPanel != null)
        {
            infoPanel.ShowInfo(str);
        }
    }
}
