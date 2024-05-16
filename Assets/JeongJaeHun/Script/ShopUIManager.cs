using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public PopUpUI purChasePanelPrefab;
    public GameObject shopCanvas;

    public Inventory inventory;

    Button[] shopBtn;
    


    //버튼 클릭 시 나오는 -> 구매 yes / no 팝업은 --> 팝업 ui 매니저 이용하기. 
    //no하면 그냥 팝업 꺼주면 되는데 yes 하면 여러 작업을 진행해야함. 

    //시작시에 canvas 꺼버려야함. 
    private void Start()
    {
        shopCanvas.gameObject.SetActive(false);

        shopBtn=shopCanvas.GetComponentsInChildren<Button>(true); //button 컴포넌트와 매치되는
        // 모든 자식 오브젝트들을 찾아옴 ( button이 안 달려있으면 안가져옴. )

        for(int i=0;i<shopBtn.Length;i++)
        {
            shopBtn[i].onClick.AddListener(onItemPanelClick);
        }
        

    }

    public void onItemPanelClick() //Item 버튼에 부여할 이벤트 
    {
        Manager.UI.ShowPopUpUI(purChasePanelPrefab);
    }


    // input을 이용한 상점 창 열기 -->임시임. 
    public void OnX(InputValue value)
    {
        if (value.isPressed)
        {
            shopCanvas.SetActive(!shopCanvas.activeSelf);
        }

    }



    


}
