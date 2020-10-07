using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    [SerializeField]
    private Transform canvasTrans = null;
    public Transform CanvasTrans { get { return canvasTrans; } }
    [SerializeField]
    PrefabCacheData[] damageFiles = null;

    private Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    private void Start()
    {
        Prepare();
    }

    public GameObject GenerateDamage(DamageType _damageType, Vector3 _position, int _damageValue, Color _textColor)
    {
        string filePath = damageFiles[(int)_damageType].filePath;
        GameObject obj = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Archive(filePath);
        obj.transform.position = Camera.main.WorldToScreenPoint(_position);

        UIDamage damage = obj.GetComponent<UIDamage>();
        damage.FilePath = filePath;
        damage.ShowDamage(_damageValue, _textColor);

        return obj;
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

    private void Prepare()
    {
        for (int i = 0; i < damageFiles.Length; i++)
        {
            GameObject obj = Load(damageFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.GenerateCache(damageFiles[i].filePath, obj, damageFiles[i].cacheCount, canvasTrans);
        }
    }

    public bool Remove(UIDamage _damage)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Restore(_damage.FilePath, _damage.gameObject);
        return true;
    }
}