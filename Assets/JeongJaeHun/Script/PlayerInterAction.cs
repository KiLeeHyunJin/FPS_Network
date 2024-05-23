using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterAction : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            Debug.Log("player interaction trigger on ");
            interactable?.Interaction(gameObject);
        }
    }

}
