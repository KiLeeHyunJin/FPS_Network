public class Define
{
    public enum PlayerProperty
    {
        Ready, Team, Load,
    }
    public enum RoomProperty
    {
        LoadTime,
    }

    public enum TeamType
    {
        Red, Blue,
    }

    public enum Weapon
    {
        Gun, Sword,
        END
    }

    public enum Heal
    {
        Drink, FirstAde,
        END
    }

    public enum Bullet
    {
        Five, Seven, Nine, FourFive,
        END
    }
    public enum Wearable
    {
        Helmet, Vest, Bag,
        END
    }
    public enum Equip
    {
        Wearable, Weapon,
        END
    }
    public enum FireType
    {
        One, Repeat,END
    }
    public enum InputWeaponType
    {
        MainWeapon, SubWeapon, Default, FlashBang, Grenade, END
    }
    public enum Item
    {
        Heal, Bullet, Part, Equip,
        END
    }

    public enum Key
    {
        Shift, C, X, Z, Space, F, F1, F2, F3, F4, Press, Pressed,
        V, R,Enter,
        Alt, Zoom,
        END
    }
    public enum State
    {
        Idle, Crouch, Run, Walk, Prone, Jump,
        Clim, Heal,
        Fire, Reload, Swap,
        Interaction, Door, Pick,
        Death,
        END
    }
    public enum Gun
    {
        AR, SR, SMG, Pistol,
        END
    }
    public enum GunPart
    {
        Eye, Grap, Shoulder, Muzzle, Cartridge,
        END
    }

    public enum UIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        Speech,
        Max,
    }

    public enum ItemType
    {
        Ect, Consume, Equip,
    }

}
