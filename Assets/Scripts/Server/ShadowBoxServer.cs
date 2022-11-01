using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using System.IO;

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


        // 以下デバッグ 
         /*

        //仮で適当なintのArrayを生成する
        int[][] arr = {
            new int[] {1, 2, 3, 4, 5},

            new int[] {1, 2, 3, 4, 5},

            new int[] {1, 2, 3, 4, 5},

            new int[] {1, 2, 3, 4, 5},

            new int[] {1, 2, 3, 4, 5},
        };

        //チャンクライターテスト
        SaveChunk(BlockLayer.InsideBlock, 0, arr);

        //チャンクリーダーテスト
        foreach (int[] larr in LoadChunk(BlockLayer.InsideBlock, 0))
            Debug.Log(string.Join(",", larr));
        // */
    }

    /// <summary>
    /// ドライバと接続情報の破棄を行う
    /// </summary>
    public void OnDestroy() {
        this.driver.Dispose();
        if(this.connectionList.IsCreated) {

            this.connectionList.Dispose();
        }
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
    /// 内部サーバー(127.0.0.1:11781)を作成する
    /// </summary>
    public void CreateInternalServer() {

    }

    /// <summary>
    /// レイヤーデータを保存する
    /// </summary>
    /// <param name="layerID">レイヤーのID</param>
    /// <param name="chunkID">チャンクのID</param>
    /// <param name="chunkData">保存するデータ</param>
    void SaveChunk(BlockLayer layerID, int chunkID, int[][] chunkData) {
        if(!Directory.Exists("./worlddata/")) {
            Directory.CreateDirectory("./worlddata");
        }
        string fileName = "./worlddata/" + layerID + ".chunk" + chunkID + ".dat";
        using (var writer = new StreamWriter(fileName, false, Encoding.UTF8)) {
            foreach(int[] row in chunkData)
                writer.WriteLine(string.Join(",", row));
        }
    }

#nullable enable
    /// <summary>
    /// レイヤーデータをファイルから読み込む
    /// </summary>
    /// <param name="layerID">読み込むレイヤーのID</param>
    /// <param name="chunkId">読み込むチャンクのID</param>
    /// <returns></returns>
    int[][]? LoadChunk(BlockLayer layerID, int chunkId) {
        string fileName = "./worlddata/" + layerID + ".chunk" + chunkId + ".dat";
        if(File.Exists(fileName)) {
            List<int[]> tempList = new List<int[]>();
            using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
                while (0 <= reader.Peek())
                    tempList.Add(Array.ConvertAll(reader.ReadLine().Split(','), int.Parse));
                return tempList.ToArray();
            }
        } else return null;
    }


}
