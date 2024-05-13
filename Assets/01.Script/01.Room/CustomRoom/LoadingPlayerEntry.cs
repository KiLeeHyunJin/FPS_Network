using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPlayerEntry : MonoBehaviour
{
    [SerializeField] public Slider loadSlider;
    public Player player {  get; private set; }
    
    public void SetPlayer(Player player)
    {
        this.player = player;
    }
    public void SetLoadingProgress(float progress)
    {
        loadSlider.value = progress;
    }

    
}
