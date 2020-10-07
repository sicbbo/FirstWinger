using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PrefabCacheData
{
    public string filePath;
    public int cacheCount;
}

public class PrefabCacheSystem
{
    private Dictionary<string, Queue<GameObject>> caches = new Dictionary<string, Queue<GameObject>>();

    public void GenerateCache(string _filePath, GameObject _gameObject, int _cacheCount, Transform _parentTrans = null)
    {
        if (caches.ContainsKey(_filePath))
        {
            return;
        }
        else
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < _cacheCount; i++)
            {
                GameObject obj = Object.Instantiate<GameObject>(_gameObject, _parentTrans);
                obj.SetActive(false);
                queue.Enqueue(obj);

                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.FilePath = _filePath;
                    NetworkServer.Spawn(obj);
                }

                Bullet bullet = obj.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.FilePath = _filePath;
                    NetworkServer.Spawn(obj);
                }
            }

            caches.Add(_filePath, queue);
        }
    }

    public GameObject Archive(string _filePath)
    {
        if (caches.ContainsKey(_filePath) == false)
            return null;

        if (caches[_filePath].Count == 0)
            return null;

        GameObject obj = caches[_filePath].Dequeue();
        obj.SetActive(true);

        if (((FWNetworkManager)FWNetworkManager.singleton).isServer == true)
        {
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RpcSetActive(true);
            }

            Bullet bullet = obj.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.RpcSetActive(true);
            }
        }

        return obj;
    }

    public bool Restore(string _filePath, GameObject _gameObject)
    {
        if (caches.ContainsKey(_filePath) == false)
            return false;

        _gameObject.SetActive(false);
        if (((FWNetworkManager)FWNetworkManager.singleton).isServer == true)
        {
            Enemy enemy = _gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RpcSetActive(false);
            }
            Bullet bullet = _gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.RpcSetActive(false);
            }
        }

        caches[_filePath].Enqueue(_gameObject);
        return true;
    }

    public void Add(string _filePath, GameObject _obj)
    {
        Queue<GameObject> queue;
        if (caches.ContainsKey(_filePath) == true)
        {
            queue = caches[_filePath];
        }
        else
        {
            queue = new Queue<GameObject>();
            caches.Add(_filePath, queue);
        }
    }
}