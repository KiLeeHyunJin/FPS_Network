using Photon.Realtime;
using System;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
public static class DefinePropertyKey
{
    
    public static string READY = "Ready";
    public static string LOAD = "Load";
    public static string RED = "Red";
    public static string BLUE = "Blue";
    public static string RANDOM_MATCH = "RandomMatch";
    public static string CUSTOMROOM = "CustomRoom";
    public static string CHAT = "Chat";
    public static string START = "Start";
    public static string LOADVALUE = "LoadValue";
    public static string LOADCOMPLETE = "LoadComplete";
}
public static class CustomProperty
{
    
    public static T GetProperty<T>(this Player player, string key)
    {
        PhotonHashtable property = player.CustomProperties;
        //key값이 있으면 해당 해시테이블에서 값을 가져와 T타입으로 변환해서 반환
        //없을 경우 해당 타입의 디폴트 값을 반환
        return property.ContainsKey(key) ? (T)property[key] : default(T);
    }

    public static void SetProperty<T>(this Player player, string str, T value)
    {
       // PhotonHashtable property = player.CustomProperties;                               
        PhotonHashtable property = new PhotonHashtable();                                    //set할때는 새로 만들고 해야함 이유는 각자 찾아보셈
        if (property.ContainsKey(str) == false) 
            //해당 프로퍼티가 포함되어있지 않을 경우 해시테이블을 생성
            property = new PhotonHashtable { { str, value } };
        else
            //포함되어있을 경우 값을 변경
            property[str] = value;
        //프로퍼티를 저장
        player.SetCustomProperties(property);
    }


    public static T GetProperty<T>(this Room room, string key)
    {
        PhotonHashtable property = room.CustomProperties;
        return property.ContainsKey(key) ? (T)property[key] : default(T);
    }
    public static void SetProperty<T>(this Room room, string str, T value)
    {
        PhotonHashtable property = room.CustomProperties;//new PhotonHashtable();
        if (property.ContainsKey(str) == false)
            /*PhotonHashtable*/
            property = new PhotonHashtable { { str, value } };
        else
            property[str] = value;
        // property.Add(str, value);
        room.SetCustomProperties(property);
    }






















    public static bool GetLoad(this Player player)
    {
        PhotonHashtable property = player.CustomProperties;
        if (property.ContainsKey(Define.PlayerProperty.Load))
            return (bool)property[Define.PlayerProperty.Load];
        else
            return false;
    }

    public static void SetLoad(this Player player, bool load)
    {
        PhotonHashtable property = new PhotonHashtable();
        property[Define.PlayerProperty.Load] = load;
        player.SetCustomProperties(property);
    }

    public static double GetLoadTime(this Room room)
    {
        PhotonHashtable property = room.CustomProperties;
        if (property.ContainsKey(Define.RoomProperty.LoadTime))
            return (double)property[Define.RoomProperty.LoadTime];
        else
            return -1;
    }

    public static void SetLoadTime(this Room room, double loadTime)
    {
        PhotonHashtable property = new PhotonHashtable();
        property[Define.RoomProperty.LoadTime] = loadTime;
        room.SetCustomProperties(property);
    }
}
