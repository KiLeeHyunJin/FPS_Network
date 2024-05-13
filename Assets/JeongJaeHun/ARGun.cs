using UnityEngine;

public class ARGun : MonoBehaviour, IInteractable
{
    //AR건 등 각각 이름이 붙은 스크립트에서는 실제적인 데미지나 발사 등의 직접적인 
    // 코드들을 구현하고 아이템을 줍고 하는 등의 행위는
    // GUN BASE에서 진행하자. 

    public GunData gunData;
    public GunBase gunBase;

    public void Interaction(GameObject player)
    {
        // 이 부분 생각해보자. 
        //줍기 구현해주면 된다. 
        gunBase.PickUp(gameObject); //자신이 들어가야 함. 
    }
}
