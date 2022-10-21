using UnityEngine;
using Unity.Networking.Transport;
using System.Net;
using Unity.Collections;

public class ShadowBoxClient : MonoBehaviour {
    public enum BlockLayer {
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
    private bool active = false;
    void Start() {
        this.driver = NetworkDriver.Create();
        // TODO さっさとやれ
    }

    // Update is called once per frame
    void Update() {
        // TODO さっさとやれ
    }

    /// <summary>
    /// 現在接続しているサーバーへチャンクデータを要求する
    /// </summary>
    /// <param name="layerId">要求するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkId">要求するチャンク</param>
    /// <returns>チャンク情報(int型2次元配列)</returns>
    public int[][] getChunk(int layerId, int chunkId) {
        // TODO さっさとやれ
        return null;
    }

    /// <summary>
    /// チャンクを現在接続しているサーバーに送信する
    /// </summary>
    /// <param name="layerId">送信するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkId">送信するチャンクの場所</param>
    /// <param name="chunkData">送信するチャンク情報</param>
    /// <returns>送信に成功したか</returns>
    public bool sendChunk(int layerId, int chunkId, int[][] chunkData) {
        // まだ作り途中ですよ
        if(this.connection.IsCreated) {
            string sendDataTemp = "";
            foreach (int[] chunkRow in chunkData)
                for (int i = 0; i < chunkRow.Length; i++)
                    sendDataTemp += chunkRow[i] + (i == chunkRow.Length - 1 ? "\n" : ",");
            if((this.driver.BeginSend(this.connection, out DataStreamWriter dsw) >= 0)) {
                dsw.WriteFixedString4096(new FixedString4096Bytes(sendDataTemp));
                this.driver.EndSend(dsw);
            }
        }
        return false;
    }

    /// <summary>
    /// 接続先のポート/IPアドレスを指定し、接続する。
    /// ポートが範囲外の時は自動的に「11781」。
    /// </summary>
    /// <param name="ipAddress">接続先IPアドレス。</param>
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
