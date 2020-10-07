using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConfigPanel : BasePanel
{
    private const string defaultIPAdress = "localhost";
    private const string defaultPort = "7777";

    [SerializeField]
    private InputField ipAdressInputField = null;
    [SerializeField]
    private InputField portInputField = null;

    public override void InitializePanel()
    {
        base.InitializePanel();
        //: IP와 Port 입력을 기본 값으로 셋팅.
        ipAdressInputField.text = defaultIPAdress;
        portInputField.text = defaultPort;
        Close();
    }

    public void OnHostButton()
    {
        SystemManager.Instance.ConnectionInfo.Host = true;
        TitleSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<TitleSceneMain>();
        sceneMain.GotoNextScene();
    }

    public void OnClientButton()
    {
        SystemManager.Instance.ConnectionInfo.Host = false;

        TitleSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<TitleSceneMain>();

        // IP 입력 값
        if (string.IsNullOrEmpty(ipAdressInputField.text) == false || ipAdressInputField.text != defaultIPAdress)
            SystemManager.Instance.ConnectionInfo.IPAdress = ipAdressInputField.text;

        if (string.IsNullOrEmpty(portInputField.text) == false || portInputField.text != defaultPort)
        {
            int port = 0;
            if (int.TryParse(portInputField.text, out port))
                SystemManager.Instance.ConnectionInfo.Port = port;
            else
            {
                Debug.LogError(string.Format("OnClientButton Error Port = {0}", portInputField.text));
                return;
            }
        }

        sceneMain.GotoNextScene();
    }
}