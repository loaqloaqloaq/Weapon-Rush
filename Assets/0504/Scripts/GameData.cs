
public struct PlayerInform
{
    public int victoryCount;
    public int weaponCount;
    public void Init()
    {
        victoryCount = 0;
        weaponCount = 0;
    }
}

public static class GameData 
{
    static public PlayerInform p1, p2;

    static public int winnerId;

    static public void Initialize()
    {
        p1.Init();
        p2.Init();
        winnerId = 0;
    }
}
