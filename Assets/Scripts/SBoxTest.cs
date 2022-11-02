/*
 * ShadowBoxServer and ClientWrapper
 * Tester by nuilpointer
 * !!DO NOT IMPORT THIS CODE WHEN MERGE!!
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBoxTest : MonoBehaviour {
    GameObject client;
    GameObject server;

    private float time = 0.0f;
    bool ready = false;

    int[][] testarr = {
        new int[] {1, 2, 3, 4, 5},
        new int[] {1, 2, 3, 4, 5},
        new int[] {1, 2, 3, 4, 5},
        new int[] {1, 2, 3, 4, 5},
        new int[] {1, 2, 3, 4, 5},
    };

    // Start is called before the first frame update
    void Start() {
        client = GameObject.Find("Client");
        server = GameObject.Find("Server");
        server.GetComponent<ShadowBoxServer>().CreateInternalServer();
        ready = true;
    }

    // Update is called once per frame
    void Update() {

        time += Time.deltaTime;
        if(time > 1.5) {
            if (ready) {

                client.GetComponent<ShadowBoxClientWrapper>().Connect("127.0.0.1", 11781);
                ready = false;
            }
            time = 0;
            if (client.GetComponent<ShadowBoxClientWrapper>().SendChunk(ShadowBoxClientWrapper.BlockLayer.InsideWall, 0, testarr)) Debug.Log("送信成功");
        }
    }
}
