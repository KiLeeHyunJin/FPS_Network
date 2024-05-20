using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakingEffect : MonoBehaviourPun, ISkill
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
        SetCloakTransparency(1f);  // 복구 시 투명도를 원래 상태로 설정
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

    private IEnumerator CloakRoutine()
    {
        SetCloakTransparency(CloakTransparency);  // 투명화 시작 시 투명도를 설정
        yield return new WaitForSeconds(CloakDuration);
        Deactivate();  // 일정 시간 후 비활성화
    }

    private void SetCloakTransparency(float transparency)
    {
        if (CloakingMaterial != null)
        {
            CloakingMaterial.SetFloat("_Transparency", transparency);  // 셰이더 속성 업데이트
        }
    }
}