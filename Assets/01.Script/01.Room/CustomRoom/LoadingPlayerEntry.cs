using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPlayerEntry : MonoBehaviour
{
    [SerializeField] public Slider loadSlider;
    [SerializeField] public TMP_Text loadPercentage;
    [SerializeField] public TMP_Text player_id;
    [SerializeField] public Image profileImg;
    public Player player {  get; private set; }
    
    public void SetPlayer(Player player)
    {
        Manager.Game.LoadProfileImage(profileImg, player);
        this.player = player;
        player_id.text = player.NickName;
        
    }
    public void SetLoadingProgress(float progress)
    {
        loadSlider.value = progress;
        loadPercentage.text = $"{(int)progress*100}%";
    }
    

    
}
