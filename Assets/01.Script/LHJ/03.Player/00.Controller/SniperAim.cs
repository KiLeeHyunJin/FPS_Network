using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAim : MonoBehaviour
{
    [SerializeField] GameObject zoom;
    public void OnAim(bool state)
    {
        zoom.SetActive(state);
    }
}
