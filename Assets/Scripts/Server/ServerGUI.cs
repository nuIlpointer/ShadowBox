using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServerGUI : MonoBehaviour
{
    [SerializeField] private GameObject serverPrefab;
    [SerializeField] private GameObject portNumObject;
    [SerializeField] private GameObject enableDebugLog;
    [SerializeField] private GameObject logObj;

    [SerializeField] private GameObject worldChunkSizeObj;
    [SerializeField] private GameObject worldSizeXObj;
    [SerializeField] private GameObject worldSizeYObj;
    [SerializeField] private GameObject worldHeightRangeObj;
    [SerializeField] private GameObject worldSeedObj;

    private TMP_InputField logArea;
    private GameObject serverObject;
    private ShadowBoxServer server;
    private bool isStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        logArea = logObj.GetComponent<TMP_InputField>();
    }

    public void ToggleServer() {
        if (!isStarted) StartServer(); else StopServer();    
    }
    private void StartServer() {
        serverPrefab.GetComponent<ShadowBoxServer>().debugMode = enableDebugLog.GetComponent<Toggle>().isOn;
        server = (serverObject = Instantiate(serverPrefab)).GetComponent<ShadowBoxServer>();
        server.StartServer(int.Parse(portNumObject.GetComponent<InputField>().text));
        Log("開始しました。");
        isStarted = true;
    }

    private void StopServer() {
        Destroy(serverObject);
        Log("停止しました。");
        isStarted = false;
    }
    public void Log(string text) {
        logArea.text = $"{text}\n{logArea.text}";
    }
    public void InitializeGUI(WorldInfo worldInfo) {

    }

    public void RegenerateWorld() {
        
    }
}
