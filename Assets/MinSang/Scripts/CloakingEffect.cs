using ExitGames.Client.Photon.StructWrapping;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakingEffect : MonoBehaviourPun, IPunObservable
{
    public bool isCloaked = false;
    public Material CloakingMaterial;
    public float CloakDuration = 3f;
    public float CloakTransparency = 1f;
    public KeyCode CloakKey = KeyCode.Q;
    private Renderer[] renderers;

    private List<Material> originalMaterials = new List<Material>();
    private List<Color> originalColors = new List<Color>();

    private DatabaseReference databaseReference;
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
        }

        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
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
        if (isCloaked)
        {
            Deactivate();
        }
        else
        {
            Activate();
            UpdateFirebaseCloakStatus(true);
        }
    }
    [PunRPC]
    IEnumerator CloakRoutine()
    {
        isCloaked = true;

        for (int i = 0; i < renderers.Length; i++)
        {
            // 투명도 적용
            if (i < originalColors.Count)
            {
                Color cloakColor = originalColors[i];
                cloakColor.a = CloakTransparency;
                renderers[i].material = CloakingMaterial;
                CloakingMaterial.color = cloakColor;
            }
        }

        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(CloakDuration);

        // 클로킹 비활성화
        Deactivate();
    }

    void UpdateFirebaseCloakStatus(bool status)
    {
        if (databaseReference != null)
        {
            string userId = PhotonNetwork.LocalPlayer.UserId;
            databaseReference.Child("users").Child(userId).Child("isCloaked").SetValueAsync(status);
        }
    }
}