using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopUIManager : MonoBehaviour
{
    public LayerMask startPosLayer;

    public GameObject shopPanel;
    public TextMeshProUGUI notPurchaseItemText;
    public TextMeshProUGUI goldText;

    public bool shopFlag;
    public bool startPos;

    private Coroutine notPurchaseText;

    public GoldManager goldManager;

    void Start()
    {
        shopPanel.SetActive(shopFlag); //게임 시작 시에는 상점 패널이 꺼져 있어야한다. 
        goldText.text = $"{goldManager.Gold}"; // start 시점에서 GoldManager의 Gold에 접근한다. (0으로 초기화되어있음)
    }

    private void OnTriggerStay(Collider other) //시작 지점에 있는 동안만 상점창을 열 수 있도록한다. 
    {
        if (((1 << other.gameObject.layer) & startPosLayer) != 0)
        {
            startPos = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & startPosLayer) != 0)
        {
            startPos = false;
        }

    }

    //하나의 키를 쓰니까 이 부분을 그냥 분리를 하는게 맞는것 같다. 
    public void OnShop(InputValue value)
    {
        if (value.isPressed) //실제로는  startpos 가 true 인 상황에서만 열리도록 해주면된다. 
        {
            shopFlag = !shopFlag;
            shopPanel.SetActive(shopFlag);
        }

    }


    public void OnNotPurchaseText()
    {
        notPurchaseText=StartCoroutine(notPurchaseTextOnRoutine());
    }

    IEnumerator notPurchaseTextOnRoutine()
    // 어차피 물품 구매 불가 text는 상점창이 떠 있는 상황에서만 뜰 수 있어야 한다. 
    // 
    {
        if(notPurchaseText!=null) //코루틴이 실행중이라면 중지시키고 실행해야 한다. 
        {
            StopCoroutine(notPurchaseText);
        }

        notPurchaseItemText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        notPurchaseItemText.gameObject.SetActive(false);

    }

}
