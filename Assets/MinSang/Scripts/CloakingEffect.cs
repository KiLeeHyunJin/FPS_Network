using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakingEffect : MonoBehaviourPun, IPunObservable, ISkill
{
    public bool isCloaked = false;
    public Material CloakingMaterial;
    public float CloakDuration = 5f;
    public float CloakTransparency = 0.2f;
    public KeyCode CloakKey = KeyCode.Q;
    private Color originalColor;
    private Renderer renderer;
    
    public void Activate()
    {
        StartCoroutine(CloakRoutine());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        renderer.material = CloakingMaterial;
    }

    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (CloakingMaterial != null)
        {
            originalColor = CloakingMaterial.color;
        }
        else
        {
            Debug.LogError("CloakingMaterial has not been assigned!");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isCloaked);
        }
        else
        {
            isCloaked = (bool)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(CloakKey) && photonView.IsMine)  // "Q" 키를 누를 경우
        {
            Debug.Log("클로킹");
            ToggleCloak();
        }
    }

    void ToggleCloak()
    {
        isCloaked = !isCloaked; // 클로킹 상태 토글
        if (isCloaked)
        {
            StartCoroutine(CloakRoutine());
        }
        else
        {
            CloakingMaterial.color = originalColor; // 클로킹 해제
            Debug.Log("클로킹 해제");
        }
    }

    IEnumerator CloakRoutine()
    {
        // 투명도 적용
        Color cloakColor = originalColor;
        cloakColor.a = CloakTransparency;
        CloakingMaterial.color = cloakColor;

        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(CloakDuration);

        // 클로킹 비활성화
        isCloaked = false;
        CloakingMaterial.color = originalColor;
    }
}