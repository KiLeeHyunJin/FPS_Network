using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class RoomPanel : MonoBehaviourShowInfo
{
    [SerializeField] string gameSceneName;
    [SerializeField] Button startButton;
    [SerializeField] Button leaveButton;

    [SerializeField] RectTransform redTeam;
    [SerializeField] RectTransform blueTeam;

    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] TextMeshProUGUI roomNameTxt;
    [SerializeField] PlayerProperty playerProperty;
    [SerializeField] Chat chat;

    List<PlayerEntry> playerList;
    Room currentRoom;

    const int RED = 2;
    const int BLUE = 1;
    int halfCount;
    bool isEnterGame;
    public bool isMaster { get; private set; }
    private void Awake()
    {
        playerList = new List<PlayerEntry>();
        startButton.onClick.AddListener(StartGame);
        leaveButton.onClick.AddListener(LeaveRoom);
    }
    private void OnEnable()
    {
        isEnterGame = false;
        currentRoom = PhotonNetwork.CurrentRoom;
        //방 이름 표기
        roomNameTxt.text = currentRoom.Name;
        //팀별 최대 인원을 계산해서 가져온다.
        halfCount = (currentRoom.MaxPlayers >> 1);
        isMaster = PhotonNetwork.IsMasterClient;
        //게임 플레이 버튼 소유권자일 경우 활성화
        startButton.gameObject.SetActive(isMaster);
        //플레이어 리스트 초기화
        playerList.Clear();

        //로컬 플레이어 프로퍼티 초기화
        PhotonNetwork.LocalPlayer.SetProperty(DefinePropertyKey.READY, false);
        PhotonNetwork.LocalPlayer.SetProperty(DefinePropertyKey.LOAD, false);

        //플레이어 리스트를 돌면서 플레이어 엔트리 생성
        foreach (Player player in PhotonNetwork.PlayerList)
            PlayerSet(player);

        //우클릭 목록에 채팅 연결
        playerProperty.SetChat(chat);
        //우클릭 목록 비활성화
        if (playerProperty.gameObject.activeSelf)
            playerProperty.gameObject.SetActive(false);

    }
    private void OnDisable()
    {
        //팀별로 돌면서 플레이어 엔트리 파괴
        ClearRoomData(redTeam);
        ClearRoomData(blueTeam);
    }
    void ClearRoomData(RectTransform team)
    {
        for (int i = 0; i < team.childCount; i++)
            Destroy(team.GetChild(i).gameObject);
    }

    void PlayerSet(Player newPlayer)
    {
        //부모객체를 블루팀으로 설정(임시로)
        Transform parent = blueTeam;
        //임시 팀코드 설정
        int teamType = 0;

        if (newPlayer.IsLocal) //본인 플레이어인지 확인
        {
            if (PhotonNetwork.IsMasterClient) //방장인지 확인
                newPlayer.JoinTeam(new PhotonTeam() { Code = BLUE }); //블루팀으로 설정
        }
        else
        {
            //팀 코드를 가져온다.
            teamType = newPlayer.GetPhotonTeam().Code;
            //팀이 블루팀이면 부모객체를 블루팀으로 아니면 레드팀으로 설정
            parent = (teamType == BLUE) ? blueTeam : redTeam;
            //팀을 가져와서 팀으로 설정(이래야 이후에 들어온 유저 팀 매니저에 데이터가 업데이트됨(자동 동기화를 지원 안함))
            newPlayer.JoinTeam(newPlayer.GetPhotonTeam());
        }
        //객체를 복사해서 해당 부모객체의 자식으로 설정
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, parent);
        //객체에 초기 데이터를 설정
        playerEntry.SetPlayer(newPlayer, playerProperty, ChangeTeam, teamType);
        //플레이어 목록에 추가
        playerList.Add(playerEntry);
    }
    public void PlayerLeftRoom(Player otherPlayer)
    {
        //플레이어 리스트에서 플레이어 제거
        RemovePlayer(otherPlayer, playerList);
        //플레이어가 나갔다는 메시지 출력
        chat.LeftPlayer(otherPlayer);
    }

    public void PlayerEnterRoom(Player newPlayer)
    {
        //플레이어 객체 복사해서 블루팀에 추가
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, blueTeam);
        //플레이어 객체에 초기 데이터 설정
        playerEntry.SetPlayer(newPlayer, playerProperty, ChangeTeam);
        //플레이어 리스트에 추가
        playerList.Add(playerEntry);
        //마스터 클라이언트면 플레이어가 들어가야할 팀 선별
        if (PhotonNetwork.IsMasterClient)
            NewPlayerEnterSetTeam(newPlayer);
    }
    void NewPlayerEnterSetTeam(Player newPlayer)
    {
        //블루팀과 레드팀의 팀 인원을 튜플에 추가
        (int blueCount, int redCount) count = (PhotonTeamsManager.Instance.GetTeamMembersCount(BLUE), PhotonTeamsManager.Instance.GetTeamMembersCount(RED));
        int teamType = RED;
        //임시로 레드팀 설정
        //레드팀과 블루팀의 인원이 다를 경우
        if (count.redCount != count.blueCount) //더 적은 팀을 팀타입으로 설정 (팀인원이 같을 경우 레드팀으로 추가)
            teamType = (count.redCount > count.blueCount) ? BLUE : RED;
        //플레이어의 팀을 설정
        newPlayer.JoinTeam(new PhotonTeam() { Code = (byte)teamType });
    }

    void RemovePlayer(Player otherPlayer, List<PlayerEntry> playerList)
    {
        //플레이어 리스트를 돌면서 
        for (int i = 0; i < playerList.Count; i++)
        {
            //액터넘버가 같을 경우 
            if (playerList[i].Player.ActorNumber == otherPlayer.ActorNumber)
            {
                //엔트리 객체 파괴 및 리스트에서 해당 객체 제거
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
        if (halfCount > teamCount)
            if(PhotonNetwork.LocalPlayer.GetProperty<bool>(DefinePropertyKey.READY))
            {
                ShowInfo("준비상태에서는 팀을 바꿀 수 없습니다");
                return;
            }
            else
            {
                Debug.Log("Change");
                PhotonNetwork.LocalPlayer.SwitchTeam((byte)num);
            }
            
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
        //플레이어 리스트를 돌면서 
        foreach (PlayerEntry player in playerList)
        {
            //플레이어 액터가 같을 경우
            if (player.Player.ActorNumber == targetPlayer.ActorNumber)
            {
                //해당 플레이어 업데이트
                player.UpdateProperty(changedProps);
                break;
            }
        }
        //모든 플레이어가 준비되었는지 확인
        AllPlayerReadyCheck();
    }

    void AllPlayerReadyCheck()
    {
        //마스터 클라이언트가 아니면 종료
        if (PhotonNetwork.IsMasterClient == false)
            return;
        //플레이어 리스트를 돌면서
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //준비가 안되어있으면
            if (player.GetProperty<bool>(DefinePropertyKey.READY) == false)
            {
                //비활성화 및 종료
                startButton.interactable = false;
                return;
            }
        }
        //활성화
        startButton.interactable = true;
    }
    void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void MasterClientSwitched(Player newMasterClient)
    {
        //마스터 클라이언트가 현재 플레이어면 시작 버튼 활성화
        if (newMasterClient.IsLocal)
        {
            startButton.gameObject.SetActive(true);
            isMaster = true;
        }
    }

    void ChangeTeam(PlayerEntry playerEntry, int team)
    {
        //플레이어 객체의 팀과 이동하려는 팀이 같을 경우 종료
        if (playerEntry.Team == team)
            return;

        //마스터만 수행
        CheckChangedTeamCount(playerEntry, team);
        //이동하려는 팀이 블루팀이면 부모객체에 블루 아니면 레드 대입
        Transform teamTransform = (team == BLUE) ? blueTeam : redTeam;
        //부모객체 하위로 이동
        playerEntry.transform.SetParent(teamTransform);
    }

    void CheckChangedTeamCount(PlayerEntry player, int team)
    {
        //마스터 클라이언트일 경우
        if (PhotonNetwork.IsMasterClient)
        {
            //이동하려는 팀의 인원이 초과했는지 확인
            int count = PhotonTeamsManager.Instance.GetTeamMembersCount((byte)team);
            // 팀의 인원이 기준을 넘을 경우
            if (count > halfCount)
                //원래 팀으로 이동
                player.Player.SwitchTeam((byte)player.Team);
        }
    }

}
