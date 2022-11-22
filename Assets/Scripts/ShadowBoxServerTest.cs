using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBoxServerTest : MonoBehaviour
{
    GameObject serverObj;
    ShadowBoxServer server;
    // Start is called before the first frame update
    void Start()
    {
        serverObj = GameObject.Find("Server");
        serverObj.GetComponent<ShadowBoxServer>();
        server.CreateInternalServer(); //ローカルでサーバーを立てる
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
