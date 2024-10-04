using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] string debugRoomName;
    bool created;
    private void Awake()
    {
        created = false;
        if (PhotonNetwork.IsConnected == false) //연결이 안되어있으면 연결
        {
            Photon.Pun.PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnected() //연결 성공시 방 생성
    {
        StartCoroutine(CreateRoom());
    }

    IEnumerator CreateRoom()
    {
        yield return new WaitForSeconds(2); //즉시 방 참여 진입 시 오류가 발생하기에 2초 후 생성
        debugRoomName = string.IsNullOrEmpty(debugRoomName) ? Random.Range(10000, 9999).ToString() : debugRoomName;
        PhotonNetwork.JoinOrCreateRoom(debugRoomName, new RoomOptions{ MaxPlayers = 8,IsVisible = false }, TypedLobby.Default);
    }
    public override void OnJoinedRoom() //방에 진입 시 객체 생성
    {
        if (created)
            return;
        created = true;
        PhotonNetwork.Instantiate("Player",new Vector3(42.25f, 10 , -88), Quaternion.identity, 0);
    }
}
