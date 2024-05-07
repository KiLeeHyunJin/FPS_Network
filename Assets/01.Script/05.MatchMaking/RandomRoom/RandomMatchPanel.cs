using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

public class RandomMatchPanel : MonoBehaviour
{
    [SerializeField] int playerCount;
    LobbyData data;
    PhotonTeamsManager teamsManager;
    public void OnEnable()
    {
        if (data == null)
            data = FindObjectOfType<LobbyData>();
        teamsManager = PhotonTeamsManager.Instance;
        data.ResetState(LobbyData.LobbyState.Random);
        string room = $"Room {Random.Range(1000, 10000)}";
        RoomOptions option = new RoomOptions { MaxPlayers = 4, IsVisible = false };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: room, roomOptions: option);
    }
    public void OnDisable()
    {

    }
    public void EnterPlayer()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (PhotonNetwork.IsMasterClient)
        {
            if (playerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonTeam team = new PhotonTeam();
                team.Name = "A";
                PhotonNetwork.LocalPlayer.JoinTeam(team);
                GameStart();
            }
        }
    }
    void GameStart()
    {

    }
    public void LeftPlayer()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
