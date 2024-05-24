using UnityEngine;

public class Armor : MonoBehaviour
{
    [SerializeField]
    private int armorLevel; // 아머의 lv  0 ,1 2 로 setting 
    [SerializeField]
    private int armorDefense; // 아머의 방어력 
    [SerializeField]
    private int armorDurability; //아머의 내구도 

    //Set 금지. 
    public int ArmorLevel { get { return armorLevel; } set { armorLevel = value; } }
    public int ArmorDefense { get { return armorDefense; } set { ArmorDefense = value; } }

    public int ArmorDurability { get { return armorDurability; } set { armorDurability = value; } }

    // 각 Armor 마다 가지고 있어야 하는 데이터들. 

}
