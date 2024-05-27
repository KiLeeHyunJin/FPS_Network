using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviourPun
{
    // coin 과 heal 팩 을 관리 할 매니저
    // 라운드 때 재생성 해야하나? 나중에 생각해보기.

    // 생성될 위치 저장 해 둘 트랜스폼 리스트 
    [SerializeField] Transform[] coinPosition; //아이템 생성 위치 목록 

    [SerializeField] Transform[] healPackPosition;

    private static Dictionary<Vector3,bool> itemStatus
        = new Dictionary<Vector3,bool>();  // 아이템의 위치와 상태를 관리 

    Dictionary<GameObject,Transform> dic = new Dictionary<GameObject,Transform>();

    public enum ItemType
    {
        HealPack, Coin
    }

    public ItemType itemType;

    bool isFisrt;


    private void Awake()
    {
        isFisrt= true;
    }

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

        if(isFisrt==true)
        {
            for (int i = 0; i < coinPosition.Length; i++)
            {
                GameObject coinItem = PhotonNetwork.Instantiate("Coin", coinPosition[i].position,
                    Quaternion.identity);

                itemStatus[coinPosition[i].position] = false; //아이템이 아직 먹히지 않은 상태로 초기화
            }

            for (int i = 0; i < healPackPosition.Length; i++)
            {
                GameObject healItem = (PhotonNetwork.Instantiate("HealPack", healPackPosition[i].position,
                    Quaternion.identity));
                itemStatus[healPackPosition[i].position] = false;
            }

            Debug.Log("첫번째 아이템 생성");
            isFisrt= false;
        }
        else // 두 번째 부터 ( 즉 라운드 )
        {
            Debug.Log("두 번째 ~ 아이템 생성");
            
            for (int i = 0; i < coinPosition.Length; i++)
            {
                if (itemStatus[coinPosition[i].position]) //true 라면 아이템이 먹혔으므로 
                {
                    GameObject coinItem = PhotonNetwork.Instantiate("Coin", coinPosition[i].position,
                    Quaternion.identity);
                    itemStatus[coinPosition[i].position] = false;  
                }

            }

            for (int i = 0; i < healPackPosition.Length; i++)
            {
                GameObject healItem = (PhotonNetwork.Instantiate("HealPack", healPackPosition[i].position,
                    Quaternion.identity));
                itemStatus[healPackPosition[i].position] = false; //다시 안 먹힌 상태로 초기화 

            }

        }
    }

    public static void OnItemCollected(Vector3 itemPosition)
    {
        if(itemStatus.ContainsKey(itemPosition))
        {
            itemStatus[itemPosition] = true; //아이템 들에서 호출 -> 아이템이 먹힘을 표시. 
        }
    }
}
