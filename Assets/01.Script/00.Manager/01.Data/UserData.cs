using System;
using System.Collections;
[Serializable]
public class UserData
{
    public string NickName;
    public int DeathCount;
    public int KillCount;
    public int Level;
    public int PlayCount;
    public UserData ()
    {
        this.NickName = "non";
    }
    public UserData(string _nickName,int _level = 0,int _playCount = 0,int _killCount = 0, int _deathCount = 0)
    {
        this.NickName = _nickName;
        DeathCount = _deathCount;
        KillCount = _killCount;
        Level = _level;
        PlayCount = _playCount;
    }
}

