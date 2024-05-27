using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    // coin 과 heal 팩 을 관리 할 매니저
    // 라운드 때 재생성 해야하나? 나중에 생각해보기.

    // 생성될 위치 저장 해 둘 트랜스폼 리스트 
    [SerializeField] List<Transform> coinPosition;
    [SerializeField] List<Transform> healPackPosition;

    [SerializeField] GameObject coin;
    [SerializeField] GameObject heal;



    public enum ItemType
    {
        HealPack, Coin
    }

    public ItemType itemType;

    [SerializeField] List<(GameObject, Vector3)> itemsList = new List<(GameObject, Vector3)>();

    // 여기서 재생성 시에 만약 List가 False 라면 재생성 시에 생성해주기. 

    // 아이템이 삭제되면 transform 위치가 missing이 된다. -->이거이용 

    private void Start()
    {
        ItemSpawn();
    }
   
    public void ItemSpawn()
    {
        
        if (!PhotonNetwork.IsMasterClient)  //호스트에서만 아이템 생성 가능 
        {
            return;
        }

        if (coinPosition.Count <= 0 && healPackPosition.Count <= 0)
        {
            return;
        }

        Debug.Log("아이템 스폰 함수 진입");
        foreach (var coins in coinPosition)
        {
            if (coins.childCount == 0)
            {
                GameObject items = PhotonNetwork.Instantiate("Coin", coins.position, Quaternion.identity);

                items.transform.parent = coins.transform;
            }

            foreach (var healpacks in healPackPosition)
            {
                if (healpacks.childCount == 0)
                {
                    GameObject heals = (PhotonNetwork.Instantiate("HealPack", healpacks.position, Quaternion.identity));
                    heals.transform.parent = healpacks.transform;

                }

            }
        }
    }
}
