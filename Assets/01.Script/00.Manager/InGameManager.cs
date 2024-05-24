using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGameManager : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField] Transform redLocation;
    [SerializeField] Transform blueLocation;
    [SerializeField] int readyPlayer;
    [SerializeField] PhotonView pv;
    [SerializeField] TMP_Text inGameTimer;
    [SerializeField] float countValue;
    [SerializeField] float roundTimeValue;

    [SerializeField] Transform blueTeamEntryList;
    [SerializeField] Transform redTeamEntryList;

    [SerializeField] List<Player> bluePlayerList;
    [SerializeField] List<Player> redPlayerList;

    [SerializeField] GameObject inGameEntryPrefab;

    [SerializeField] GameObject chatPanel;
    [SerializeField] TMP_InputField chat;
    Coroutine chatRoutine;
    [SerializeField] float chatViewTime;

    [SerializeField] int curRound;
    [SerializeField] int roundCount;

    [SerializeField] TapUI tapUi;

    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject ShopCanvasPrefab;

    [SerializeField] ShopUIManager shopManager;
    Room curRoom = PhotonNetwork.CurrentRoom;

    void Start()
    {

        tapUi.SetTapList();
        bluePlayerList = new List<Player>();
        redPlayerList = new List<Player>();
        EntryListInit();
        Manager.Scene.StartFadeIn();

        MessageUp("곧 게임이 시작됩니다");


        if (PhotonNetwork.InRoom == false)
            return;

        AllPlayerReadyCheck();
    }
    public void OnTab(InputValue value)
    {

        tapUi.gameObject.SetActive(!tapUi.gameObject.activeSelf);
        tapUi.test();

    }
    public void OnEnter(InputValue value)
    {
        chat.gameObject.SetActive(true);
        if (chatRoutine != null)
            StopCoroutine(chatRoutine);
        chatRoutine = StartCoroutine(ChatViewRoutine());

        if (chat.gameObject.activeSelf)
        {
            chat.ActivateInputField();
            Cursor.visible = chat.gameObject.activeSelf;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = chat.gameObject.activeSelf;
            Cursor.lockState = CursorLockMode.Locked;
        }


    }

    IEnumerator ChatViewRoutine()
    {

        chatPanel.SetActive(true);
        yield return new WaitForSeconds(chatViewTime);
        chatPanel.SetActive(false);
        chat.gameObject.SetActive(false);

    }
    void EntryListInit()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player player = PhotonNetwork.PlayerList[i];
            int teamCode = player.GetPhotonTeam().Code;
            Transform parentTransform = teamCode == 1 ? blueTeamEntryList : redTeamEntryList;

            GameObject ins = Instantiate(inGameEntryPrefab, parentTransform);

            (teamCode == 1 ? bluePlayerList : redPlayerList).Add(player);

            string profileImageName = Manager.Game.UserData.profileImageName;
            if (!string.IsNullOrEmpty(profileImageName))
            {
                Sprite profileImage = Resources.Load<Sprite>($"ProfileImage/{profileImageName}");
                if (profileImage != null)
                {
                    ins.GetComponent<Image>().sprite = profileImage;
                }
                else
                {
                    Debug.LogWarning("Failed to load profile image from resources: " + profileImageName);
                }
            }
            else
            {
                Debug.Log("Image is Null");
            }
        }

    }
    void AllPlayerReadyCheck()
    {

        PhotonNetwork.LocalPlayer.SetProperty(DefinePropertyKey.LOADCOMPLETE, true);
        StartCoroutine(WaitForPlayers());

    }
    IEnumerator WaitForPlayers()
    {
        while (!AllReady(DefinePropertyKey.LOADCOMPLETE))
            yield return null;

        RoundStart();
    }
    [PunRPC]
    void RoundStart()
    {
        Manager.Game.StartGame(blueLocation, redLocation);                                  //1
        Debug.Log("RoundStart");
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("MessageUp", RpcTarget.All, ($"- {curRound}라운드 - \n 무기 사시고 전투를 준비하세요 {countValue}초드림"));
            //   PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.Time);

            PhotonNetwork.CurrentRoom.SetProperty(DefinePropertyKey.SHOPPINGTIME, true);
        }
    }
    public bool AllReady(string key)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null)
                return false;
        return true;

    }
    [PunRPC]
    public void MessageUp(string s)
    {
        Manager.Game.ShowMessage(s);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("OnRoomUpdate");
        InGamePropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged.ContainsKey(DefinePropertyKey.BLUESCORE))
            tapUi.blueScore.text = $"{curRoom.GetProperty<int>(DefinePropertyKey.BLUESCORE)}";
        if (propertiesThatChanged.ContainsKey(DefinePropertyKey.REDSCORE))
            tapUi.redScore.text = $"{curRoom.GetProperty<int>(DefinePropertyKey.REDSCORE)}";


    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
       
        foreach (var key in changedProps.Keys)
        {
            if (key is string scoreType && (scoreType == "Kill" || scoreType == "Death" || scoreType == "Assist"))
            {
                if (changedProps[key] is int scoreValue)
                {
                    tapUi.SetUpKDA(scoreType, targetPlayer, scoreValue);
                }
            }
        }
    }

    public void InGamePropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(DefinePropertyKey.SHOPPINGTIME))                  //2
            if ((bool)propertiesThatChanged[DefinePropertyKey.SHOPPINGTIME])
            {
                Debug.Log("isStart");
                StartCoroutine(ShoppingTime());
                Manager.Game.redTeamSpawner.gameObject.SetActive(true);
                Manager.Game.blueTeamSpawner.gameObject.SetActive(true);
            }

            else
            {
                
                ShopCanvasPrefab.SetActive(false);
                Manager.Game.onShop = false;
                Manager.Game.redTeamSpawner.gameObject.SetActive(false);
                Manager.Game.blueTeamSpawner.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        if ((propertiesThatChanged.ContainsKey(DefinePropertyKey.STARTGAME)))           //5
            if ((bool)propertiesThatChanged[DefinePropertyKey.STARTGAME])
                StartCoroutine(StartGameTime());
    }

        IEnumerator ShoppingTime()                                                          //3
        {

            double loadTime = PhotonNetwork.Time;


            while (PhotonNetwork.Time - loadTime < countValue)
            {
                int remainTime = (int)(countValue - (PhotonNetwork.Time - loadTime));
                inGameTimer.text = (remainTime + 1).ToString();
                yield return null;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                pv.RPC("MessageUp", RpcTarget.All, ($"GAME START "));

                PhotonNetwork.CurrentRoom.SetProperty(DefinePropertyKey.SHOPPINGTIME, false);
                PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.Time);                          //4
                PhotonNetwork.CurrentRoom.SetProperty(DefinePropertyKey.STARTGAME, true);

            }


        }
        public void GameOver()
        {
        Debug.Log("GameOver");
        if (PhotonNetwork.IsMasterClient)
        {
            int remainBlue = 0;
            int remainRed = 0;
            Debug.Log(PhotonNetwork.PlayerList.Length);
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                Debug.Log($"playerCheck : {player.ActorNumber}player ");
                if (1 == player.GetPhotonTeam().Code)
                {
                    if (player.GetProperty<bool>(DefinePropertyKey.DEAD))
                    {
                        Debug.Log($"{player.ActorNumber} player is Dead");
                        continue;
                    }
                        
                    else
                    {
                        Debug.Log($"Blue is {remainBlue}");
                        remainBlue++;
                    }
                }
                else if (2 == player.GetPhotonTeam().Code)
                {
                    if (player.GetProperty<bool>(DefinePropertyKey.DEAD))
                    {
                        Debug.Log($"{player.ActorNumber} player is Dead");
                        continue;
                    }
                        
                    else
                    {
                        Debug.Log($"Red is {remainRed}");
                        remainRed++;
                    }
                }
                else
                    Debug.Log($"player team code is {player.GetPhotonTeam().Code}");


            }

            if (remainBlue > remainRed)
            {
                int blueScore = curRoom.GetProperty<int>(DefinePropertyKey.BLUESCORE);
                curRoom.SetProperty(DefinePropertyKey.BLUESCORE, blueScore + 1);
                pv.RPC("MessageUp", RpcTarget.All, ("블루팀 +1점"));

            }
            else if (remainRed > remainBlue)
            {
                int redScore = curRoom.GetProperty<int>(DefinePropertyKey.REDSCORE);
                curRoom.SetProperty(DefinePropertyKey.REDSCORE, redScore + 1);
                pv.RPC("MessageUp", RpcTarget.All, ("레드팀 +1점"));
            }
            else
            {
                pv.RPC("MessageUp", RpcTarget.All, ("이번 라운드는 무승부입니다"));
            }
        }
        else
        {
            Debug.Log("isnot");
            MessageUp("i'm NOt Master");
        }


        }
        IEnumerator StartGameTime()                                                     //6
        {
            double loadTime = PhotonNetwork.Time;


            while (PhotonNetwork.Time - loadTime < roundTimeValue)
            {
                int remainTime = (int)(roundTimeValue - (PhotonNetwork.Time - loadTime));
                int minutes = Mathf.FloorToInt((remainTime % 3600) / 60);
                int seconds = Mathf.FloorToInt(remainTime % 60);

                inGameTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                yield return null;
            }
        GameOver();
        if (PhotonNetwork.IsMasterClient)                                           //7
            {
                PhotonNetwork.CurrentRoom.SetProperty(DefinePropertyKey.STARTGAME, false);
               
                
                if (curRound < roundCount)
                {

                    pv.RPC("MessageUp", RpcTarget.All, ("라운드 종료"));
                    shopManager.InitList();
                    yield return new WaitForSeconds(3f);
                    pv.RPC("RoundStart", RpcTarget.All);
                    curRound++;
                }
                else
                {
                    PhotonNetwork.CurrentRoom.SetLoadTime(0);
                    pv.RPC("MessageUp", RpcTarget.All, ("모든 라운드 종료"));
                    yield return new WaitForSeconds(1f);
                    pv.RPC("RoundOver", RpcTarget.All);

                }
            }
        }
        [PunRPC]
        void RoundOver()
        {
            int blueTeamScore = curRoom.GetProperty<int>(DefinePropertyKey.BLUESCORE);
            int redTeamScore = curRoom.GetProperty<int>(DefinePropertyKey.REDSCORE);

            if (1 == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            {
                if (blueTeamScore > redTeamScore)
                {
                    MessageUp("승리!");
                    Manager.Game.SetIncreaseDB("win");

                }
                else if (blueTeamScore < redTeamScore)
                {
                    MessageUp("패배!");
                    Manager.Game.SetIncreaseDB("lose");

                }
                else if (blueTeamScore == redTeamScore)
                {
                    MessageUp("무승부!");

                }
            }
            else
            {
                if (blueTeamScore > redTeamScore)
                {
                    MessageUp("패배!");
                    Manager.Game.SetIncreaseDB("lose");
                }
                else if (blueTeamScore < redTeamScore)
                {
                    MessageUp("승리!");
                    Manager.Game.SetIncreaseDB("win");
                }
                else if (blueTeamScore == redTeamScore)
                {
                    MessageUp("무승부!");
                }
            }
            tapUi.RecordKDA();
            Manager.Game.SetUpCount();
            StartCoroutine(GoToLobby());
        }
        IEnumerator GoToLobby()
        {
            yield return new WaitForSeconds(3f);

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel("LobbyScene");
        }

    [PunRPC]
    public void AttackedEffect()
    {
       StartCoroutine(Manager.Scene.AtkedEffect());
    }
}

