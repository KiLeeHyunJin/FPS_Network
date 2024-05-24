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

    int coinValue;

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
        InventoryController inventory = player.GetComponent<InventoryController>();

        if(photonView.IsMine)
        {
            inventory.GetCoin(coinValue);
            PhotonNetwork.Destroy(gameObject);
        }

    }
}
