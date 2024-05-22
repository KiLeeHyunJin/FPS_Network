using Firebase.Database;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapUI : MonoBehaviour
{
    [SerializeField] Transform redTeamList;
    [SerializeField] Transform blueTeamList;

    [SerializeField] TapEntry tapInfoEntryPrefab;

    [SerializeField] TapEntry m_Entry;

    [SerializeField] List<TapEntry> entryList;
    public void SetTapList()
    {
        for(int i = 0;i< PhotonNetwork.PlayerList.Length; i++)
        {
            var player = PhotonNetwork.PlayerList[i];
            
            Transform parentTransform = player.GetPhotonTeam().Code == 1 ? blueTeamList : redTeamList;

            TapEntry tapIns =  Instantiate(tapInfoEntryPrefab, parentTransform);
            entryList.Add(tapIns);
            tapIns.Init(player);
            if(player == PhotonNetwork.LocalPlayer)
                m_Entry = tapIns;
        }
    }
    public void SetUpScore( string scoreType,Player player,int value)
    {
        foreach(TapEntry entry in entryList)
        {
            if (entry.player == player)
            {
                switch (scoreType)
                {
                    case "Kill":
                        entry.SetKill(value);
                        
                        break;
                    case "Death":
                        entry.SetDeath(value);
                        break;
                    case "Assist":
                        entry.SetAssist(value);
                        break;
                }
               
            }
                
        }

    }
    public void RecordKDA()
    {
        DatabaseReference m_DB = FireBaseManager.DB
              .GetReference("UserData")
              .Child(FireBaseManager.Auth.CurrentUser.UserId);



        m_DB.Child("KillCount")
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve kill count: " + task.Exception);
                    return;
                }

                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    int previousKill = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;


                    m_DB.Child("KillCount").SetValueAsync(previousKill + m_Entry.GetKill());
                }
            });
        m_DB.Child("DeathCount")
    .GetValueAsync().ContinueWith(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Failed to retrieve Death count: " + task.Exception);
            return;
        }

        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            int previousKill = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;


            m_DB.Child("DeathCount").SetValueAsync(previousKill + m_Entry.GetDeath());
        }
    });
        m_DB.Child("AssistCount")
    .GetValueAsync().ContinueWith(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Failed to retrieve Assist count: " + task.Exception);
            return;
        }

        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            int previousKill = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;


            m_DB.Child("AssistCount").SetValueAsync(previousKill + m_Entry.GetAssist());
        }
    });
    }
    public void test()
    {
        int r = Random.Range(0, 3);
        switch (r)
        {
            case 0: m_Entry.IncreaseKill();
                break;
            case 1: m_Entry.IncreaseDeath();
                break;
            case 2: m_Entry.IncreaseAssist();
                break;
        }
        
       
    }
}
