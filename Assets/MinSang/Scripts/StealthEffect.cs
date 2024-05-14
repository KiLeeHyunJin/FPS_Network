using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StealthEffect : MonoBehaviour
{
    public Material StealthMaterial; // 클로킹에 사용할 재질
    public float StealthDuration = 5f; // 클로킹 지속 시간
    public float StealthTransparency = 0.2f; // 클로킹 투명도
    public KeyCode StealthKey = KeyCode.L;

    private Color originalColor; // 원래 색상을 저장할 변수

    void Start()
    {
        originalColor = StealthMaterial.color; // 초기 색상 저장
    }
    
    void Update()
    {
        if (Input.GetKeyDown(StealthKey))  // '' 키를 누르면
        {
            Debug.Log("클로킹");
            GetComponent<StealthEffect>().ActivateStealth();  // 클로킹 활성화
        }
    }

    // 클로킹 활성화
    public void ActivateStealth()
    {
        StartCoroutine(StealthRoutine());
    }

    // 클로킹 실행 코루틴
    IEnumerator StealthRoutine()
    {
        // 투명도 적용
        Color StealthColor = originalColor;
        StealthColor.a = StealthTransparency;
        StealthMaterial.color = StealthColor;

        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(StealthDuration);

        // 원래 색상으로 복원
        StealthMaterial.color = originalColor;
    }
}
