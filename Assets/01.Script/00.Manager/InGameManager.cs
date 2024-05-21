using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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


    void Start()
    {
        bluePlayerList = new List<Player>();
        redPlayerList = new List<Player>();
        EntryListInit();
        Manager.Scene.StartFadeIn();

        MessageUp("곧 게임이 시작됩니다");


        if (PhotonNetwork.InRoom == false)
            return;

        AllPlayerReadyCheck();
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

        
        Manager.Game.StartGame(blueLocation, redLocation);

        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("MessageUp", RpcTarget.All, ("준비완료 \n 무기 사시고 전투를 준비하세요 20초드림"));
            PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.Time);
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
    }


    public void InGamePropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(DefinePropertyKey.SHOPPINGTIME))
            if ((bool)propertiesThatChanged[DefinePropertyKey.SHOPPINGTIME])
            {
                Debug.Log("isStart");
                StartCoroutine(ShoppingTime());
                Manager.Game.redTeamSpawner.gameObject.SetActive(true);
                Manager.Game.blueTeamSpawner.gameObject.SetActive(true);
            }

            else
            {
                Manager.Game.redTeamSpawner.gameObject.SetActive(false);
                Manager.Game.blueTeamSpawner.gameObject.SetActive(false);
            }

        if ((propertiesThatChanged.ContainsKey(DefinePropertyKey.STARTGAME)))
            StartCoroutine(StartGameTime());


    }

    IEnumerator ShoppingTime()
    {
        double loadTime = PhotonNetwork.CurrentRoom.GetLoadTime();
        while (PhotonNetwork.Time - loadTime < countValue)
        {
            int remainTime = (int)(countValue - (PhotonNetwork.Time - loadTime));
            inGameTimer.text = (remainTime + 1).ToString();
            yield return null;
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("MessageUp", RpcTarget.All, ($"GAME START  {pv.Controller.ActorNumber}"));

            PhotonNetwork.CurrentRoom.SetProperty(DefinePropertyKey.SHOPPINGTIME, false);
            PhotonNetwork.CurrentRoom.SetProperty(DefinePropertyKey.STARTGAME, true);
            PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.Time);
        }
            

    }

    IEnumerator StartGameTime()
    {
        double loadTime = PhotonNetwork.CurrentRoom.GetLoadTime();

        while (PhotonNetwork.Time - loadTime < roundTimeValue)
        {
            int remainTime = (int)(roundTimeValue - (PhotonNetwork.Time - loadTime));
            inGameTimer.text = (remainTime + 1).ToString();
            yield return null;
        }
        if (PhotonNetwork.IsMasterClient)
            pv.RPC("MessageUp", RpcTarget.All, ("TIME OVER"));
    }
}
