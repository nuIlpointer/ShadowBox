using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenNextScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private GameObject ipAddressInput;
    [SerializeField] private GameObject portInput;
    [SerializeField] private GameObject playerNameInput;
    [SerializeField] private GameObject skinIdInput;

    /// <summary>
    /// 本体のシーンを開きます。
    /// </summary>
    /// <param name="ipAddress">接続先IP</param>
    /// <param name="port">接続先ポート</param>
    /// <param name="multi">マルチプレイかどうか</param>
    /// <param name="name">名前</param>
    /// <param name="skin">スキンタイプ</param>
    void LoadNext(string ipAddress, int port, bool multi, string name, int skin) {
        TitleData.ipAddress = ipAddress;
        TitleData.port = port;
        TitleData.isMultiPlay = multi;
        TitleData.playerName = name;
        TitleData.skinID = skin;

        SceneManager.LoadScene(nextSceneName);
    }

    public void OpenLocal() {
        LoadNext("127.0.0.1", 11781, false, "Player", 1);
    }

    public void OpenMulti() {

    }
}
