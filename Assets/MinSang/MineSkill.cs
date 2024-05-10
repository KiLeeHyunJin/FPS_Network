using UnityEngine;
using Photon.Pun;
using Firebase.Database;
using System.Collections.Generic;

public class MineSkill : MonoBehaviourPun
{
    public GameObject minePrefab;
    public int numberOfMines;
    public int baseDamageAmount = 50;
    public int damageIncrement = 10;

    private DatabaseReference dbReference;

    void Start()
    {
        // Firebase Database 초기화
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void DeployMines(Vector3 position)
    {
        List<Dictionary<string, object>> mineDataList = new List<Dictionary<string, object>>();

        for (int i = 0; i < numberOfMines; i++)
        {
            Vector3 minePosition = position + new Vector3(i, 0, 0); // 위치
            GameObject mine = PhotonNetwork.Instantiate(minePrefab.name, minePosition, Quaternion.identity);
            Mine mineScript = mine.GetComponent<Mine>();

            if (mineScript != null)
            {
                mineScript.damageAmount = baseDamageAmount + (damageIncrement * i);
            }

            // Firebase용 데이터 생성
            Dictionary<string, object> mineData = new Dictionary<string, object>
            {
                { "positionX", minePosition.x },
                { "positionY", minePosition.y },
                { "positionZ", minePosition.z },
                { "damageAmount", mineScript.damageAmount }
            };
            mineDataList.Add(mineData);
        }

        // Firebase에 지뢰 배치 데이터 저장
        string userId = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        dbReference.Child("mineDeployments").Child(userId).SetValueAsync(mineDataList);
    }
}