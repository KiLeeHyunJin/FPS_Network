using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerReady;
    [SerializeField] Button playerReadyButton;
    [SerializeField] TMP_Text buttonName;
    [SerializeField] int team;

    [SerializeField] Image readyOn;

    public int Team { get { return team; } }
    public bool ReadyState { get; private set; }
    public Player Player { get { return player; } }
    Player player;
    PlayerProperty property;
    Action<PlayerEntry, int> changeTeamMethod;
    bool isMine;
    public void SetPlayer(Player player , PlayerProperty buttons, Action<PlayerEntry, int> _changeTeam , int teamType = 0)
    {
        this.player = player;
        playerName.text = player.NickName; //플레이어 닉네임 출력
        changeTeamMethod = _changeTeam; //액션 함수 연결
        isMine = player.IsLocal; //본인 객체인지 확인
        buttonName.text = isMine ? "준비" : "속성"; //본인 객체라면 준비 표기, 아니라면 속성 표기
        ReadyState = player.GetProperty<bool>(DefinePropertyKey.READY);  //레디인지 설정 
        team = teamType; //팀 타입 설정

        ChangeReady(ReadyState); //준비상태인지 업데이트
        property = buttons;//클릭 버튼
        if (teamType != 0) //팀 타입이 0이 아니라면  팀 매니저에 업데이트 
            player.JoinTeam((byte)teamType);
    }
    public void Ready()
    {
        //준비 버튼을 클릭할 경우
      
        if (isMine) //본인 객체를 눌렀다면
        {
            bool ready = player.GetProperty<bool>(DefinePropertyKey.READY);
            ready = !ready;
            player.SetProperty(DefinePropertyKey.READY, ready);
            PhotonNetwork.AutomaticallySyncScene = ready;
           
        }
        else //상대 객체를 눌렀다면
        {
            //속성창 출력
            property.gameObject.SetActive(true);
            //속성 대상 설정
            property.SetPlayer(player);
            //위치를 마우스 위치로 설정
            RectTransform rect = property.transform as RectTransform;
            if(rect != null)
                rect.position = Input.mousePosition;
           
        }
       
    }

    public void UpdateProperty(PhotonHashtable property)
    {
        
        if (player.GetPhotonTeam() != null) //팀 업데이트
            ChangeTeam(player.GetPhotonTeam().Code);
        //준비 상태 업데이트
        bool ready = property.ContainsKey(DefinePropertyKey.READY) ?
           (bool)property[DefinePropertyKey.READY] : player.GetProperty<bool>(DefinePropertyKey.READY);
        ChangeReady(ready);
    }
    void ChangeReady(bool ready)
    {
        playerReady.text = ready ? "Ready" : "";
        ReadyState = ready;
        readyOn.gameObject.SetActive(ready);
        Color color = team == 1 ? new Color(0f, 0f, 1f, 0.118f) : new Color(1f, 0f, 0f, 0.118f);
        readyOn.color = color;
            
    }
    void ChangeTeam(int changeTeamValue)
    {
        //액션 함수 호출
        changeTeamMethod.Invoke(this, changeTeamValue);
        //팀이 변경되었다면 새로운 값 대입
        if(team != changeTeamValue)
            team = changeTeamValue;
    }
}
