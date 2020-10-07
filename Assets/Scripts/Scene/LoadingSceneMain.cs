using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneMain : BaseSceneMain
{
    private const float nextSceneIntaval = 3f;
    private const float textUpdateIntaval = 0.15f;
    private const string loadingTextValue = "Loading...";

    [SerializeField]
    private Text loadingText = null;

    private int textIndex = 0;
    private float lastUpdateTime = 0f;

    private float sceneStartTime = 0f;
    private bool isNextSceneCall = false;

    protected override void OnStart()
    {
        sceneStartTime = Time.time;
    }

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if (currentTime - lastUpdateTime > textUpdateIntaval)
        {
            loadingText.text = loadingTextValue.Substring(0, textIndex + 1);

            textIndex++;
            if (textIndex >= loadingTextValue.Length)
            {
                textIndex = 0;
            }

            lastUpdateTime = currentTime;
        }

        if (currentTime - sceneStartTime > nextSceneIntaval)
        {
            if (isNextSceneCall == false)
                GotoNextScene();
        }
    }

    private void GotoNextScene()
    {
        SceneController.Instance.LoadScene(SceneType.InGame);
        isNextSceneCall = true;
    }
}