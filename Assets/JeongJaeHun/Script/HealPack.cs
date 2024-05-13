using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealPack : MonoBehaviour,IInteractable
{

    

    public int heal = 50;

    public Slider slider;
    public TextMeshProUGUI text;

    private void Start()
    {
        
    }

    
    public void Interaction(GameObject player)
    {
        Controller controller= player.GetComponent<Controller>();
        
        //Controller 컴포넌트가 있다면
        if(controller != null)
        {
            controller.AddHp(heal);
        }

        Destroy(gameObject);
    }

   // 스킬 힐은 어떤 식으로?? 


}
