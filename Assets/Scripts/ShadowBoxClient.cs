using UnityEngine;
using Unity.Networking.Transport;
using System.Net;
using Unity.Collections;
using Unity.Collections;

public class ShadowBoxClient : MonoBehaviour {
    enum BlockLayer {
        InsideWall   = 1,
        InsideBlock  = 2,
        OutsideWall  = 3,
        OutsideBlock = 4
    };

    private IPAddress connectAddress;
    private int connectPort;
    private NetworkDriver driver;
    private NetworkEndPoint endPoint;
    private NetworkConnection connection;

    void Start() {
        this.driver = NetworkDriver.Create();
        
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// 現在接続しているサーバーへチャンクデータを要求する
    /// </summary>
    /// <param name="layerId">受信するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkId">要求するチャンク</param>
    /// <returns>チャンク情報(int型2次元配列)</returns>
    public int[,] getChunk(int layerId, int chunkId) {
        return null;
    }

    /// <summary>
    /// チャンクを現在接続しているサーバーに送信する
    /// </summary>
    /// <param name="layerId">送信するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkId">送信するチャンクの場所</param>
    /// <param name="sendChunkData">送信するチャンク情報</param>
    /// <returns>送信に成功したか</returns>
    public bool sendChunk(int layerId, int chunkId, int[,] sendChunkData) {

        return false;
    }

    /// <summary>
    /// 接続先のポート/IPアドレスを指定し、接続する。
    /// ポートがnullの時は「11781」。
    /// </summary>
    /// <param name="ipAddress">IPアドレス。"127.0.0.1"でローカル。</param>
    /// <param name="port">接続先ポート番号。デフォルトは11781。</param>
    public void connect(string ipAddress, int port) {
        this.connectAddress = IPAddress.Parse(ipAddress);
        endPoint = NetworkEndPoint.AnyIpv4;
        using(NativeArray<byte> rawIpAddr = new NativeArray<byte>(this.connectAddress.GetAddressBytes().Length, Allocator.Temp)) {
            endPoint.SetRawAddressBytes(rawIpAddr);
        }

        if (port < 1 || port > 65535) port = 11781;
        endPoint.Port = (ushort)port;
        this.connectPort = port;
        this.connection = this.driver.Connect(endPoint);
    }
}
