using Firebase.Extensions;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Firebase.Database;
public class MenuPanel : MonoBehaviour
{
    [SerializeField] GameObject PlayButton;
    [SerializeField] GameObject PlayButtons;
    [SerializeField] GameObject userInfoWindow;
    [SerializeField] Button userInfoButton;
    [SerializeField] Button openButton;
    [SerializeField] Button closeButton;
    [SerializeField] Button logoutButton;
    [SerializeField] Button lobbyButton;
    [SerializeField] Button randomButton;

    LobbyData data;

    private void Awake()
    {
        userInfoButton.onClick.AddListener(() => { userInfoWindow.SetActive(true); });
        openButton.onClick.AddListener(()=> { OpenPlayButtons(true); });
        closeButton.onClick.AddListener(()=> { OpenPlayButtons(false); });
        logoutButton.onClick.AddListener(Logout);
        lobbyButton.onClick.AddListener(JoinLobby);
        randomButton.onClick.AddListener(RandomMatching);
    }
    private void Start()
    {
        data = FindObjectOfType<LobbyData>();
        PhotonNetwork.EnableCloseConnection = true;
        UserData userData = new UserData("sad");
        FireBaseManager.DB
            .GetReference("UserData")
            .Child("id")
            .SetRawJsonValueAsync(JsonUtility.ToJson(userData));
        //FireBaseManager.DB
        //  .GetReference("UserData")
        //  .Child("LHJ")
        //  .GetValueAsync()
        //  .ContinueWithOnMainThread(task =>
        //  {
        //      if (task.IsCanceled)
        //      {
        //          return;
        //      }
        //      if (task.IsFaulted)
        //      {
        //          return;
        //      }
        //      if (task.IsCompleted)
        //      {
        //          DataSnapshot snapShot = task.Result;
        //          if (snapShot.Exists)
        //          {
        //              string json = snapShot.GetRawJsonValue();
        //              UserData userData = JsonUtility.FromJson<UserData>(json);
        //          }
        //          else
        //          {
        //              UserData userData = new UserData();
        //          }
        //      }
        //  });



    }
    private void OnEnable()
    {
        OpenPlayButtons(false);
    }

    public void RandomMatching()
    {
        data.SetLobbyState(LobbyData.LobbyState.Random,true);
        JoinLobby();
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    void Logout()
    {
        FireBaseManager.Auth.SignOut();
        PhotonNetwork.Disconnect();
    }

    public void OpenPlayButtons(bool state)
    {
        PlayButton.SetActive(!state);
        PlayButtons.SetActive(state);
    }
}
