using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamage : MonoBehaviour
{
    [SerializeField]
    private UIDamageState damageState = UIDamageState.None;
    [SerializeField]
    private Text damageText = null;

    const float sizeUpDuration = 0.1f;
    const float displayDuration = 0.5f;
    const float fadeOutDuration = 0.2f;

    private Vector3 currentVelocity = Vector3.zero;

    private float displayStartTime = 0f;
    private float fadeOutStartTime = 0f;

    private string filePath = string.Empty;
    public string FilePath { get { return filePath; } set { filePath = value; } }

    private void Update()
    {
        UpdateDamage();
    }

    public void ShowDamage(int _damage, Color _textColor)
    {
        damageText.text = _damage.ToString();
        damageText.color = _textColor;
        Reset();
        damageState = UIDamageState.SizeUp;
    }

    private void Reset()
    {
        transform.localScale = Vector3.zero;
        Color newColor = damageText.color;
        newColor.a = 1f;
        damageText.color = newColor;
    }

    private void UpdateDamage()
    {
        if (damageState == UIDamageState.None)
            return;

        switch (damageState)
        {
            case UIDamageState.SizeUp:
                transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref currentVelocity, sizeUpDuration);
                if (transform.localScale == Vector3.one)
                {
                    damageState = UIDamageState.Display;
                    displayStartTime = Time.time;
                }
                break;
            case UIDamageState.Display:
                if (Time.time - displayStartTime > displayDuration)
                {
                    damageState = UIDamageState.FadeOut;
                    fadeOutStartTime = Time.time;
                }
                break;
            case UIDamageState.FadeOut:
                Color newColor = damageText.color;
                newColor.a = Mathf.Lerp(1, 0, (Time.time - fadeOutStartTime) / fadeOutDuration);
                damageText.color = newColor;

                if (newColor.a == 0)
                {
                    damageState = UIDamageState.None;
                    SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Remove(this);
                }
                break;
        }
    }
}