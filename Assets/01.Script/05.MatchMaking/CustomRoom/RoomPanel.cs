using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] string gameSceneName;
    [SerializeField] Button startButton;
    [SerializeField] Button leaveButton;

    [SerializeField] RectTransform redTeam;
    [SerializeField] RectTransform blueTeam;

    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] TextMeshProUGUI roomNameTxt;
    List<PlayerEntry> playerList;
    [SerializeField] PlayerProperty playerProperty;
    Room currentRoom;
    [SerializeField] Chat chat;
    int halfCount;
    const int RED = 2;
    const int BLUE = 1;
    bool isEnterGame;
    public bool isMaster { get; private set; }
    private void Awake()
    {
        playerList = new List<PlayerEntry>();
        playerProperty.gameObject.SetActive(false);
        startButton.onClick.AddListener(StartGame);
        leaveButton.onClick.AddListener(LeaveRoom);
    }
    private void OnEnable()
    {
        isEnterGame = false;
        currentRoom = PhotonNetwork.CurrentRoom;
        roomNameTxt.text = currentRoom.Name;
        halfCount = (currentRoom.MaxPlayers >> 1);
        isMaster = PhotonNetwork.IsMasterClient;
        startButton.gameObject.SetActive(isMaster);
        playerList.Clear();

        PhotonNetwork.LocalPlayer.SetProperty(DefinePropertyKey.READY, false);
        PhotonNetwork.LocalPlayer.SetProperty(DefinePropertyKey.LOAD, false);

        foreach (Player player in PhotonNetwork.PlayerList)
            PlayerSet(player);
        playerProperty.SetChat(chat);
    }
    private void OnDisable()
    {
        ClearRoomData(redTeam);
        ClearRoomData(blueTeam);
    }
    void ClearRoomData( RectTransform team)
    {
        for (int i = 0; i < team.childCount; i++)
            Destroy(team.GetChild(i).gameObject);
    }

    void PlayerSet(Player newPlayer)
    {
        Transform parent = blueTeam;

        int teamType = 0;
        if (newPlayer.IsLocal)
        {
            if (PhotonNetwork.IsMasterClient)
                newPlayer.JoinTeam(new PhotonTeam() { Code = BLUE});
        }
        else
        {
            teamType = newPlayer.GetPhotonTeam().Code;
            parent = (teamType == BLUE) ? blueTeam : redTeam;
            newPlayer.JoinTeam(newPlayer.GetPhotonTeam());
        }
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, parent);
        playerEntry.SetPlayer(newPlayer, this, playerProperty, ChangeTeam, teamType);
        playerList.Add(playerEntry);
    }
    public void PlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayer(otherPlayer, playerList);
        chat.LeftPlayer(otherPlayer);
    }

    public void PlayerEnterRoom(Player newPlayer)
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, blueTeam);
        playerEntry.SetPlayer(newPlayer, this, playerProperty, ChangeTeam);
        playerList.Add(playerEntry);
        if (PhotonNetwork.IsMasterClient)
            NewPlayerEnterSetTeam(newPlayer);
    }
    void NewPlayerEnterSetTeam(Player newPlayer)
    {
        (int blueCount, int redCount) count = (PhotonTeamsManager.Instance.GetTeamMembersCount(BLUE), PhotonTeamsManager.Instance.GetTeamMembersCount(RED));
        int teamType = (RED);
        if (count.redCount != count.blueCount)
            teamType = (count.redCount > count.blueCount) ? BLUE : RED;
        newPlayer.JoinTeam(new PhotonTeam() { Code = (byte)teamType });
    }

    void RemovePlayer(Player otherPlayer, List<PlayerEntry> playerList)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].Player.ActorNumber == otherPlayer.ActorNumber)
            {
                Destroy(playerList[i].gameObject);
                playerList.RemoveAt(i);
                return;
            }
        }
    }

    public void TeamChange(int num)
    {
        num += BLUE;
        if (num == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            return;
        int teamCount = PhotonTeamsManager.Instance.GetTeamMembersCount((byte)num);
        if(halfCount > teamCount)
            PhotonNetwork.LocalPlayer.SwitchTeam((byte)num);
    }
    void StartGame()
    {
        if (isEnterGame)
            return;
        isEnterGame = true;
        PhotonNetwork.LoadLevel(gameSceneName);

    }
    public void PlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        foreach (PlayerEntry player in playerList)
        {
            if (player.Player.ActorNumber == targetPlayer.ActorNumber)
            {
                player.UpdateProperty(changedProps);
                break;
            }
        }
        AllPlayerReadyCheck();
    }

    void AllPlayerReadyCheck()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetProperty<bool>(DefinePropertyKey.READY) == false)
            {
                startButton.interactable = false;
                return;
            }
        }
        startButton.interactable = true;
    }
    void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void MasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            startButton.gameObject.SetActive(true);
            isMaster = true;
        }
    }

    void ChangeTeam(PlayerEntry playerEntry, int team)
    {
        if (playerEntry.Team == team)
            return;
        CheckChangedTeamCount(playerEntry, team);
        Transform teamTransform = (team == BLUE) ? blueTeam : redTeam;
        playerEntry.transform.SetParent(teamTransform);
    }

    void CheckChangedTeamCount(PlayerEntry player, int team)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int count = PhotonTeamsManager.Instance.GetTeamMembersCount((byte)team);
            if (count > halfCount)
                player.Player.SwitchTeam((byte)player.Team);
        }
    }

}
