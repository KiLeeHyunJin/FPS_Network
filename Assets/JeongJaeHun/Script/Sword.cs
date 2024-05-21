using UnityEngine;
using UnityEngine.UI;

public class Sword : CloseWeapon //close 웨폰 상속 
{
    //일단 여기에 다 모아보자. 

    [Tooltip("무기 공격 궤적")]
    public TrailRenderer TrailRenderer;

    [Tooltip("활성화 여부")]
    public static bool isActivate = true;

    [Tooltip("무기의 range 체크 범위 활성화")]
    [SerializeField] private bool debug;
    

    


    private void OnEnable()
    {
        SetUp(); //함수를 통해서 기본 데미지 세팅을 진행. 
        
    }

    private void SetUp()
    {

        // 일단은 인스펙터창에서 할당해 줬음. 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        Vector3 rightDir = Quaternion.Euler(0, angle * 0.5f, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, angle * -0.5f, 0) * transform.forward;

        Debug.DrawRay(transform.position, rightDir * range, Color.cyan);
        Debug.DrawRay(transform.position, leftDir * range, Color.cyan);
    }


}
