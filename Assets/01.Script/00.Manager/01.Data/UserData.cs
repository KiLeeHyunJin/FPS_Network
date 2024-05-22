using System;
[Serializable]
public class UserData
{
    public string NickName;
    public int DeathCount;
    public int KillCount;
    public int AssistCount;
    public int Level;
    public int PlayCount;
    public string profileImageName;
    public UserData()
    {
        this.NickName = "non";
        this.profileImageName = null;
        this.Level = 1;
        this.DeathCount = 0;
        this.KillCount = 0;
        this.PlayCount = 0;
    }

}

