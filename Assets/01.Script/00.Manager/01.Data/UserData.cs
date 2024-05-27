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
    public int win;
    public int lose;
    public UserData()
    {
        this.NickName = "non";
        this.profileImageName = null;
        this.Level = 1;
        this.DeathCount = 0;
        this.KillCount = 0;
        this.PlayCount = 0;
        this.win = 0;
        this.lose = 0;
    }
    public float GetWinRate()
    {
        int totalGames = win + lose;
        if (totalGames == 0)
        {
            return 0f;
        }
        float winRate = (float)win / totalGames * 100;
        return (float)Math.Round(winRate, 2);
    }
}

