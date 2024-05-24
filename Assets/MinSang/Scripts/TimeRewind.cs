using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine.UI;

public class TimeRewind : MonoBehaviourPun
{
    public Controller Controller;
    public float rewindDuration = 3.0f;
    public float positionRecordInterval = 0.016f;
    public KeyCode skillKey;
    [SerializeField] public int maxHealth = 100;

    private Queue<Vector3> positionHistory;
    private Queue<int> healthHistory;
    private int currentHealth;
    private Coroutine rewindCoroutine;
    private Coroutine recordCoroutine;
    public PhotonView pv;

    public SkillEntry thisEntry;
    public Image skillEntryImg;

    [SerializeField] ParticleSystem rewindEff;
    [SerializeField] ParticleSystem outRewindEff;
    [SerializeField] SkinnedMeshRenderer[] renderers;
    [SerializeField] MeshRenderer[] mrenders;

    void Start()
    {
        pv = GetComponent<PhotonView>();    
        Controller = GetComponent<Controller>();
        currentHealth = maxHealth;
        positionHistory = new Queue<Vector3>();
        healthHistory = new Queue<int>();
    }

    void Update()
    {
        if (!IsRewinding())
        {
            if (recordCoroutine == null)
                recordCoroutine = StartCoroutine(RecordPositionAndHealth());

            if (Input.GetKeyDown(skillKey))
            {
                Debug.LogError("시간 역행");
                Manager.Scene.RewindOut();
                
                rewindCoroutine = StartCoroutine(RewindCoroutine());
                
            }
        }
    }

    IEnumerator RecordPositionAndHealth()
    {
       
        while (true)
        {
            if (positionHistory.Count >= Mathf.CeilToInt(rewindDuration / positionRecordInterval))
            {
                positionHistory.Dequeue();
                healthHistory.Dequeue();
            }

            positionHistory.Enqueue(transform.position);
            healthHistory.Enqueue(currentHealth);

            yield return new WaitForSeconds(positionRecordInterval);
        }
    }

    IEnumerator RewindCoroutine()
    {
        Debug.LogError("InRewind Coroutine");
        if (recordCoroutine != null)
        {
            StopCoroutine(recordCoroutine);
            recordCoroutine = null;
        }

        Controller.enabled = false;
        float time = 0;
        var positionHistoryArray = positionHistory.ToArray();
        var healthHistoryArray = healthHistory.ToArray();
        int historyCount = positionHistoryArray.Length;
        Debug.LogError($"PosHistory count is {positionHistory.Count}");
        pv.RPC("RewindEffectOn",RpcTarget.Others);

        while (time < rewindDuration && historyCount > 0)
        {
            int rewindIndex = Mathf.Clamp(historyCount - 1 - Mathf.FloorToInt(time / positionRecordInterval), 0, historyCount - 1);

            transform.position = positionHistoryArray[rewindIndex];
            currentHealth = healthHistoryArray[rewindIndex];
            time += Time.deltaTime;
            yield return null;
        }
        Manager.Scene.RewindIn();
        pv.RPC("RewindEffectOff", RpcTarget.Others);
        Controller.enabled = true;
        rewindCoroutine = null;
        recordCoroutine = StartCoroutine(RecordPositionAndHealth());
        GetComponent<TimeRewind>().enabled = false;
        skillEntryImg.sprite = null;
        skillEntryImg.gameObject.SetActive(false);
        skillEntryImg = null;
        thisEntry.isIt = false;
        thisEntry = null;


    }

    public void TakeDamage(int damage)
    {
        if (!IsRewinding())
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Debug.Log("캐릭터가 사망했습니다.");
            }
        }
    }

    private bool IsRewinding()
    {
        return rewindCoroutine != null;
    }
    [PunRPC]
    public void RewindEffectOn()
    {
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        mrenders = GetComponentsInChildren<MeshRenderer>();
        Debug.Log("other Rewind");
        Instantiate(rewindEff, transform.position, Quaternion.identity);
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }
        foreach (MeshRenderer renderer in mrenders)
        {
            renderer.enabled = false;
        }
    }
    [PunRPC]
    public void RewindEffectOff()
    {
        mrenders = GetComponentsInChildren<MeshRenderer>();
        Instantiate(outRewindEff, transform.position, Quaternion.identity);
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.enabled = true;
        }
        foreach (MeshRenderer renderer in mrenders)
        {
            renderer.enabled = true;
        }

    }
}