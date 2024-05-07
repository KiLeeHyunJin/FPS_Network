using UnityEngine;

public class FollowAnimationCurve : MonoBehaviour
{
    [field : SerializeField]
    public AnimationCurve curve { get; set; }

    public void SetCurves(AnimationCurve xC)
    {
        curve = xC;
    }
    
    void Update()
    {
        transform.position = new Vector3(curve.Evaluate(Time.time),0,0);
    }
}
