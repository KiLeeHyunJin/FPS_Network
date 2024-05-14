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
    public Player player {  get; private set; }
    
    public void SetPlayer(Player player)
    {
        this.player = player;
    }
    public void SetLoadingProgress(float progress)
    {
        loadSlider.value = progress;
        loadPercentage.text = $"{(int)progress*100}%";
    }
    

    
}
