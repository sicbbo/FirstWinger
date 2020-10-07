using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public const string enemyPath = "Prefabs/Enemy";

    private Dictionary<string, GameObject> enemyFileCache = new Dictionary<string, GameObject>();

    public GameObject Load(string _resourcePath)
    {
        GameObject res = null;

        if (enemyFileCache.ContainsKey(_resourcePath))
        {
            res = enemyFileCache[_resourcePath];
        }
        else
        {
            res = Resources.Load<GameObject>(_resourcePath);
            if (res == null)
            {
                Debug.LogError(string.Format("Load Error : path = {0]", _resourcePath));
                return null;
            }

            enemyFileCache.Add(_resourcePath, res);
        }

        return res;
    }
}