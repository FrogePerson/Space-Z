using UnityEngine;

public class PlayerHealthChangedEvent
{
    public int ConnId { get; }
    public int NewHealth { get; }

    public PlayerHealthChangedEvent(int connId, int newHealth)
    {
        ConnId = connId;
        NewHealth = newHealth;
    }
}
