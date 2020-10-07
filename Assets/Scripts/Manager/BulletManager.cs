using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField]
    private PrefabCacheData[] bulletFiles = null;

    private Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    private void Start()
    {
        Prepare();
    }

    public GameObject Load(string _resourcePath)
    {
        GameObject obj = null;

        if (fileCache.ContainsKey(_resourcePath) == true)
        {
            obj = fileCache[_resourcePath];
        }
        else
        {
            obj = Resources.Load<GameObject>(_resourcePath);
            if (obj == null)
                return null;

            fileCache.Add(_resourcePath, obj);
        }

        return obj;
    }

    public void Prepare()
    {
        for (int i = 0; i < bulletFiles.Length; i++)
        {
            GameObject obj = Load(bulletFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.GenerateCache(bulletFiles[i].filePath, obj, bulletFiles[i].cacheCount);
        }
    }

    public Bullet Generate(BulletType _bulletType)
    {
        string filePath = bulletFiles[(int)_bulletType].filePath;
        GameObject obj = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Archive(filePath);

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.FilePath = filePath;

        return bullet;
    }

    public bool Remove(Bullet _bullet)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Restore(_bullet.FilePath, _bullet.gameObject);
        return true;
    }
}