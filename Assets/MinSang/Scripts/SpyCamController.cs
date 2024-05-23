using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class SpyCamController : MonoBehaviourPun
{
    [SerializeField] public CinemachineVirtualCamera spyCamVirtualCamera;
    [SerializeField] public GameObject spyCamPrefab;
    private bool isSpyCamActive = false;
    private bool isSpyCamPlaced = false;

    private GameObject currentSpyCam;
    private List<GameObject> spyCams = new List<GameObject>();
    private const int maxSpyCams = 3;

    public void Activate()
    {
        if (currentSpyCam != null)
        {
            spyCamVirtualCamera.Priority = 120;
            spyCamVirtualCamera.Follow = currentSpyCam.transform;
            spyCamVirtualCamera.LookAt = currentSpyCam.transform;
            spyCamVirtualCamera.gameObject.SetActive(true);
            isSpyCamActive = true;
        }
    }

    public void Deactivate()
    {
        spyCamVirtualCamera.Priority = -2;
        spyCamVirtualCamera.Follow = null;
        spyCamVirtualCamera.LookAt = null;
        spyCamVirtualCamera.gameObject.SetActive(false);
        isSpyCamActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleSpyCam();
        }

        if (isSpyCamActive && isSpyCamPlaced && currentSpyCam != null)
        {
            RotateSpyCam();
        }
    }

    void ToggleSpyCam()
    {
        if (!isSpyCamActive)
        {
            if (currentSpyCam == null)
            {
                StartCoroutine(PlaceSpyCam());
            }
            else
            {
                Activate();
            }
        }
        else
        {
            Deactivate();
        }
    }

    IEnumerator PlaceSpyCam()
    {
        isSpyCamActive = true;
        while (!isSpyCamPlaced && isSpyCamActive)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (IsPlacementValid(hit.point))
                    {
                        photonView.RPC("RPC_PlacedSpyCam", RpcTarget.AllBuffered, hit.point, hit.normal);
                        isSpyCamActive = false;
                    }
                }
            }
            yield return null;
        }
    }

    [PunRPC]
    void RPC_PlacedSpyCam(Vector3 position, Vector3 normal)
    {
        if (spyCams.Count >= maxSpyCams)
        {
            Destroy(spyCams[0]);
            spyCams.RemoveAt(0);
        }

        currentSpyCam = Instantiate(spyCamPrefab, position, Quaternion.LookRotation(normal));
        spyCams.Add(currentSpyCam);
        spyCamVirtualCamera.Follow = currentSpyCam.transform;
        spyCamVirtualCamera.LookAt = currentSpyCam.transform;
        isSpyCamPlaced = true;

        spyCamVirtualCamera.m_Lens.FieldOfView = 60;
        spyCamVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, -2, 5);
    }

    bool IsPlacementValid(Vector3 position)
    {
        if (Physics.CheckSphere(position, 0.5f))
        {
            return false;
        }

        RaycastHit groundHit;
        if (!Physics.Raycast(position, Vector3.down, out groundHit, 1.0f))
        {
            return false;
        }

        if (groundHit.collider.tag != "Ground")
        {
            return false;
        }

        float maxDistance = 10.0f;
        float minDistance = 1.0f;
        if (Vector3.Distance(transform.position, position) > maxDistance || Vector3.Distance(transform.position, position) < minDistance)
        {
            return false;
        }

        return true;
    }

    void RotateSpyCam()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentSpyCam.transform.Rotate(Vector3.up, -1f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentSpyCam.transform.Rotate(Vector3.up, 1f);
        }
    }
}