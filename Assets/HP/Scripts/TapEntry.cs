using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class TapEntry : MonoBehaviour
{
   [SerializeField]Image entryProfileImage;
   [SerializeField]TMP_Text playerName;
   [SerializeField]TMP_Text kill;
   [SerializeField]TMP_Text death;
   [SerializeField]TMP_Text assist;

    public Player player {  get; private set; }
    public void Init(Player player)
    {
        this.player = player;
        playerName.text = player.NickName;
        SetProfileImage(player);
        SetKill(0);
        SetDeath(0);
        SetAssist(0);
    }
    void SetProfileImage(Player player)
    {
        Manager.Game.LoadProfileImage(entryProfileImage, player);
    }
    public void SetKill(int killCount)
    {
        kill.text = killCount.ToString();
    }
    public void SetDeath(int deathCount)
    {
            death.text = deathCount.ToString();
    }
    public void SetAssist(int assistCount)
    {
            assist.text = assistCount.ToString();
    }
 
    public int GetKill()
    {
        return player.GetProperty<int>(DefinePropertyKey.KILL);
    } 

    public int GetDeath()
    {
        return player.GetProperty<int>(DefinePropertyKey.DEATH);
    }
    public int GetAssist()
    {
        return player.GetProperty<int>(DefinePropertyKey.ASSIST);
    }
    public void IncreaseKill()
    {
        player.SetProperty(DefinePropertyKey.KILL, GetKill() + 1);
        
        Debug.Log($"Increased Kill {GetKill()}");
    }
    public void IncreaseDeath()
    {
        player.SetProperty(DefinePropertyKey.DEATH, GetDeath() + 1);
        Debug.Log($"Increased death {GetDeath()}");
    }
    public void IncreaseAssist()
    {
        player.SetProperty(DefinePropertyKey.ASSIST, GetAssist() + 1);
        Debug.Log($"Increased assist {GetAssist()}");
    }
}
