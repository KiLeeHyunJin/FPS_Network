using JJH;

public class ArmorData : ItemDataSet, IInteractable
{
    private int _defense;
    private int _durability;

    //추후에 추가적으로 가격이나 프리팹 형태 등 들어가야 되면 넣으면 된다. 

    public int Defense  //방어구 아이템의 방어도. 
    {
        get { return _defense; }
        set { _defense = value; }
    }

    public int Durability  //방어구 아이템의 내구도 
    {
        get { return _durability; }
        set { _durability = value; }
    }

    public ArmorType armorType; //public enum을 통해 실제로 inspector 창에서 아머 구분해주기. 

    private void Start()
    {

        if (armorType == ItemDataSet.ArmorType.Standard_Armor) //아머가 기본 아머
        {

            Price = 0; //기본으로 주어짐. ++ 상속 받은 price 임
            Defense = 1;
            Durability = 5;

        }
        else if (armorType == ItemDataSet.ArmorType.Light_Armor) //경량 아머 
        {
            Price = 100;
            Defense = 2;
            Durability = 10;

        }
        else if (armorType == ItemDataSet.ArmorType.Middle_Armor) //중간 아머 
        {
            Price = 200;
            Defense = 3;
            Durability = 15;

        }
        else if (armorType == ItemDataSet.ArmorType.Heavy_Armor) //무거운 아머 
        {

            Price = 300;
            Defense = 4;
            Durability = 20;

        }


    }

    public void Interaction() //아머 같은 경우는 바닥에서 줍는 경우가 없으니까 일단 구현 x 
    {

    }

    private void Update()
    {

    }



    public ArmorData(ArmorType armorType)
    {
        Price = Price;
        _defense = Defense;
        _durability = Durability;
    }

}
