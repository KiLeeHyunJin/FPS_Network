using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviourPun
{
    public enum ChatType { ALL, TARGET, TEAM, NEW, END }

    [SerializeField] Button chatResetButton;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] RectTransform content;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] TMP_Text chatTextPrefab;
    [SerializeField] ChatType chatTarget;
    Player currentMessageTarget;

    private void Awake()
    {
        inputField.onSubmit.AddListener(SendChat);
        gameObject.GetOrAddComponent<PhotonView>();
        chatResetButton.onClick.AddListener(RemoveMessageEntry);
    }
    private void OnEnable()
    {
        chatTarget = ChatType.ALL;
        photonView.RPC("AddMessage", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.NickName}이 합류하였습니다.", ChatType.NEW, (byte)0);
    }
    private void OnDisable()
    {
        RemoveMessageEntry();
    }

    public void LeftPlayer(Player otherPlayer)
    {
        //현재 귓속말 상대가 방을 나갔다면
        if (currentMessageTarget.ActorNumber == otherPlayer.ActorNumber)
        {
            //타겟을 비우고 채팅 타입을 전체채팅으로 변경한다.
            currentMessageTarget = null;
            chatTarget = ChatType.ALL;
        }
        AddMessage($"{otherPlayer.NickName}이 퇴장하였습니다.", Chat.ChatType.NEW, (byte)0);
    }
    public void SendTarget(Player target)
    {
        //타겟이 비어있다면 덮어쓴다.
        if (currentMessageTarget == null)
            currentMessageTarget = target;
        else //타겟이 현재 설정되어있는 타겟과 동일하다면 비운다.
            currentMessageTarget = currentMessageTarget.ActorNumber == target.ActorNumber ? null : target;
        //현재 타겟이 비워저있으면 전체채팅으로 있다면 귓속말로 설정한다.
        chatTarget = currentMessageTarget == null ? ChatType.ALL : ChatType.TARGET;
    }
    void SendChat(string chat)
    {
        //내용을 보낸다.
        SendMessageToTarget(chat);
        //입력창을 비우고 다시 활성화 상태로 변경한다.
        inputField.text = "";
        inputField.ActivateInputField();
    }

    [PunRPC]
    public void AddMessage(string message, ChatType chatType, byte teamCode = 0)
    {
        //대화창을 복사한다.
        TMP_Text newMessage = Instantiate(chatTextPrefab);
        //ui transform컴포넌트를 가져온다.
        RectTransform rect = newMessage.transform as RectTransform;
        //
        if (rect != null)
            rect.localScale = Vector3.one;

        switch (chatType)
        {
            case ChatType.ALL:
                newMessage.color = Color.black;
                break;
            case ChatType.TARGET:
                newMessage.color = Color.red;
                break;
            case ChatType.TEAM:
                newMessage.color = Color.blue;
                break;
            case ChatType.NEW:
                newMessage.color = Color.yellow;
                newMessage.fontStyle |= FontStyles.Bold;
                break;
        }
        newMessage.text = message;
        newMessage.transform.SetParent(content);
        scrollRect.verticalScrollbar.value = 0;
    }


    // 클라이언트 변조로 인하여 도청의 위험이 존재
    void SendMessageToTarget(string chat)
    {
        byte defaultParam = default(byte);
        switch (chatTarget)
        {
            case ChatType.ALL:
                photonView.RPC("AddMessage", RpcTarget.All, chat, ChatType.ALL, defaultParam);
                break;
            case ChatType.TARGET:
                photonView.RPC("AddMessage", currentMessageTarget, chat, ChatType.TARGET, defaultParam);
                break;
            case ChatType.TEAM:
                SendTeam(chat);
                break;
        }
    }
    void SendTeam(string chat)
    {
        byte code = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        PhotonTeamsManager.Instance.TryGetTeamMembers(code, out Player[] teams);
        if (teams != null)
        {
            foreach (Player target in teams)
            {
                photonView.RPC("AddMessage", target, chat, ChatType.TEAM, code);
            }
        }
    }

    void RemoveMessageEntry()
    {
        for (int i = 0; i < content.childCount; i++)
            Destroy(content.GetChild(i).gameObject);
    }
}
