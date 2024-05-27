using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviourPun
{
    // coin 과 heal 팩 을 관리 할 매니저
    // 라운드 때 재생성 해야하나? 나중에 생각해보기.

    // 생성될 위치 저장 해 둘 트랜스폼 리스트 
    [SerializeField] Transform[] coinPosition;
    [SerializeField] Transform[] healPackPosition;

    [SerializeField] GameObject[] coinArr;
    [SerializeField] GameObject[] healArr;



    Dictionary<GameObject,Transform> dic = new Dictionary<GameObject,Transform>();

    public enum ItemType
    {
        HealPack, Coin
    }

    public ItemType itemType;


    private void Awake()
    {
        Debug.Log("코인 ARR 어웨이크에서 널이니??" + coinArr[0]);
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

        for (int i = 0; i < coinPosition.Length; i++)
        {
            GameObject coinItem = PhotonNetwork.Instantiate("Coin", coinPosition[i].localPosition,
                Quaternion.identity);


        }

        for(int i=0;i<healPackPosition.Length;i++)
        {
            GameObject healItem = (PhotonNetwork.Instantiate("HealPack", healPackPosition[i].localPosition,
                Quaternion.identity));
        }

        
    }
}

//items.transform.parent = coins.transform; --> 자식으로 생성하던 방법에서 수정. 
/*if (healpacks.childCount == 0)*/
