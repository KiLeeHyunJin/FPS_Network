using System.Collections;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    [SerializeField] public int changeImgIdx { get; protected set; }
    public abstract IEnumerator LoadingRoutine();
}
