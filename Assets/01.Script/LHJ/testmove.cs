#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
public class SaveHandPos : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] Transform leftHandPos;
    [SerializeField] Transform rightHandPos;
    [ContextMenu("Save_Hand_Pose")]
    public void SavePos()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(leftHandPos.gameObject, true);
        recorder.BindComponentsOfType<Transform>(rightHandPos.gameObject, true);
        recorder.TakeSnapshot(0);
        //recorder.SaveToClip()
    }
#endif
}
