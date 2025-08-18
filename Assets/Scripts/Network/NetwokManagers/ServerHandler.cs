using log4net;
using Mirror;
using System;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class ServerHandler : MonoBehaviour
{
    static readonly ILog log = Log4NetLogger.SetLogger(typeof(ServerHandler));


    [Server]
    public static void ByteDataMessageHandle(NetworkConnection connection, ByteDataMessage message)
    {
        Log4NetLogger.Log($"Сервер получил сообщение типа ByteDataMessage:{string.Join(", ", message.data)}, от сущности с netid = {connection.identity.netId}", log);
    }

    void OnDestroy()
    {
        if (NetworkServer.active) NetworkServer.UnregisterHandler<ByteDataMessage>();
    }
}

