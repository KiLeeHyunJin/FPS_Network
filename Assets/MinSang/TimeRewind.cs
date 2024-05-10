using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
public class TimeRewind : MonoBehaviourPun, IDamagable
{
    public Controller Controller;
    public float rewindDuration = 3.0f; // 되감기 지속 시간 (초)
    public float positionRecordInterval = 0.1f; // 위치 기록 간격
    public KeyCode rewindKey = KeyCode.Z; // 되감기 활성화 키
    public int maxHealth = 100;

    private float rewindTime;
    private List<Vector3> positionHistory = new List<Vector3>();
    private List<int> healthHistory = new List<int>();
    private int currentHealth;
    private bool isRewinding = false;
    private float recordTimer = 0.0f;

    void Start()
    {
        Controller = GetComponent<Controller>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // 역행 중이 아니면 위치와 체력을 주기적으로 기록
        if (!isRewinding)
        {
            recordTimer += Time.deltaTime;
            if (recordTimer >= positionRecordInterval)
            {
                positionHistory.Add(transform.position);
                healthHistory.Add(currentHealth);
                recordTimer = 0.0f;
            }

            // 기록된 위치의 수를 되감기 지속 시간에 맞게 제한
            while (positionHistory.Count > Mathf.FloorToInt(rewindDuration / positionRecordInterval))
            {
                positionHistory.RemoveAt(0);
                healthHistory.RemoveAt(0);
            }
        }

        // 되감기 시작
        if (Input.GetKeyDown(rewindKey) && positionHistory.Count > 0)
        {
            Debug.Log("시간 역행 시작");
            StartRewind();
        }

        // 되감기 진행
        if (isRewinding)
        {
            rewindTime -= Time.deltaTime;
            int rewindIndex = Mathf.FloorToInt(rewindTime / positionRecordInterval);
            if (rewindIndex < 0 || rewindIndex >= positionHistory.Count)
            {
                StopRewind();
            }
            else
            {
                Controller.enabled = false; // 수동 위치 업데이트를 위해 Controller 비활성화
                transform.position = positionHistory[rewindIndex];
                currentHealth = healthHistory[rewindIndex];
                Controller.enabled = true;
            }
        }
    }

    void StartRewind()
    {
        isRewinding = true;
        rewindTime = rewindDuration;
    }

    void StopRewind()
    {
        isRewinding = false;
    }

    public void TakeDamage(int damage)
    {
        if (!isRewinding)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                // 캐릭터 사망 처리
                Debug.Log("캐릭터가 사망했습니다.");
            }
        }
    }
}
