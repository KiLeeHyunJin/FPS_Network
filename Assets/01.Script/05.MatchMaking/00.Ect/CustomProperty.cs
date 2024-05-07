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
}
public static class CustomProperty
{
    public static T GetProperty<T>(this Player player, string key)
    {
        PhotonHashtable property = player.CustomProperties;
        return property.ContainsKey(key) ? (T)property[key] : default(T);
    }
    public static void SetProperty<T>(this Player player, string str, T value)
    {
        PhotonHashtable property = player.CustomProperties;//new PhotonHashtable();
        if (property.ContainsKey(str) == false)
            /*PhotonHashtable*/ property = new PhotonHashtable { { str, value } };
        else
            property[str] = value;
        // property.Add(str, value);
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
