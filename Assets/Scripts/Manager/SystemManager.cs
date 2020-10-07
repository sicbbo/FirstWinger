using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance;
    public static SystemManager Instance { get { return instance; } }

    [SerializeField]
    private EnemyTable enemyTable = null;
    public EnemyTable EnemyTable { get { return enemyTable; } }

    private BaseSceneMain currentSceneMain = null;
    public BaseSceneMain CurrentSceneMain { set { currentSceneMain = value; } }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        Debug.Log(string.Format("OnSceneLoaded! baseSceneMain.name = {0}", baseSceneMain.name));
        currentSceneMain = baseSceneMain;
    }

    public T GetCurrentSceneMain<T>()
        where T : BaseSceneMain
    {
        return currentSceneMain as T;
    }
}