using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;

public class ShadowBoxServer : MonoBehaviour {
    public enum BlockLayer {
        InsideWall = 1,
        InsideBlock = 2,
        OutsideWall = 3,
        OutsideBlock = 4
    }

    public struct PlayerData {
        public string name;
        public int skinType;
        public Guid playerID;
        public float playerX;
        public float playerY;
        public BlockLayer playerLayer;
        public override string ToString() => $"{name},{skinType},{playerID.ToString()},{playerX},{playerY},{playerLayer}";
    }

    private NetworkDriver driver;
    private NativeList<NetworkConnection> connectionList;
    private Dictionary<Guid, PlayerData> userList;
    private Dictionary<BlockLayer, int[][][]> layerCache;
    private bool active = false;
    // Start is called before the first frame update
    void Start() {
        userList = new Dictionary<Guid, PlayerData>();
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
        if (this.connectionList.IsCreated) {
            this.connectionList.Dispose();
        }
    }

    /// <summary>
    /// StartServer()が実行済か確認する
    /// </summary>
    /// <returns>実行済かどうかの bool 値</returns>
    public bool IsActive() {
        return active;
    }

    public bool StartServer(int port) {
        this.driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = (ushort)port;
        Debug.Log("Trying to bind port " + port);
        if (this.driver.Bind(endpoint) != 0) {
            Debug.LogError("Failed to bind port " + port + ".");
            return false;
        } else this.driver.Listen();
        this.connectionList = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        Debug.Log("Listen on " + port);
        active = true;
        return true;
    }

    // Update is called once per frame
    void Update() {
        if (active) {
            this.driver.ScheduleUpdate().Complete();

            for (int i = 0; i < this.connectionList.Length; i++) {
                if (!this.connectionList[i].IsCreated) { //破棄されたコネクションを削除
                    this.connectionList.RemoveAtSwapBack(i);
                    i--;
                }
            }

            NetworkConnection connection;
            while ((connection = this.driver.Accept()) != default(NetworkConnection)) {
                this.connectionList.Add(connection);
            }

            DataStreamReader stream;
            for (int i = 0; i < this.connectionList.Length; i++) {
                //コネクションが作成されているかどうか
                Assert.IsTrue(this.connectionList[i].IsCreated);

                NetworkEvent.Type cmd;
                while ((cmd = this.driver.PopEventForConnection(this.connectionList[i], out stream)) != NetworkEvent.Type.Empty) {
                    if (cmd == NetworkEvent.Type.Data) {
                        //データを受信したとき
                        String receivedData = ("" + stream.ReadFixedString4096());
                        if (receivedData.StartsWith("SCH")) { //チャンクを受信したとき
                            receivedData = receivedData.Replace("SCH,", "");
                            Debug.Log("Received Chunk Data: \n" + receivedData);
                        }

                        if (receivedData.StartsWith("SPD")) { //プレイヤーデータを受信したとき
                            var dataArr = receivedData.Split(',');
                            PlayerData newPlayer;
                            newPlayer.name = dataArr[1];
                            newPlayer.skinType = Int32.Parse(dataArr[2]);
                            newPlayer.playerID = Guid.Parse(dataArr[3]);
                            newPlayer.playerX = float.Parse(dataArr[4]);
                            newPlayer.playerY = float.Parse(dataArr[5]);
                            newPlayer.playerLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[6]);
                            userList[Guid.Parse(dataArr[3])] = newPlayer;

                            Debug.Log("ADD USER DATA: " + userList[Guid.Parse(dataArr[3])].ToString());
                        }
                    } else if (cmd == NetworkEvent.Type.Disconnect) {
                        Debug.Log("Disconnected.");
                        this.connectionList[i].Disconnect(driver);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 内部サーバー(127.0.0.1:11781)を作成する
    /// </summary>
    public void CreateInternalServer() {
        StartServer(11781);
    }

    /// <summary>
    /// レイヤーデータを保存する
    /// </summary>
    /// <param name="layerID">レイヤーのID</param>
    /// <param name="chunkID">チャンクのID</param>
    /// <param name="chunkData">保存するデータ</param>
    void SaveChunk(BlockLayer layerID, int chunkID, int[][] chunkData) {
        if (!Directory.Exists("./worlddata/")) {
            Directory.CreateDirectory("./worlddata");
        }
        string fileName = "./worlddata/" + layerID + ".chunk" + chunkID + ".dat";
        using (var writer = new StreamWriter(fileName, false, Encoding.UTF8)) {
            foreach (int[] row in chunkData)
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
        if (File.Exists(fileName)) {
            List<int[]> tempList = new List<int[]>();
            using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
                while (0 <= reader.Peek())
                    tempList.Add(Array.ConvertAll(reader.ReadLine().Split(','), int.Parse));
                return tempList.ToArray();
            }
        } else return null;
    }


}
