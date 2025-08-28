using Interfaces;
using log4net;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool : NetworkBehaviour
{
    static readonly ILog log = Log4NetLogger.SetLogger(typeof(Pool));

    [Header("Настройки")]
    [SerializeField] private GameObject[] gameObjects;
    [SyncVar] public int SumOfAvailableObjects = 0;

    private Stack<GameObject> stack = new Stack<GameObject>();
    private Dictionary<GameObject, int> objectToIndex = new Dictionary<GameObject, int>();

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (gameObjects == null || gameObjects.Length == 0)
        {
            Debug.LogError("Нет объектов в пуле");

            Log4NetLogger.LogError("Нет объектов в пуле", log);
            return;
        }

        SumOfAvailableObjects = gameObjects.Length;

        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject obj = gameObjects[i];
            stack.Push(obj);
            objectToIndex[obj] = i;
            obj.SetActive(false);
        }
        Log4NetLogger.Log($"Инициализирован пулл c {gameObjects.Length} объектами", log);
    }

    #region Server Side

    [Server]
    public GameObject ServerPop()
    {
        if (stack.Count == 0)
        {
            Log4NetLogger.Log($"Закончилсь объекты в пулле!", log);
            return null;
        }

        GameObject obj = stack.Pop();
        SumOfAvailableObjects--;

        obj.SetActive(true);

        int index = GetObjectIndex(obj);

        RpcActivateObject(index);

        Log4NetLogger.Log($"Включен объект с индексом {index} на сервере ", log);

        return obj;
    }

    [Server]
    public void ServerPush(GameObject obj)
    {
        if (!objectToIndex.ContainsKey(obj))
        {
            Log4NetLogger.LogError($"Объект не из пулла!", log);
            return;
        }

        stack.Push(obj);
        SumOfAvailableObjects++;

        obj.SetActive(false);

        int index = GetObjectIndex(obj);

        RpcDeactivateObject(index);

        Log4NetLogger.Log($"Выключен объект с индексом {index} на сервере ", log);
    }

    [Server]
    private int GetObjectIndex(GameObject obj)
    {
        return objectToIndex[obj];
    }

    #endregion

    #region Client RPCs
    private GameObject GetObjectByIndex(int index)
    {
        if (index >= 0 && index < gameObjects.Length)
            return gameObjects[index];

        Log4NetLogger.LogError($"Нет объекта в пуле с индексом = {index}", log);
        return null;
    }

    [ClientRpc]
    private void RpcActivateObject(int objectIndex)
    {
        GameObject obj = GetObjectByIndex(objectIndex);
        if (obj != null)
        {
            obj.SetActive(true);

            Log4NetLogger.Log($"Включен объект с индексом {objectIndex} на клиенте ", log);
        }
    }

    [ClientRpc]
    private void RpcDeactivateObject(int objectIndex)
    {
        GameObject obj = GetObjectByIndex(objectIndex);
        if (obj != null)
        {
            IPoolObj poolObj = obj.GetComponent<IPoolObj>();

            poolObj.ReturnToPool();

            obj.SetActive(false);

            Log4NetLogger.Log($"Выключен объект с индексом {objectIndex} на клиенте ", log);
        }
    }

    #endregion

    #region Public Methods

    [Command]
    public void CmdPop()
    {
        ServerPop();
    }

    [Command]
    public void CmdPush(GameObject obj)
    {
        ServerPush(obj);
    }


    public void Pop()
    {
        if (isServer)
        {
            ServerPop();
        }
        else
        {
            CmdPop();
        }
    }


    public void Push(GameObject obj)
    {
        if (isServer)
        {
            ServerPush(obj);
        }
        else
        {
            CmdPush(obj);
        }
    }

    #endregion
}