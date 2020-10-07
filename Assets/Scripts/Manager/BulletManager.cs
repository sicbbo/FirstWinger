using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField]
    private PrefabCacheData[] bulletFiles = null;

    private Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

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
        if (((FWNetworkManager)FWNetworkManager.singleton).isServer == false)
            return;

        for (int i = 0; i < bulletFiles.Length; i++)
        {
            GameObject obj = Load(bulletFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.GenerateCache(bulletFiles[i].filePath, obj, bulletFiles[i].cacheCount, this.transform);
        }
    }

    public Bullet Generate(BulletType _bulletType)
    {
        if (((FWNetworkManager)FWNetworkManager.singleton).isServer == false)
            return null;

        string filePath = bulletFiles[(int)_bulletType].filePath;
        GameObject obj = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Archive(filePath);

        Bullet bullet = obj.GetComponent<Bullet>();

        return bullet;
    }

    public bool Remove(Bullet _bullet)
    {
        if (((FWNetworkManager)FWNetworkManager.singleton).isServer == false)
            return true;

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Restore(_bullet.FilePath, _bullet.gameObject);
        return true;
    }
}