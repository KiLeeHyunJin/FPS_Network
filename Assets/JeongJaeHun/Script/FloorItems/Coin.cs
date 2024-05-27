using Photon.Pun;
using UnityEngine;

public class Coin : MonoBehaviourPun, IInteractable
{

    public enum CoinType
    {
        Coin1 = 50,
        Coin2 = 100,
        Coin3 = 150,
        Coin4 = 200
    }

    [SerializeField] int coinValue;

    private void Start()
    {
        coinValue = (int)(GetRandomEnumValue());

    }

    public CoinType GetRandomEnumValue()
    {
        var enumValues = System.Enum.GetValues(enumType: typeof(CoinType));
        return (CoinType)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));

    }

    public void Interaction(GameObject player)
    {

        InventoryController inventory = player.GetComponentInParent<InventoryController>();

        if (photonView == null) return;

        if (inventory != null)
        {
            inventory.GetCoin(coinValue);

            photonView.RPC("DestroyItem", RpcTarget.MasterClient);
        }

    }

    [PunRPC]
    private void DestroyItem()
    {
        PhotonNetwork.Destroy(gameObject);
        Debug.Log("마스터가 아이템 삭제 함.");
    }
}
