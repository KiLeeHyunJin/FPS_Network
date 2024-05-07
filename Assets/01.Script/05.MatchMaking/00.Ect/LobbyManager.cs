
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class LobbyManager : MonoBehaviourPunCallbacks
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
    }
    private void Start()
    {
        SetActivePanel(Panel.Login);
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
        SetActivePanel(Panel.Menu);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Random Room Failed with Error : {returnCode}, {message}");
    }
    public override void OnJoinedLobby()
    {
        if(data.GetLobbyState(LobbyData.LobbyState.Random))
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
        PhotonNetwork.NickName = FireBaseManager.Auth.CurrentUser.DisplayName;
        SetActivePanel(Panel.Menu);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false)
            lobbyPanel.UpdateRoomList(roomList);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false)
            roomPanel.PlayerPropertiesUpdate(targetPlayer, changedProps);

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false)
            roomPanel.PlayerEnterRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false)
            roomPanel.PlayerLeftRoom(otherPlayer);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (data.GetLobbyState(LobbyData.LobbyState.Random) == false)
            roomPanel.MasterClientSwitched(newMasterClient);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByServerLogic)
            return;
        SetActivePanel(Panel.Login);
    }

}
