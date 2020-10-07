using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        return obj;
    }

    public bool Restore(string _filePath, GameObject _gameObject)
    {
        if (caches.ContainsKey(_filePath) == false)
            return false;

        _gameObject.SetActive(false);

        caches[_filePath].Enqueue(_gameObject);
        return true;
    }
}