using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    private GameObject panelObject = null;

    private void Awake()
    {
        panelObject = gameObject;

        InitializePanel();
    }

    private void Update()
    {
        UpdatePanel();
    }

    private void OnDestroy()
    {
        DestroyPanel();
    }

    public virtual void InitializePanel()
    {
        PanelManager.RegistPanel(GetType(), this);
    }

    public virtual void UpdatePanel()
    {

    }

    public virtual void DestroyPanel()
    {
        PanelManager.UnregistPanel(GetType());
    }

    public virtual void Show()
    {
        panelObject.SetActive(true);
    }

    public virtual void Close()
    {
        panelObject.SetActive(false);
    }
}