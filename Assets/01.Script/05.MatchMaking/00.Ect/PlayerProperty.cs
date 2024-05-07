using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperty : MonoBehaviour
{
    Chat chat;
    Player player;
    [SerializeField] RoomPanel roomManager;
    [SerializeField] Button whisper;
    [SerializeField] Button getOut;
    [SerializeField] Button teamChange;
    public bool isMaster { get { return roomManager.isMaster; } }
    private void Awake()
    {
        whisper.onClick.AddListener(Whisper);
        getOut.onClick.AddListener(GetOut);
        teamChange.onClick.AddListener(TeamChange);
    }
    private void Start()
    {
        chat = chat != null ? chat : FindObjectOfType<Chat>();
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
        if (_player != null)
            player = _player;
        else
            gameObject.SetActive(false);
    }
    public void SetChat(Chat _chat)
    {
        chat = _chat;
    }

    void Whisper()
    {
        //채팅을 보낼 상대를 저장한다.
        chat.SendTarget(player);
        gameObject.SetActive(false);
    }

    void GetOut()
    {
        //추방시킨다.
        PhotonNetwork.CloseConnection(player);
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
        gameObject.SetActive(false);
    }
}
