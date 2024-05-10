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
        roomNameInputField.characterLimit = 15;
    }

    void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        if (roomName == "")
            roomName = Random.Range(1000, 10000).ToString();

        int halfNum = int.Parse(maxPlayerDropdown.captionText.text[0].ToString()); //ù��° ������ ���� ������ ��ȯ
        int maxPlayer = halfNum << 1; //���� 2��� ����

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayer }; //�ִ��ο����� �ɼ� ����
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        gameObject.SetActive(false);
    }
    void CreateRoomCancel()
    {
        gameObject.SetActive(false);
    }
}
