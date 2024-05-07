using System;
using System.Xml.Serialization;

[Serializable]
public class GameData
{
    public class CharacterData
    {

        [XmlAttribute]
        public int ID;
        [XmlAttribute]
        public int level;
        [XmlAttribute]
        public int maxHp;
        [XmlAttribute]
        public int maxMp;
        [XmlAttribute]
        public int maxEXP;

        [XmlAttribute]
        public int moveSpeed;
        [XmlAttribute]
        public int atckSpeed;
        [XmlAttribute]
        public int staminaHealSpeed;

        [XmlAttribute]
        public int hp;
        [XmlAttribute]
        public int mp;
        [XmlAttribute]
        public int stamina;

        [XmlAttribute]
        public int atck;
        [XmlAttribute]
        public float critical;


    }

    public class UserData
    {
        public string userName;
        public int level;
        public int money;
    }

}
