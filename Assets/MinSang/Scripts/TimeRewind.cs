using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using System.Collections.Generic;

public class TimeRewind : MonoBehaviourPun, IDamagable
{
    public Controller Controller;
    public float rewindDuration = 3.0f;
    public float positionRecordInterval = 0.1f;
    public KeyCode rewindKey = KeyCode.Z;
    [SerializeField] public int maxHealth = 100;

    private Queue<Vector3> positionHistory;
    private Queue<int> healthHistory;
    private int currentHealth;
    private Coroutine rewindCoroutine;
    private Coroutine recordCoroutine;

    void Start()
    {
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

            if (Input.GetKeyDown(rewindKey))
            {
                Debug.Log("시간 역행");
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

        while (time < rewindDuration && historyCount > 0)
        {
            int rewindIndex = Mathf.Clamp(historyCount - 1 - Mathf.FloorToInt(time / positionRecordInterval), 0, historyCount - 1);

            transform.position = positionHistoryArray[rewindIndex];
            currentHealth = healthHistoryArray[rewindIndex];
            time += Time.deltaTime;
            yield return null;
        }

        Controller.enabled = true;
        rewindCoroutine = null;
        recordCoroutine = StartCoroutine(RecordPositionAndHealth());
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
}