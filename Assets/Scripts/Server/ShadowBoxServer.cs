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
        public int actState;
        public Guid playerID;
        public float playerX;
        public float playerY;
        public BlockLayer playerLayer;
        public override string ToString() => $"{name},{skinType},{actState},{playerID.ToString()},{playerX},{playerY},{playerLayer}";
    }

    private NetworkDriver driver;
    private NativeList<NetworkConnection> connectionList;
    private Dictionary<Guid, PlayerData> userList;
    private Dictionary<BlockLayer, int[][][]> layerCache;
    private bool active = false;
    // Start is called before the first frame update
    void Start() {
        userList = new Dictionary<Guid, PlayerData>();
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
        this.connectionList = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = (ushort)port;
        if (this.driver.Bind(endpoint) != 0) {
            Debug.LogError("[SERVER]Failed to bind port " + port + ".");
            return false;
        } else this.driver.Listen();
        Debug.Log("[SERVER]Listen on " + port);
        active = true;
        return true;
    }

    // Update is called once per frame
    void Update() {
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
                if (cmd == NetworkEvent.Type.Connect) {
                    Debug.Log("[SERVER]User Connected.");
                } else if (cmd == NetworkEvent.Type.Data) {
                    //データを受信したとき
                    String receivedData = ("" + stream.ReadFixedString4096());
                    if (receivedData.StartsWith("SCH")) { //チャンクを受信したとき
                        receivedData = receivedData.Replace("SCH,", "");
                        Debug.Log("[SERVER]Receive chunk data: \n" + receivedData);
                        BlockLayer layerID = (BlockLayer)Enum.Parse(typeof(BlockLayer), receivedData.Split(',')[0]);
                        int chunkID = Int32.Parse(receivedData.Split(',')[1]);
                        receivedData = receivedData.Replace($"{layerID},{chunkID},", "");
                        List<int[]> tempArr = new List<int[]>();
                        foreach (string line in receivedData.Split('\n'))
                            if (line != "")
                                tempArr.Add(Array.ConvertAll(line.Split(','), int.Parse));
                        SaveChunk(layerID, chunkID, tempArr.ToArray());
                    }

                    if (receivedData.StartsWith("SPD")) { //プレイヤーデータを受信したとき
                        var dataArr = receivedData.Split(',');
                        PlayerData newPlayer;
                        // 受信したデータをPlayerDataに落としこむ
                        newPlayer.name = dataArr[1];
                        newPlayer.skinType = Int32.Parse(dataArr[2]);
                        newPlayer.actState = Int32.Parse(dataArr[3]);
                        newPlayer.playerID = Guid.Parse(dataArr[4]);
                        newPlayer.playerX = float.Parse(dataArr[5]);
                        newPlayer.playerY = float.Parse(dataArr[6]);
                        newPlayer.playerLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[7]);
                        userList[newPlayer.playerID] = newPlayer;

                        //デバッグ出力
                        Debug.Log("[SERVER]Recieve new user data: " + userList[newPlayer.playerID].ToString());
                    }

                    //送出
                    if(receivedData.StartsWith("RQC")) { //チャンク要求を受け取った時
                        Debug.Log("[SERVER]Recieve chunk request from client");
                        receivedData = receivedData.Replace("RQC,", "");
                        var dataArr = receivedData.Split(',');
                        BlockLayer blockLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[0]);
                        int chunkID = Int32.Parse(dataArr[1]);
                        var sendChunkData = this.LoadChunk(blockLayer, chunkID);
                        var sendChunkStr = $"CKD,{blockLayer},{chunkID},";
                        foreach (int[] chunkLine in sendChunkData)
                            sendChunkStr += string.Join(",", chunkLine) + "\n";
                        var writer = this.driver.BeginSend(NetworkPipeline.Null, this.connectionList[i], out DataStreamWriter dsw);
                        dsw.WriteFixedString4096(new FixedString4096Bytes(sendChunkStr));
                        this.driver.EndSend(dsw);
                    }

                    //プレイヤー一覧の取得要求
                    if (receivedData.StartsWith("RPL")) {
                        var sendPListStr = "PDL,";
                        foreach (PlayerData data in userList.Values)
                            sendPListStr += data.ToString() + "\n";
                        var writer = this.driver.BeginSend(NetworkPipeline.Null, this.connectionList[i], out DataStreamWriter dsw);
                        dsw.WriteFixedString4096(new FixedString4096Bytes(sendPListStr));
                        this.driver.EndSend(dsw);
                    }
                    
                    // プレイヤーのデータ
                    if(receivedData.StartsWith("RPD")) {
                        var writer = this.driver.BeginSend(NetworkPipeline.Null, this.connectionList[i], out DataStreamWriter dsw);
                        dsw.WriteFixedString4096(new FixedString4096Bytes($"PLD,{userList[Guid.Parse(receivedData.Replace("RPD,", ""))]}"));
                        this.driver.EndSend(dsw);
                    }

                    //プレイヤーの移動データ受信 
                    if(receivedData.StartsWith("PMV")) {
                        receivedData = receivedData.Replace("PMV,", "");
                        var dataArr = receivedData.Split(',');
                        PlayerData newPlayer;
                        newPlayer.playerID = Guid.Parse(dataArr[0]);
                        newPlayer.playerLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[1]);
                        newPlayer.playerX = float.Parse(dataArr[2]);
                        newPlayer.playerY = float.Parse(dataArr[3]);
                        newPlayer.actState = Int32.Parse(dataArr[4]);
                        newPlayer.name = userList[newPlayer.playerID].name;
                        newPlayer.skinType = userList[newPlayer.playerID].skinType;
                        userList[newPlayer.playerID] = newPlayer;

                        //全ユーザに移動情報を通知する
                        foreach(NetworkConnection conn in connectionList) {
                            var writer = this.driver.BeginSend(NetworkPipeline.Null, conn, out DataStreamWriter dsw);
                            dsw.WriteFixedString4096(new FixedString4096Bytes($"PLM,{newPlayer.playerID},{newPlayer.playerLayer},{newPlayer.playerX},{newPlayer.playerY},{newPlayer.actState}"));
                            this.driver.EndSend(dsw);
                        }
                    }
                } else if (cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("[SERVER]Disconnected.");
                    this.connectionList[i].Disconnect(driver);
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
        } else {
            var chunkData = LoadDefaultChunk();
            SaveChunk(layerID, chunkId, chunkData);
            return chunkData;
        }
    }

    int[][] LoadDefaultChunk() {
        List<int[]> tempList = new List<int[]>();
        using (var reader = new StreamReader("./default.dat", Encoding.UTF8)) {
            while (0 <= reader.Peek())
                tempList.Add(Array.ConvertAll(reader.ReadLine().Split(','), int.Parse));
            return tempList.ToArray();
        }
    }
}
