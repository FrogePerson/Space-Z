using Interfaces;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Pool : NetworkBehaviour
{
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
    }

    #region Server Side

    [Server]
    public GameObject ServerPop()
    {
        if (stack.Count == 0)
        {
            return null;
        }

        GameObject obj = stack.Pop();
        SumOfAvailableObjects--;

        // Активируем объект на сервере
        obj.SetActive(true);

        // Синхронизируем клиентам
        RpcActivateObject(GetObjectIndex(obj));

        return obj;
    }

    [Server]
    public void ServerPush(GameObject obj)
    {
        if (!objectToIndex.ContainsKey(obj))
        {
            Debug.LogWarning("Объект не из пулла!");
            return;
        }

        stack.Push(obj);
        SumOfAvailableObjects++;

        obj.SetActive(false);

        RpcDeactivateObject(GetObjectIndex(obj));
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
        return null;
    }

    [ClientRpc]
    private void RpcActivateObject(int objectIndex)
    {
        GameObject obj = GetObjectByIndex(objectIndex);
        if (obj != null)
        {
            obj.SetActive(true);
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