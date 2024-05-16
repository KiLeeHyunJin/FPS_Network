using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEditor;

// 게임 오브젝트가 이전 상태로 돌아갈 수 있도록 시간역행 기능을 제공
public class TimeRewind : MonoBehaviourPun, IDamagable, ISkill
{
    public Controller Controller;
    public float rewindDuration = 3.0f;
    public float positionRecordInterval = 0.1f;
    public KeyCode rewindKey = KeyCode.Z;
    [SerializeField] public int maxHealth = 100;

    private Vector3[] positionHistory;
    private int[] healthHistory;
    private int historyIndex = 0;
    private int currentHealth;
    private int maxPositionHistoryCount;
    private Coroutine rewindCoroutine;
    bool on;

    public void Activate()
    {
        StartCoroutine(RewindCoroutine());
    }

    // 역행을 비활성화할 때 모든 활성 코루틴을 중지
    public void Deactivate()
    {
        StopAllCoroutines();
    }

    // 초기 상태와 이력 배열을 설정
    void Start()
    {
        Controller = GetComponent<Controller>();
        currentHealth = maxHealth;
        maxPositionHistoryCount = Mathf.CeilToInt(rewindDuration / positionRecordInterval);

        positionHistory = new Vector3[maxPositionHistoryCount];
        healthHistory = new int[maxPositionHistoryCount];

    }

    // 매 프레임마다 입력을 처리하고 위치를 기록
    void Update()
    {
        if (!IsRewinding())
        {
            if (!on)
                StartCoroutine(RecordPositionAndHealth());
            if (Input.GetKeyDown(rewindKey))
            {
                Debug.Log("시간 역행");
                rewindCoroutine = StartCoroutine(RewindCoroutine());
            }
        }
    }

    IEnumerator RecordPositionAndHealth() // 위치와 체력 기록
    {
        if (!on)
        {
            on = true;

            positionHistory[historyIndex] = transform.position;
            // Debug.Log($"index = {historyIndex} , position {positionHistory[historyIndex]}");
            healthHistory[historyIndex] = currentHealth;
            historyIndex = (historyIndex + 1) % maxPositionHistoryCount;
            yield return new WaitForSeconds(positionRecordInterval);
            on = false;
        }



    }

    IEnumerator RewindCoroutine()
    {
        Controller.enabled = false;
        float time = 0;

        while (time < rewindDuration)
        {
            int rewindIndex = (historyIndex - 1 - Mathf.FloorToInt(time / positionRecordInterval) + maxPositionHistoryCount) % maxPositionHistoryCount;
            // Debug.Log($"RewindPos {positionHistory[rewindIndex]} , index {rewindIndex} ");
            transform.position = positionHistory[rewindIndex];
            currentHealth = healthHistory[rewindIndex];
            time += Time.deltaTime;
            yield return null;
        }

        Controller.enabled = true;
        if (rewindCoroutine != null)
        {
            StopCoroutine(rewindCoroutine);
            rewindCoroutine = null;
        }
    }

    // 객체가 피해를 입었을 때 호출되는 메소드로 현재 역행 중이 아닐 때만 실행
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