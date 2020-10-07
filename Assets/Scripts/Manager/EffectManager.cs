using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private PrefabCacheData[] effectFiles = null;

    private Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    private void Start()
    {
        Prepare();
    }

    public GameObject GenerateEffect(EffectType _effectType, Vector3 _position)
    {
        string filePath = effectFiles[(int)_effectType].filePath;
        GameObject obj = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Archive(filePath);
        obj.transform.position = _position;

        AutoCachableEffect effect = obj.GetComponent<AutoCachableEffect>();
        effect.FilePath = filePath;

        return obj;
    }

    private GameObject Load(string _resourcePath)
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

    private void Prepare()
    {
        for (int i = 0; i < effectFiles.Length; i++)
        {
            GameObject obj = Load(effectFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.GenerateCache(effectFiles[i].filePath, obj, effectFiles[i].cacheCount);
        }
    }

    public bool RemoveEffect(AutoCachableEffect _effect)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Restore(_effect.FilePath, _effect.gameObject);
        return true;
    }
}