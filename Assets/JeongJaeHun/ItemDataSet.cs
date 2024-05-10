using UnityEngine;

namespace JJH
{
    public class ItemDataSet : MonoBehaviour
    {
        private int _id;
        private string _name;
        private string _tooltip;
        private Sprite _iconSprite;
        private GameObject _dropItemPrefab;


        private int price;
        private float _maxDistance; //수류탄용 

        public int ID //// 아이템의 id 
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name  //아이템의 이름 
        {
            get { return _name; }
            set { _name = value; }
        }

        
        public string Tooltip //상점에서 사용할 아이템의 설명 및 스펙 
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        public Sprite IconSprite  //상점에서 사용할 이미지 
        {
            get { return _iconSprite; }
            set { _iconSprite = value; }
        }

        public GameObject DropItmePrefab //아이템을 떨궛을 때 생길 프리팹 
        {
            get { return _dropItemPrefab; }
            set { _dropItemPrefab = value; }
        }


        public float MaxDistance // 수류탄 (총은 미정 )의 최대 사정거리 
        {
            get { return _maxDistance; }
            set { _maxDistance = value; }
        }

        public int Price
        {
            get { return price; }
            set { price = value; }
        }



        public enum ArmorType //방어구의 타입 
        {
            Standard_Armor, //기본 시작 방어구 
            Light_Armor,
            Middle_Armor,
            Heavy_Armor,
            END
        }

        public enum GunType //총의 타입 
        {
            AR, SR, SMG, StandardPistol,
            END
        }

        public enum SwordType // 칼의 타입 
        {
            ShortSword, //기본 시작 칼 
            longSword
        }

        public enum ThrowType //투척물의 타입 
        {
            Grenade, //구매해야 생기는 걸로 하기. --> 같은 칸을 공유하며 한 번에 한 종류밖에 가질 수 없게하자.
            Flashbang
        }


    }


}
