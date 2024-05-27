using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterAction : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("플레이어 트리거");
        if(other.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            Debug.Log("player interaction trigger on ");
            interactable?.Interaction(gameObject);
        }
    }

}
