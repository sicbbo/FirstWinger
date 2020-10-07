using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkConnectionInfo
{
    public bool Host = false;               // 호스트로 실행 여부
    public string IPAdress = string.Empty;  // 클라이언트로 실행시 접속할 호스트의 IP 주소
    public int Port = 0;                    // 클라이언트로 실행시 접속할 호스트의 Port
}