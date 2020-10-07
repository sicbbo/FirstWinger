using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private static Dictionary<Type, BasePanel> panels = new Dictionary<Type, BasePanel>();

    public static bool RegistPanel(Type _panelClassType, BasePanel _basePanel)
    {
        if (panels.ContainsKey(_panelClassType))
        {
            Debug.LogError(string.Format("RegistPanel Error! Already exist Type! PanelClassType = {0}", _panelClassType.ToString()));
            return false;
        }

        Debug.Log(string.Format("RegistPanel is called! Type = {0}, basePanel = {1}", _panelClassType.ToString(), _basePanel.name));

        panels.Add(_panelClassType, _basePanel);
        return true;
    }

    public static bool UnregistPanel(Type _panelClassType)
    {
        if (panels.ContainsKey(_panelClassType) == false)
        {
            Debug.LogError(string.Format("UnregistPanel Error! Can't Find Type! PanelClassType = {0}", _panelClassType.ToString()));
            return false;
        }

        panels.Remove(_panelClassType);
        return true;
    }

    public static BasePanel GetPanel(Type _panelClassType)
    {
        if (panels.ContainsKey(_panelClassType) == false)
        {
            Debug.LogError(string.Format("UnregistPanel Error! Can't Find Type! PanelClassType = {0}", _panelClassType.ToString()));
            return null;
        }

        return panels[_panelClassType];
    }
}