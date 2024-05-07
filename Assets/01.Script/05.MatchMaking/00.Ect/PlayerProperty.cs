using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperty : MonoBehaviour
{
    Chat chat; //대화창
    Player player; //우클릭한 대상
    [SerializeField] RoomPanel roomManager; //룸패널
    [SerializeField] Button whisper; //귓속말 버튼
    [SerializeField] Button getOut; //추방 버튼
    [SerializeField] Button teamChange; //팀 이동 버튼
    bool isMaster { get { return roomManager.isMaster; } }
    private void Awake()
    {
        whisper.onClick.AddListener(Whisper); //귓속말 버튼에 귓속말 함수 연결
        getOut.onClick.AddListener(GetOut); //추방 버튼에 추방 함수 연결
        teamChange.onClick.AddListener(TeamChange); //팀 변경 버튼에 빔 변경 함수 연결
    }
    private void OnEnable()
    {
        if(isMaster)
        {
            //속성버튼을 클릭한 클라이언트가 마스터라면 추방과 팀 변경 버튼이 비활성화되어있다면 활성화한다.
            if (getOut.gameObject.activeSelf == false)
                getOut.gameObject.SetActive(true);
            if (teamChange.gameObject.activeSelf == false)
                teamChange.gameObject.SetActive(true);
        }
        else
        {
            //속성버튼을 클릭한 클라이언트가 마스터가 아니라면 추방과 팀 변경 버튼이 활성화되어있다면 비활성화한다.
            if (getOut.gameObject.activeSelf)
                getOut.gameObject.SetActive(false);
            if (teamChange.gameObject.activeSelf)
                teamChange.gameObject.SetActive(false);
        }
    }
    public void SetPlayer(Player _player)
    {
        if (_player != null) //우클릭 객체가 비어있는지 확인
            player = _player; //있다면 대입
        else
            gameObject.SetActive(false); //없다면 오류기때문에 비활성화
    }
    public void SetChat(Chat _chat)
    {
        chat = _chat; //대화창 연결
    }

    void Whisper()
    {
        //채팅을 보낼 상대를 설정한다.
        chat.SendTarget(player);
        //임무를 마쳤기에 비활성화
        gameObject.SetActive(false);
    }

    void GetOut()
    {
        //추방시킨다.
        PhotonNetwork.CloseConnection(player);
        //임무를 마쳤기에 비활성화
        gameObject.SetActive(false);
    }

    void TeamChange()
    {
        //최대 설정인원의 반을 저장한다.
        int halfCount = PhotonNetwork.CurrentRoom.MaxPlayers >> 1;
        int num = 1;
        //현재 팀 코드가 1이면 변경할 팀 코드를 2로 저장한다.
        if (num == player.GetPhotonTeam().Code)
            num = 2;
        //변경할 팀의 인원을 가져온다.
        int teamCount = PhotonTeamsManager.Instance.GetTeamMembersCount((byte)num);
        //변경할 팀의 인원이 찼으면 팀 변경을 시도하지 않는다.
        //팀의 인원이 안찼으면 팀 변경을 시도한다.
        if (halfCount > teamCount)
            player.SwitchTeam((byte)num);
        //임무를 마쳤기에 비활성화
        gameObject.SetActive(false);
    }
}
