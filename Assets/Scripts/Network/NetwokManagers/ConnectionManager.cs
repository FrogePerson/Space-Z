using log4net;
using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(NetworkConnectionManager))]
public class ConnectionManager : NetworkManager
{
    static readonly ILog log = Log4NetLogger.SetLogger(typeof(ConnectionManager));

    NetworkConnectionManager networkConnectionManager;

    [Header("Настройки")]
    [SerializeField]
    bool _canClientConnect =true;
    public bool CanClientConnect
    {
        get
        {
            return _canClientConnect;
        }
        private set
        {
            _canClientConnect = value;
        }
    }
    [SerializeField]
    bool _canClientDisconnect =true;
    public bool CanClientDisconnect
    {
        get
        {
            return _canClientDisconnect;
        }
        private set
        {
            _canClientDisconnect= value;
        }
    }

    [SerializeField]
    Transform startPosition;


    [Header("Подключено игроков: ")]
    [SerializeField]
    int _connectedPlayers = 0;
    public int ConnectedPlayers
    {
        get 
        {
            return _connectedPlayers;
        }
        private set
        {
            _connectedPlayers = value;
        }
    }

    public int ActivePlayers;

    [Header("Подключённые игроки: ")]
    [SerializeField]
    List<Player.ActivePlayer> Players = new List<Player.ActivePlayer>();

    public Player.ActivePlayer TryGetPlayerById(int id)
    {
        return Players.FirstOrDefault(p => p != null && p.ConnId == id);
    }

    public override void Start()
    {
        base.Start();
        NetworkManagerHUD hud = GetComponent<NetworkManagerHUD>();
        if (hud != null) hud.manager = this;//костыль!!!, сделали поле с networkManager public для переназначения на наш менеджер

        if (startPosition != null) RegisterStartPosition(startPosition);

        networkConnectionManager = GetComponent<NetworkConnectionManager>();

    }

    public override void Update()
    {
        base.Update();
        if(ActivePlayers != numPlayers)
        {
            ActivePlayers = numPlayers;
            networkConnectionManager.ServerSetActivePlayers(ActivePlayers);
        }
        
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<ByteDataMessage>(ServerHandler.ByteDataMessageHandle);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (CanClientConnect)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);

            var tmp = player.GetComponent<Player.ActivePlayer>();

            tmp.ConnId = conn.connectionId;

            Players.Add(tmp);
            networkConnectionManager.ServerAddPlayerToList(tmp);

            Log4NetLogger.Log($"Добавлен клиент с id = {tmp.ConnId}", log);
        }
        else
        {
            Log4NetLogger.Log($"Нельзя добавить клиента c id = {conn.connectionId}", log);
        }
    }

    //просто информативное сообщение, клиент подключается всегда
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Log4NetLogger.Log($"Клиент подключён с id = {conn.connectionId}", log);
        ConnectedPlayers += 1;   
    }

    //срабатывает всегда даже если нельзя подключать клиента
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (CanClientDisconnect)
        {
            base.OnServerDisconnect(conn);
            ConnectedPlayers -= 1;

            var tmp = Players.FirstOrDefault(t => t != null && t.ConnId == conn.connectionId);

            if(tmp != null)
            {
                Players.Remove(tmp);
                networkConnectionManager.ServerRemovePlayerFromList(tmp);

                Log4NetLogger.Log($"Отключён клиент с id = {conn.connectionId}", log);
            }
            else
            {
                Log4NetLogger.Log($"Отключён не добавленный клиент с id = {conn.connectionId}", log);
            }
        }
        else
        {
            Log4NetLogger.Log($"Клиента нельзя отключить с id = {conn.connectionId} ", log);
        } 
    }
}
