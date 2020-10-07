using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private EnemyFactory enemyFactory = null;
    [SerializeField]
    private PrefabCacheData[] enemyFiles = null;
    private List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        Prepare();
    }

    public bool GenerateEnemy(SquadronMemberStruct _data)
    {
        string filePath = SystemManager.Instance.EnemyTable.GetEnemyData(_data.enemyID).filePath;
        GameObject obj = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Archive(filePath);

        obj.transform.position = new Vector3(_data.generatePointX, _data.generatePointY, 0f);
        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.FilePath = filePath;
        enemy.Reset(_data);

        enemies.Add(enemy);
        return true;
    }

    public bool RemoveEnemy(Enemy _enemy)
    {
        if (enemies.Contains(_enemy) == false)
            return false;

        enemies.Remove(_enemy);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.Restore(_enemy.FilePath, _enemy.gameObject);

        return true;
    }

    private void Prepare()
    {
        for (int i = 0; i < enemyFiles.Length; i++)
        {
            GameObject obj = enemyFactory.Load(enemyFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CacheSystem.GenerateCache(enemyFiles[i].filePath, obj, enemyFiles[i].cacheCount);
        }
    }
}