using UnityEngine;

public class FPSCameraPosition : MonoBehaviour
{
    [field: SerializeField] 
    public Transform StandPos   { get; private set; }
    
    [field: SerializeField] 
    public Transform CrouchPos  { get; private set; }


}
