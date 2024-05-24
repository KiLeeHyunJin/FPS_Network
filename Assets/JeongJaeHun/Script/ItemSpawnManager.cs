using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    // coin 과 heal 팩 을 관리 할 매니저
    // 라운드 때 재생성 해야하나? 나중에 생각해보기.

    // 생성될 위치 저장 해 둘 트랜스폼 리스트 
    [SerializeField] List<Transform> coinPosition;
    [SerializeField] List<Transform> healPackPosition;


    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)  //호스트에서만 아이템 생성 가능 
        {
            if(coinPosition.Count > 0 && healPackPosition.Count>0)
            {
                ItemSpawn();
            }
            
        }       
    }


    public void ItemSpawn()
    {
        foreach (var coins in coinPosition)
        {
            PhotonNetwork.Instantiate("Coin",coins.position,Quaternion.identity);
        }

        foreach (var healpacks in healPackPosition)
        {
            PhotonNetwork.Instantiate("HealPack",healpacks.position,Quaternion.identity);
        }
    }
}
