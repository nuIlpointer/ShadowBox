using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
public class ShadowBoxServer : MonoBehaviour {
    public enum BlockLayer {
        InsideWall = 1,
        InsideBlock = 2,
        OutsideWall = 3,
        OutsideBlock = 4
    }

    public struct PlayerData {
        string name;
        int skinType;
        Guid playerID;
        float playerX;
        float playerY;
        BlockLayer playerLayer;
    }

    private NetworkDriver driver;
    private NativeList<NetworkConnection> connectionList;
    private Dictionary<Guid, PlayerData> userList;
    private Dictionary<BlockLayer, int[][][]> layerCache;

    // Start is called before the first frame update
    void Start() {
    }

    /// <summary>
    /// ドライバと接続情報の破棄を行う
    /// </summary>
    public void OnDestroy() {
        this.driver.Dispose();
        this.connectionList.Dispose();
    }

    public bool StartServer(int port) {
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = (ushort)port;

        if (this.driver.Bind(endpoint) != 0) {
            Debug.LogError("Failed to bind port " + port + ".");
            return false;
        } else this.driver.Listen();

        this.connectionList = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        Debug.Log("Listen on " + port);
        return true;
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// 内部サーバー(127.0.0.1:11781
    /// </summary>
    public void CreateInternalServer() {

    }

    /// <summary>
    /// レイヤーデータを保存する
    /// </summary>
    /// <param name="layerID"></param>
    /// <param name="chunkData"></param>
    void SaveChunk(BlockLayer layerID, int[][] chunkData) {

    }

    /// <summary>
    /// レイヤーデータをファイルから読み込む
    /// </summary>
    /// <param name="layerID">読み込むレイヤーのID</param>
    /// <param name="chunkId">読み込むチャンクのID</param>
    /// <returns></returns>
    int[][]? LoadChunk(BlockLayer layerID, int chunkId) {
        return null;
    }


}
