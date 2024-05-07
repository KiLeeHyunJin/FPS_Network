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

    public int Team { get { return team; } }
    public bool ReadyState { get; private set; }
    public Player Player { get { return player; } }
    Player player;
    RoomPanel owner;
    PlayerProperty property;
    Action<PlayerEntry, int> changeTeamMethod;
    bool isMine;
    public void SetPlayer(Player player, RoomPanel owner, PlayerProperty buttons, Action<PlayerEntry, int> _changeTeam , int teamType = 0)
    {
        this.owner = owner;
        this.player = player;
        changeTeamMethod = _changeTeam;
        playerName.text = player.NickName;
        isMine = player.IsLocal;
        buttonName.text = isMine ? "준비" : "속성";
        ChangeReady(player.CustomProperties);
        ReadyState = player.GetProperty<bool>(DefinePropertyKey.READY);
        team = teamType;
        property = buttons;
        if (teamType != 0)
            player.JoinTeam((byte)teamType);
    }
    public void Ready()
    {
        if (isMine)
        {
            bool ready = player.GetProperty<bool>(DefinePropertyKey.READY);
            ready = !ready;
            player.SetProperty(DefinePropertyKey.READY, ready);
            PhotonNetwork.AutomaticallySyncScene = ready;
        }
        else
        {
            property.gameObject.SetActive(true);
            property.SetPlayer(player);
            RectTransform rect = property.transform as RectTransform;
            if(rect != null)
                rect.position = Input.mousePosition;
        }
    }

    public void UpdateProperty(PhotonHashtable property)
    {
        if (player.GetPhotonTeam() != null)
            ChangeTeam(player.GetPhotonTeam().Code);
        ChangeReady(property);
    }
    void ChangeReady(PhotonHashtable property)
    {
        bool ready = property.ContainsKey(DefinePropertyKey.READY) ?
            (bool)property[DefinePropertyKey.READY] : player.GetProperty<bool>(DefinePropertyKey.READY);
        playerReady.text = ready ? "Ready" : "";
        ReadyState = ready;
    }
    void ChangeTeam(int changeTeamValue)
    {
        changeTeamMethod.Invoke(this, changeTeamValue);
        //owner.ChangeTeam(this, changeTeam);
        if(team != changeTeamValue)
            team = changeTeamValue;
    }
}
