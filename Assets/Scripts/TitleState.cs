using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TitleData {
    public static string ipAddress = "127.0.0.1";
    public static int port = 11781;
    public static bool isMultiPlay = false;
    public static string playerName = "Player";
    public static int skinID = 1;
}

public class TitleState : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private GameObject serverItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConnectedToServer() {

    }
}
