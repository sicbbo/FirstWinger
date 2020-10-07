using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoCachableEffect : MonoBehaviour
{
    private string filePath = string.Empty;
    public string FilePath { get { return filePath; } set { filePath = value; } }

    private ParticleSystem pSystem = null;

    private void OnEnable()
    {
        pSystem = GetComponent<ParticleSystem>();
        StartCoroutine(CheckIfAlive());
    }

    private IEnumerator CheckIfAlive()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            if (pSystem.IsAlive(true) == false)
            {
                SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.RemoveEffect(this);
                break;
            }
        }
    }
}