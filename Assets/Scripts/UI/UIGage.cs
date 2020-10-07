using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGage : MonoBehaviour
{
    [SerializeField]
    private Slider slider = null;

    public void SetGage(float _currentValue, float _maxValue)
    {
        if (_currentValue > _maxValue)
            _currentValue = _maxValue;

        slider.value = _currentValue / _maxValue;
    }
}