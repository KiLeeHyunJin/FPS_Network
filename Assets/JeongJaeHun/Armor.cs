using UnityEngine;

public class Armor : MonoBehaviour
{
    [SerializeField]
    private int armorId; //아머의 ID 번호
    [SerializeField]
    private int armorDefense; // 아머의 방어력 
    [SerializeField]
    private int armorDurability; //아머의 내구도 

    //Set 금지. 
    public int ArmorID { get { return armorId; } }
    public int ArmorDefense { get { return armorDefense; } }

    public int ArmorDurability { get { return armorDurability; } set { armorDurability = value; } }

    private void Start()
    {
        
    }

    private void OnEnable() // Armor가 켜질 때 작동 시킬 작업. 
    {
       
    }

}
