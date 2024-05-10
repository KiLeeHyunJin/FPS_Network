using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    
    // 여기서 골드 관리 및 상점 연계 (골드쓰니까)
    public int Gold { get; set; }
    public TextMeshProUGUI goldText;

    private void Start()
    {
        goldText.text = $"{0}"; //시작 시에 0원으로 초기화 
    }


    public void GetCoin(int coin)
    {
        Gold += coin; //골드 추가. 
        goldText.text = $"{Gold}";

    }
    
    public void LoseCoin(int coin) //상점 아이템 구매 등
    {
        Gold -= coin;
        if (Gold < 0) Gold = 0; //최소값 0으로 제한 
        goldText.text = $"{Gold}"; //골드텍스트 초기화 
    }

    //상점창이 꺼져있을 때도 골드 증가가 제대로 되는지 확인 (임시 ) 
    

}
