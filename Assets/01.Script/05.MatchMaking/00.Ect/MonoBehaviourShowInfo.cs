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
        infoParent = GetComponentInParent<Canvas>();
    }
    InfoPanel GetInfo()
    {
        PooledObject obj = Manager.Pool.GetPool(info, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(infoParent.transform, true);
        RectTransform rect = obj.transform as RectTransform;
        if (rect != null)
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        InfoPanel infoPanel = obj as InfoPanel;
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
