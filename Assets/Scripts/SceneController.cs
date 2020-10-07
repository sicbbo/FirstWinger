using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController instance = null;
    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("SceneController");
                if (obj == null)
                {
                    obj = new GameObject("SceneController");

                    SceneController controller = obj.AddComponent<SceneController>();
                    return controller;
                }
                else
                {
                    instance = obj.GetComponent<SceneController>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Can't have two instance of singletone.");
            DestroyImmediate(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public void LoadScene(SceneType _sceneType)
    {
        StartCoroutine(LoadSceneAsync(_sceneType, LoadSceneMode.Single));
    }

    public void LoadSceneAdditive(SceneType _sceneType)
    {
        StartCoroutine(LoadSceneAsync(_sceneType, LoadSceneMode.Additive));
    }

    private IEnumerator LoadSceneAsync(SceneType _sceneType, LoadSceneMode _loadSceneMode)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)_sceneType, _loadSceneMode);

        while (asyncOperation.isDone == false)
            yield return null;

        Debug.Log("LoadSceneAsync is complete");
    }

    public void OnActiveSceneChanged(Scene _scene0, Scene _scene1)
    {
        Debug.Log(string.Format("OnActiveSceneChanged is called! scene0 = {0}, scene1 = {1}", _scene0.name, _scene1.name));
    }

    public void OnSceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        Debug.Log(string.Format("OnSceneLoaded is called! scene = {0}, loadSceneMode = {1}", _scene.name, _loadSceneMode.ToString()));

        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        Debug.Log(string.Format("OnSceneLoaded! baseSceneMain.name = {0}", baseSceneMain.name));
        SystemManager.Instance.CurrentSceneMain = baseSceneMain;
    }

    public void OnSceneUnloaded(Scene _scene)
    {
        Debug.Log(string.Format("OnSceneUnloaded is called! scene = {0}", _scene.name));
    }
}