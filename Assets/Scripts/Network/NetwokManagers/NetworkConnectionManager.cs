using log4net;
using Mirror;
using System.Linq;
using UnityEngine;

[Tooltip("Класс для сетевого управления NetworkManager")]
public class NetworkConnectionManager : NetworkBehaviour
{
    static readonly ILog log = Log4NetLogger.SetLogger(typeof(NetworkConnectionManager));

    [SyncVar]
    [SerializeField]
    int _activePlayers = 0;
    public int ActivePlayers 
    { 
        get 
        { 
            return _activePlayers; 
        } 
        private set 
        { 
            _activePlayers = value; 
        } 
    }

    public readonly SyncList<Player.ActivePlayer> Players = new SyncList<Player.ActivePlayer>();

    public Player.ActivePlayer TryGetPlayerById(uint id)
    {
        return Players.FirstOrDefault(p => p != null && p.ConnId == id);
    }

    [Server]
    public void ServerSetActivePlayers(int ActivePlayers)
    {
        this.ActivePlayers = ActivePlayers;

        Log4NetLogger.LogDbg($"Изменено количество активных игроков, ActivePlayers = {this.ActivePlayers}", log);
    }

    [Server]
    public void ServerAddPlayerToList(Player.ActivePlayer player)
    {
        Players.Add(player);

        Log4NetLogger.Log($"Добавлен игрок с id = {player.ConnId}", log);
    }
    [Server]
    public void ServerRemovePlayerFromList(Player.ActivePlayer player)
    {
        Players.Remove(player);

        Log4NetLogger.Log($"Удалён игрок с id = {player.ConnId}", log);
    }




}