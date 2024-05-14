using System;
using UnityEngine;

public class BombController : MonoBehaviour
{
    private Camera mainCamera; //메인 카메라
    [SerializeField] private Bomb currentBomb; //현재 들고 있는 폭탄 --> 수류탄 프리팹이나 마찬가지 아니냐?
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float throwPower; //수류탄 투척력

    private void Start()
    {
        // 현재 폭탄은 이거 다른 스크립트에서 조정하는것 같은데? 
        // 일단 임시로 bomb 인스펙터 참조해두기. 
        mainCamera = Camera.main;
        lineRenderer = currentBomb.GetComponent<LineRenderer>();
        Throw(); // fire1 하면 투척하고. 
        ShowTrajectory(); //던지기 준비하면 궤도를 보여주고 --> 수류탄을 들고 있으면?

    }

    // 이게자식으로 들어있어서 그런지는 모르겠는데 
    private void Throw() //투척 함수 
    {
        // 투척 위치 계산 
        // 이게 손에 들어가 있는 상황에서는 transform을 어떻게 잡아줘야 될지 고민되네
        // local 인지 전역 position인지.. 
        Vector3 position = transform.position;
        Vector3 forward = mainCamera.transform.forward; //카메라의 전방으로 라인을 그릴 거기 때문에 
        Vector3 startVelocity = throwPower * forward; // 전방으로 던지는 힘. 
        Vector3 startPosition = transform.position;
        // 스타트 포지션 조금 생각해보자. 이거 실제 아이템 팔에 붙어있는 localPosition으로 잡아주면 되지 않나?
        
        //Vector3 startPosition = transform.localPosition;  --> 이거 실험해보자. 

        //아무튼 실제로는 던져도 실물을 던지는게 아니라 복사본? 약간 그런걸 던지면됨. 
        Rigidbody rigidGrenade = currentBomb.GetComponent<Rigidbody>();
        rigidGrenade.AddForce(startVelocity, ForceMode.Impulse);
        Destroy(rigidGrenade.gameObject,5f); 
    }

    private void ShowTrajectory() //궤도를 보여줌 (라인렌더러)
    {
        // 궤적 표시 관련 변수 초기화
        int linePoint = 1;
        float timePoint = 0.1f;

        //라인 렌더러 점 개수 설정
        lineRenderer.positionCount=Mathf.CeilToInt(linePoint / timePoint) + 1;

        // 초기 위치 및 방향 설정
        Vector3 forward = mainCamera.transform.forward;
        Vector3 startVelocity = throwPower * forward;
        Vector3 startPosition = transform.position; //홀더 위치? --> 홀더도 자식이니까 실제 홀더면 local인가?
        

        //초기 위치 설정
        lineRenderer.SetPosition(0, startPosition);

        for(int i=0;i<lineRenderer.positionCount;i++) // 1/0.1 이므로 10개의 선으로 이루어져 곡선을 그림
        {
            float time = i * timePoint;

            // 시간에 따른 궤적 위치 계산 
            Vector3 point = startPosition + (time * startVelocity);
            point.y=startPosition.y + startVelocity.y * time + (Physics.gravity.y / 1f * time * time);

            // 라인 렌더러에 궤적 위치 설정
            lineRenderer.SetPosition(i, point);
        }

    }


}
