using Photon.Pun;
using UnityEngine;

public class StealthPlayer : MonoBehaviourPunCallbacks
{
    private bool isStealth = false;
    private Renderer playerRenderer;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && photonView.IsMine) // L로 클로킹 활성화
        {
            ToggleStealth();
        }
    }

    [PunRPC]
    void UpdateStealthState(bool newState)
    {
        isStealth = newState;
        Color color = playerRenderer.material.color;
        color.a = isStealth ? 0.2f : 1.0f;
        playerRenderer.material.color = color;
    }

    void ToggleStealth()
    {
        isStealth = !isStealth;
        photonView.RPC("UpdateStealthState", RpcTarget.All, isStealth);
    }
}