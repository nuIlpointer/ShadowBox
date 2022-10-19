using UnityEngine;
using Unity.Networking.Transport;
using System.Net;
using Unity.Collections;

public class ShadowBoxClient : MonoBehaviour {
    private IPAddress connectAddress;
    private int connectPort;
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// 現在接続しているサーバーへチャンクデータを要求する
    /// </summary>
    /// <param name="chunkNumber">要求するチャンク</param>
    /// <returns>チャンク情報(int型2次元配列)</returns>
    public int[,] getChunk(int chunkNumber) {
        return null;
    }

    /// <summary>
    /// チャンクを現在接続しているサーバーに送信する
    /// </summary>
    /// <param name="chunkNumber">送信するチャンクの場所</param>
    /// <param name="sendChunkData">送信するチャンク情報</param>
    /// <returns>送信に成功したか</returns>
    public bool sendChunk(int chunkNumber, int[,] sendChunkData) {
        return false;
    }

    /// <summary>
    /// 接続先のポート/IPアドレスを指定する。
    /// ポートがnullの時は「11781」。
    /// </summary>
    /// <param name="ipAddress">IPアドレス。"127.0.0.1"でローカル。</param>
    /// <param name="port">接続先ポート番号。デフォルトは11781。</param>
    public void setConnection(string ipAddress, int port) {
        if (port == null) port = 11781;

    }
}
