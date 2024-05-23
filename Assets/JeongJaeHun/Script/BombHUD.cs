using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BombHUD : MonoBehaviourPun
{
    [SerializeField] //폭탄의 정보를 얻기 위한 스크립트 참조
    private BombController[] bombController;
    public Bomb currentBomb; //현재 폭탄 상태를 어떻게 체크해줘야될지 생각해보자.

    [SerializeField]
    private GameObject gO_BombHUD; // 넌 뭐니 ?

    [SerializeField]
    private Image bombImage; //각 폭탄에 따라 (현재 active 된 holder) 스프라이트가 변해야 하는데.. 가능한가?

    //폭탄 갯수를 텍스트에 ui에 반영 
    public TextMeshProUGUI textBomb;

    private void Update()
    {


        CheckUi(); 
    }

    private void CheckUi()
    {

        /*for(int i=0;i<bombController.Length;i++)  // 붐컨이 2개니까.. 하나가 꺼져도 계속 getbomb이 됨. 
        {
            if (bombController[i] !=null)
            {
                if (bombController[i].gameObject.activeSelf) 
                {                   
                    currentBomb= bombController[i].GetBomb(); //현재 상태의 폭탄을 가져옴. 
                    break; //여기서 브레이크 띄워주기. --> 하나만 가져오도록. 
                }
                

            }
        }*/

        
        //폭탄 갯수 업데이트 ( 폭탄 이름 추가. )
        if(currentBomb!=null)
        {
            textBomb.text = $"{currentBomb.bombName} \n {currentBomb.currentBombNumber} /{currentBomb.fullBombNumber} ";

            bombImage.sprite = currentBomb.bombSprite;
        }
        
    }

    public void SetCurrentBomb(Bomb _bomb)
    {
        currentBomb= _bomb;
    }



}
