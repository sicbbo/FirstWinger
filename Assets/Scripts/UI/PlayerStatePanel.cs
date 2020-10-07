using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatePanel : BasePanel
{
    [SerializeField]
    private Text scoreValue = null;
    [SerializeField]
    private UIGage HPGage = null;

    public void SetScore(int _value)
    {
        scoreValue.text = _value.ToString();
    }

    public void SetHP(float _currentValue, float _maxValue)
    {
        HPGage.SetGage(_currentValue, _maxValue);
    }
}