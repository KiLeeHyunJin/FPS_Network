using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveHandPos : MonoBehaviour
{
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
}
