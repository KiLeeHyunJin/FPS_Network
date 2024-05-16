using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BombHUD : MonoBehaviour
{
    [SerializeField] //폭탄의 정보를 얻기 위한 스크립트 참조
    private BombController bombController;
    private Bomb currentBomb; //현재 폭탄 상태를 어떻게 체크해줘야될지 생각해보자.

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
        currentBomb=bombController.GetBomb(); //현재 상태의 폭탄.

        //폭탄 갯수 업데이트 ( 폭탄 이름 추가. )
        textBomb.text = $"{currentBomb.bombName} : {currentBomb.currentBombNumber} /{currentBomb.currentBombNumber} ";

        bombImage.sprite = currentBomb.bombSprite; 
    }

    //폭탄 숫자는 폭탄 던질 때 해야지.. 



}
