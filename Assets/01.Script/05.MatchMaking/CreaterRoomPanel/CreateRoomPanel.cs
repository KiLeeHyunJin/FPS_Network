using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanel : MonoBehaviour
{
    [SerializeField] Button createRoomButton;
    [SerializeField] Button createCancleButton;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Dropdown maxPlayerDropdown;


    void Awake()
    {
        createCancleButton.onClick.AddListener(CreateRoomCancel);
        createRoomButton.onClick.AddListener(CreateRoomConfirm);
    }

    void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        if (roomName == "")
            roomName = Random.Range(1000, 10000).ToString();

        int halfNum = int.Parse(maxPlayerDropdown.captionText.text[0].ToString()); //첫번째 글자의 값을 정수로 변환
        int maxPlayer = halfNum << 1; //값을 2배로 설정

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayer }; //최대인원으로 옵션 설정
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    void CreateRoomCancel()
    {
        gameObject.SetActive(false);
    }
}
