using Firebase.Extensions;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Firebase.Database;
using System.Collections.Generic;
using UnityEditor.Playables;
using System;
public class MenuPanel : MonoBehaviour
{
    [SerializeField] GameObject PlayButton;
    [SerializeField] GameObject PlayButtons;
    [SerializeField] GameObject userInfoWindow;

    [SerializeField] Button userInfoButton;
    [SerializeField] Button openButton;
    [SerializeField] Button closeButton;
    [SerializeField] Button logoutButton;
    [SerializeField] Button lobbyButton;
    [SerializeField] Button randomButton;

    LobbyData data;

    private void Awake()
    {
        userInfoButton.onClick.AddListener(() => { userInfoWindow.SetActive(true); });
        openButton.onClick.AddListener(()=> { OpenPlayButtons(true); });
        closeButton.onClick.AddListener(()=> { OpenPlayButtons(false); });
        logoutButton.onClick.AddListener(Logout);
        lobbyButton.onClick.AddListener(JoinLobby);
        randomButton.onClick.AddListener(RandomMatching);
    }
    [Serializable]
    public class NickNames
    {
        public string[] nickNames;
    }
    private void Start()
    {


    }
    private void OnEnable()
    {
        OpenPlayButtons(false);
    }

    public void RandomMatching()
    {
        data.SetLobbyState(LobbyData.LobbyState.Random,true);
        JoinLobby();
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    void Logout()
    {
        FireBaseManager.Auth.SignOut();
        PhotonNetwork.Disconnect();
    }

    public void OpenPlayButtons(bool state)
    {
        PlayButton.SetActive(!state);
        PlayButtons.SetActive(state);
    }
}
