
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;



public class ConnectManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Menu, Lobby, Room, Random }
    [SerializeField] GameObject loginPanel;
    [SerializeField] MenuPanel menuPanel;
    [SerializeField] RoomPanel roomPanel;
    [SerializeField] LobbyPanel lobbyPanel;

    [SerializeField] RandomMatchPanel randomPanel;
    LobbyData data;
    private ClientState state;
    private void Awake()
    {
        data = GetComponent<LobbyData>();
        StartCoroutine(UpUi());
        Debug.Log("StartOn");
    }
    IEnumerator UpUi()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log(Manager.Game.afterGame);

        if (Manager.Game.afterGame)
        {
            SetActivePanel(Panel.Room);
            Manager.Game.afterGame = false;
            
        }
        else
        {
            Debug.Log("Login");
            SetActivePanel(Panel.Login);
        }
    }

    public void Update()
    {
        ClientState currentState = PhotonNetwork.NetworkClientState;
        if (state == currentState)
            return;
        state = currentState;
        Debug.Log(currentState);
    }
    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject?.SetActive(panel == Panel.Login);
        menuPanel.gameObject?.SetActive(panel == Panel.Menu);
        roomPanel.gameObject?.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject?.SetActive(panel == Panel.Lobby);
        randomPanel.gameObject?.SetActive(panel == Panel.Random);

        if(Manager.Scene != null)
        Manager.Scene.StartFadeIn();
    }
    public override void OnJoinedRoom()
    {
        if(data.GetLobbyState(LobbyData.LobbyState.Random) == false)
            SetActivePanel(Panel.Room);
        Debug.Log($"Join Room Success");
    }
    public override void OnCreatedRoom()
    {
        Debug.Log($"Create Room Success");
    }
    public override void OnLeftRoom()
    {
        Debug.Log("leftRoom");
        
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Random Room Failed with Error : {returnCode}, {message}");
    }
    public override void OnJoinedLobby()
    {
        if(data.GetLobbyState(LobbyData.LobbyState.Random)) //랜덤 매칭일 경우 랜덤 화면 출력
            SetActivePanel(Panel.Random);
        else
            SetActivePanel(Panel.Lobby);
    }
    public override void OnLeftLobby()
    {
       
        SetActivePanel(Panel.Menu);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Create Room Failed With Error : {returnCode} , :{message}");
    }

    public override void OnConnected()
    {
        Debug.Log("isConnect");
        PhotonNetwork.NickName = FireBaseManager.Auth.CurrentUser.DisplayName;
        
        SetActivePanel(Panel.Menu);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false) //랜덤 매칭이 아닐 경우 로비 업데이트
            lobbyPanel.UpdateRoomList(roomList);
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        roomPanel.RoomPropertiesUpdate(propertiesThatChanged);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false) //랜덤 매칭이 아닐 경우 룸패널 내용 실행
            roomPanel.PlayerPropertiesUpdate(targetPlayer, changedProps);
        if (PhotonNetwork.CurrentRoom.GetProperty<bool>(DefinePropertyKey.START))
            roomPanel.GameLoadPropertiesUpdate(targetPlayer, changedProps);
            

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false)//랜덤 매칭이 아닐 경우 룸패널 내용 실행
            roomPanel.PlayerEnterRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false)//랜덤 매칭이 아닐 경우 룸패널 내용 실행
            roomPanel.PlayerLeftRoom(otherPlayer);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false)//랜덤 매칭이 아닐 경우 룸패널 내용 실행
            roomPanel.MasterClientSwitched(newMasterClient);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByServerLogic)
            return;
        SetActivePanel(Panel.Login);
        Debug.Log("OnDisconnected");
    }

}
