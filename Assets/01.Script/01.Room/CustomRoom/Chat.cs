using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviourPun
{
    public enum ChatType { ALL, TARGET, TEAM, NEW, MY, END }
    public enum InChatType { InGame,Lobby}
    [SerializeField] public InChatType inChatType {  get; set; }   
    [SerializeField] Button chatResetButton; //대화창 초기화 버튼
    [SerializeField] TMP_InputField inputField; //입력창
    [SerializeField] RectTransform content; //대화목록 부모
    [SerializeField] ScrollRect scrollRect; //스크롤
    [SerializeField] TMP_Text chatTextPrefab; //대화를 복사할 객체
    [SerializeField] ChatType chatTarget; //전송 타입
    Player currentMessageTarget; //전송 타겟

    private void Awake()
    {
        inputField.onSubmit.AddListener(SendChat); //엔터할때 전송하도록 이벤트 붙여넣는다.
        gameObject.GetOrAddComponent<PhotonView>(); //포톤뷰가 없을 경우 포톤뷰를 붙인다.
        if(chatResetButton!=null)
        chatResetButton.onClick.AddListener(RemoveEntry); //초기화 버튼에 초기화 함수 이벤트를 붙여넣는다.
    }
    private void OnEnable()
    {
        
        chatTarget = ChatType.ALL; //전송타입을 전채로 설정
        //합류 메시지 전송
        photonView.RPC("AddMessage", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.NickName}이 합류하였습니다.", ChatType.NEW, (byte)0);
    }
    private void OnDisable()
    {
        RemoveEntry();  //모든 대화목록 삭제(대화 객체 파괴)
    }

    public void LeftPlayer(Player otherPlayer)
    {
        //현재 귓속말 상대가 방을 나갔다면
        if (currentMessageTarget!=null && currentMessageTarget.ActorNumber == otherPlayer.ActorNumber)
        {
            //타겟을 비우고 채팅 타입을 전체채팅으로 변경한다.
            currentMessageTarget = null;
            chatTarget = ChatType.ALL;
        }
        AddMessage($"{otherPlayer.NickName}이 퇴장하였습니다.", Chat.ChatType.NEW, (byte)0);
    }
    public bool isTest(Player player)
    {
        if (player == currentMessageTarget)
            return true;
        else
            return false;
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
        if (chat == "")
            return;
        SendMessageToTarget($"{PhotonNetwork.LocalPlayer.NickName} : {chat}");
        //입력창을 비우고 다시 활성화 상태로 변경한다.
        inputField.text = "";
        inputField.ActivateInputField();
    }

    [PunRPC]
    public void AddMessage(string message, ChatType chatType, byte teamCode = 0)
    {
        //if (chatTarget == ChatType.TEAM) //팀 대화 목록이고
        //    if((int)teamCode != (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code) //팀이 다르다면 종료
        //        return;
        
        TMP_Text newMessage = Instantiate(chatTextPrefab);

        if(inChatType == InChatType.Lobby)
        {
            switch (chatType) //타입에 따라 폰트 색상 변경
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
                case ChatType.MY:
                    newMessage.color = Color.green;
                    break;
                case ChatType.NEW:
                    newMessage.color = Color.yellow;
                    newMessage.fontStyle |= FontStyles.Bold; //폰트를 두껍게 설정
                    break;
            }
        }
        else if ( inChatType == InChatType.InGame)
        {
            switch (chatType) 
            {
                case ChatType.ALL:
                    newMessage.color = Color.white;
                    break;
                case ChatType.TARGET:
                    newMessage.color = Color.red;
                    break;
                case ChatType.TEAM:
                    newMessage.color = Color.blue;
                    break;
                case ChatType.MY:
                    newMessage.color = Color.green;
                    break;
                case ChatType.NEW:
                    newMessage.color = Color.yellow;
                    newMessage.fontStyle |= FontStyles.Bold; //폰트를 두껍게 설정
                    break;
            }
        }
        
        newMessage.text = message;  //내용 입력
        newMessage.transform.SetParent(content); //객체를 콘텐츠의 자식으로 설정
        RectTransform rect = newMessage.transform as RectTransform;
        if (rect != null) // 스케일을 1로 변경
            rect.localScale = Vector3.one;
        scrollRect.verticalScrollbar.value = 0; //스크롤 뷰를 맨 밑으로 설정
        if (inChatType == InChatType.InGame)
            inputField.gameObject.SetActive(false);
    }


    // 클라이언트 변조로 인하여 도청의 위험이 존재
    void SendMessageToTarget(string chat)
    {
        byte defaultParam = default(byte);
        switch (chatTarget)
        {
            case ChatType.ALL:
                photonView.RPC("AddMessage", RpcTarget.Others, chat, ChatType.ALL, defaultParam);
               
                break;
            case ChatType.TARGET:
                photonView.RPC("AddMessage", currentMessageTarget, $"[Whisper]  {chat}", ChatType.TARGET, defaultParam);
                
                break;
            case ChatType.TEAM:
                SendTeam(chat);
                break;
        }
        if(chatTarget != ChatType.TARGET)
            AddMessage(chat, chatTarget);
        else
            AddMessage(chat, ChatType.MY);

    }
    void SendTeam(string chat)
    {
        //팀 코드를 가져온다.
        byte code = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        //해당 코드의 팀 맴버를 가져온다.
        PhotonTeamsManager.Instance.TryGetTeamMembers(code, out Player[] teams);
        if (teams != null)
        {
            foreach (Player target in teams) //팀원 목록을 돌며 전송을 한다.
            {
                photonView.RPC("AddMessage", target, chat, ChatType.TEAM, code);
            }
        }
    }

    void RemoveEntry()
    {
        for (int i = 0; i < content.childCount; i++)
            Destroy(content.GetChild(i).gameObject);
    }
}
