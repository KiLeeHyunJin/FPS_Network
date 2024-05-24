using ExitGames.Client.Photon.StructWrapping;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloakingEffect : Skill, IPunObservable
{
    public bool isCloaked = false;
    public Material CloakingMaterial;
    public float CloakDuration = 3f;
    public float CloakTransparency = 1f;
    public KeyCode skillKey;
    public Renderer[] renderers;

    [SerializeField] List<Material> originalMaterials = new List<Material>();
    [SerializeField] List<Color> originalColors = new List<Color>();


    public SkillEntry thisEntry;
    public Image skillEntryImg;

    public override void SkillOn()
    {
        Debug.Log(SkillName + "SkillOn");
    }

    public override void SkillOff()
    {
        Debug.Log(SkillName + "SkillOff");
    }
    public void Activate()
    {
        StartCoroutine(CloakRoutine());
    }

   
    public void Deactivate()
    {
        if (isCloaked)
        {
            StopAllCoroutines();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (i < originalMaterials.Count && i < originalColors.Count)
                {
                    renderers[i].material = originalMaterials[i];
                    originalMaterials[i].color = originalColors[i];
                }
            }
            isCloaked = false;
            Debug.Log("클로킹 해제");
            GetComponent<CloakingEffect>().enabled = false;
            skillEntryImg.sprite = null;
            skillEntryImg.gameObject.SetActive(false);
            skillEntryImg = null;
            thisEntry.isIt = false;
            thisEntry = null;
        }
    }

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.Log("Renderer가 할당되지 않았습니다! 이 스크립트는 Renderer 컴포넌트가 필요합니다.");
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            originalMaterials.Add(renderer.material);
            if (renderer.material.HasProperty("_Color"))
            {
                originalColors.Add(renderer.material.GetColor("_Color"));
            }
            else
                originalColors.Add(Color.white);
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
        if (Input.GetKeyDown(skillKey)) 
        {
            Debug.Log("Cloacking");
            ToggleCloak();
        }
    }

    void ToggleCloak()
    {
        if (isCloaked)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }
    [PunRPC]
    IEnumerator CloakRoutine()
    {
        isCloaked = true;

        for (int i = 0; i < renderers.Length; i++)
        {
           Color cloakColor = originalColors[i];
           cloakColor.a = CloakTransparency;
           renderers[i].material = CloakingMaterial;
           CloakingMaterial.color = cloakColor;
        
        }

        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(CloakDuration);

        // 클로킹 비활성화
        Deactivate();
    }
}