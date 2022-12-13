using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;

public class ShadowBoxServer : MonoBehaviour {
    public bool debugMode = false;
    public GameObject terrainGeneratorObj;

    private GenerateTerrain terrainGenerator;
    /// <summary>
    /// 最後の通信からこの時間が経過した場合、切断とみなす時間
    /// </summary>
    public float timeout = 0.5f;
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
    private Dictionary<int, Guid> guidConnectionList;
    private Dictionary<int, float> lastCommandSend;
    private bool isWorldGenerated = false;
    private bool active = false;
    private WorldInfo worldInfo;
    // Start is called before the first frame update
    void Start() {
        userList = new Dictionary<Guid, PlayerData>();
        guidConnectionList = new Dictionary<int, Guid>();
        lastCommandSend = new Dictionary<int, float>();
        terrainGenerator = terrainGeneratorObj.GetComponent<GenerateTerrain>();
        if ((worldInfo = WorldInfo.LoadWorldData()) != null) {
            isWorldGenerated = true;
        }
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
            if (debugMode) Debug.LogError("[SERVER]Failed to bind port " + port + ".");
            return false;
        } else this.driver.Listen();
        if (debugMode) Debug.Log("[SERVER]Listen on " + port);
        active = true;
        return true;
    }

    //TODO
    void GenerateWorld(int width, int height, int chunkWidth, int chunkHeight, int heightRange, int seed) {

        // SaveChunk(BlockLayer.OutsideWall, 96, terrainGenerator.Generate(1, 64, 64, 6, new System.Random().Next(0, Int32.MaxValue))[0]);
        int chunkNum = 0;
        foreach (int[][] chunkData in terrainGenerator.Generate(width, height, chunkWidth, chunkHeight, heightRange, seed)) {
            SaveChunk(BlockLayer.InsideWall, chunkNum, chunkData);
            SaveChunk(BlockLayer.InsideBlock, chunkNum, chunkData);
            SaveChunk(BlockLayer.OutsideWall, chunkNum, chunkData);
            SaveChunk(BlockLayer.OutsideBlock, chunkNum, chunkData);
            chunkNum++;
        }
    }

    /// <summary>
    /// 座標からチャンク番号を返す
    /// </summary>
    /// <param name="x">計算する座標X</param>
    /// <param name="y">計算する座標Y</param>
    /// <param name="chunkWidth">チャンク当たりの横ブロック数</param>
    /// <param name="chunkHeight">チャンク当たりの縦ブロック数</param>
    /// <param name="worldXChunkSize">ワールドの横幅(チャンク単位)</param>
    /// <returns>チャンク番号</returns>
    int CoordinateToChunkNo(int x, int y, int chunkWidth, int chunkHeight, int worldXChunkSize) {
        int chunkNum = (x / chunkWidth) + worldXChunkSize * (y / chunkHeight);
        return chunkNum;
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

        // なんかDisconnectイベントを拾ってくれないので一定時間何のパケットも送信しなかった時切断とみなすよう変更。
        // 原因不明の為少し不安要素。まあいいや。
        //if(debugMode) Debug.Log(string.Join(",", lastCommandSend.Values));
        foreach (int connectionId in new List<int>(lastCommandSend.Keys)) {
            if (lastCommandSend[connectionId] >= timeout) {
                foreach (NetworkConnection conn in connectionList) {
                    if (conn.IsCreated) {
                        if (guidConnectionList.ContainsKey(conn.InternalId) && conn.InternalId != connectionId)
                            Send(conn, $"UDC,{guidConnectionList[connectionId]}");

                    }
                }

                if (debugMode) Debug.Log($"[Server]User {guidConnectionList[connectionId]} has disconnected.");
                userList.Remove(guidConnectionList[connectionId]);
                lastCommandSend.Remove(connectionId);
                guidConnectionList.Remove(connectionId);
                this.connectionList[connectionId].Disconnect(driver);
                this.connectionList[connectionId] = default(NetworkConnection);
            } else
                lastCommandSend[connectionId] += Time.deltaTime;
        }

        DataStreamReader stream;
        for (int i = 0; i < this.connectionList.Length; i++) {
            //コネクションが作成されているかどうか
            if (this.connectionList[i].IsCreated) {
                Assert.IsTrue(this.connectionList[i].IsCreated);

                NetworkEvent.Type cmd;
                while ((cmd = this.driver.PopEventForConnection(this.connectionList[i], out stream)) != NetworkEvent.Type.Empty) {
                    if (cmd == NetworkEvent.Type.Connect) {
                        if (debugMode) Debug.Log("[SERVER]User Connected.");
                    } else if (cmd == NetworkEvent.Type.Data) {
                        //データを受信したとき
                        String receivedData = ("" + stream.ReadFixedString4096());
                        if (stream.HasFailedReads) Debug.Log("[SERVER]Failed to read data");
                        lastCommandSend[this.connectionList[i].InternalId] = 0.0f;
                        if (receivedData.StartsWith("SCH")) { //チャンクを受信したとき
                            receivedData = receivedData.Replace("SCH,", "");
                            if (debugMode) Debug.Log("[SERVER]Receive chunk data: \n" + receivedData);
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
                            if (debugMode) Debug.Log("[SERVER]Recieve new user data: " + userList[newPlayer.playerID].ToString());

                            //当該プレイヤーとNetworkConnectionを紐づけする
                            guidConnectionList[this.connectionList[i].InternalId] = newPlayer.playerID;

                            //プレイヤー情報を周知する
                            foreach (NetworkConnection conn in connectionList)
                                Send(conn, $"NPD,{newPlayer.ToString()}");

                            //当該ユーザに現在のユーザ一覧を送信する
                            var sendPListStr = "PDL,";
                            foreach (PlayerData data in userList.Values)
                                sendPListStr += data.ToString() + "\n";
                            Send(connectionList[i], sendPListStr);
                            if (debugMode) Debug.Log("[SERVER]USERS:" + string.Join(",", userList.Values));
                        }

                        //送出
                        if (receivedData.StartsWith("RQC")) { //チャンク要求を受け取った時
                            if (debugMode) Debug.Log("[SERVER]Recieve chunk request from client");
                            receivedData = receivedData.Replace("RQC,", "");
                            var dataArr = receivedData.Split(',');
                            BlockLayer blockLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[0]);
                            int chunkID = Int32.Parse(dataArr[1]);
                            var sendChunkData = isWorldGenerated ? LoadChunk(blockLayer, chunkID) : LoadDefaultChunk();
                            var sendChunkStr = $"CKD,{blockLayer},{chunkID},";
                            foreach (int[] chunkLine in sendChunkData)
                                sendChunkStr += string.Join(",", chunkLine) + "\n";
                            if (debugMode) Debug.Log($"[SERVER]Sending data {sendChunkStr}");
                            Send(connectionList[i], sendChunkStr);
                        }

                        if (receivedData.StartsWith("WGC")) {
                            Send(connectionList[i], $"WST,{isWorldGenerated}");
                        }

                        //プレイヤー一覧の取得要求
                        if (receivedData.StartsWith("RPL")) {
                            var sendPListStr = "PDL,";
                            foreach (PlayerData data in userList.Values)
                                sendPListStr += data.ToString() + "\n";
                            Send(connectionList[i], sendPListStr);
                            if (debugMode) Debug.Log("USERS:" + string.Join(",", userList.Values));
                        }

                        // プレイヤーのデータ
                        if (receivedData.StartsWith("RPD"))
                            Send(connectionList[i], $"PLD,{userList[Guid.Parse(receivedData.Replace("RPD,", ""))]}");

                        //プレイヤーの移動データ受信 
                        if (receivedData.StartsWith("PMV")) {
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

                            //仮 ログ出力
                            // if (debugMode) Debug.Log($"[SERVER]Player {userList[newPlayer.playerID].name} moving to {newPlayer.playerX}, {newPlayer.playerY}");

                            //自身を除くユーザに移動情報を通知する
                            int senderConnectionID = connectionList[i].InternalId;
                            foreach (NetworkConnection conn in connectionList)
                                if (conn.InternalId != senderConnectionID)
                                    Send(conn, $"PLM,{newPlayer.playerID},{newPlayer.playerLayer},{newPlayer.playerX},{newPlayer.playerY},{newPlayer.actState}");
                        }

                        //ブロック単位の更新を受け取った時のやつ
                        if (receivedData.StartsWith("SBC")) {
                            receivedData = receivedData.Replace("SBC,", "");
                            var dataArr = receivedData.Split(',');
                            BlockLayer layer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[0]);
                            int x = Int32.Parse(dataArr[1]);
                            int y = Int32.Parse(dataArr[2]);
                            int blockId = Int32.Parse(dataArr[3]);
                            if (worldInfo != null) {
                                int chunkNum = CoordinateToChunkNo(x, y, worldInfo.GetChunkSizeX(), worldInfo.GetChunkSizeY(), worldInfo.GetWorldSizeX());
                                var oldChunk = LoadChunk(layer, chunkNum);
                                oldChunk[x % worldInfo.GetChunkSizeX()][y % worldInfo.GetWorldSizeY()] = blockId;
                                SaveChunk(layer, chunkNum, oldChunk);
                            }

                            //全ユーザに移動情報を通知
                            foreach (NetworkConnection conn in connectionList)
                                Send(conn, $"BCB,{receivedData}");
                        }

                        //ワールドのconfigを設定するやつ
                        if (receivedData.StartsWith("SWD")) {
                            receivedData = receivedData.Replace("SWD,", "");
                            var dataArr = receivedData.Split(',');
                            int worldSizeX = Int32.Parse(dataArr[0]);
                            int worldSizeY = Int32.Parse(dataArr[1]);
                            int chunkSizeX = Int32.Parse(dataArr[2]);
                            int chunkSizeY = Int32.Parse(dataArr[3]);
                            int heightRange = Int32.Parse(dataArr[4]);
                            int seed = Int32.Parse(dataArr[5]);
                            string worldName = dataArr[6];

                            worldInfo = new WorldInfo(worldSizeX, worldSizeY, chunkSizeX, chunkSizeY, heightRange, seed, worldName);
                            worldInfo.SaveWorldData();
                            if (debugMode) Debug.Log("[SERVER]World regenerate complete.");
                            foreach (NetworkConnection conn in connectionList)
                                Send(conn, "RCP");
                        }

                        //ワールドを再生成するやつ
                        if (receivedData.StartsWith("RGN")) {
                            if (debugMode) Debug.Log("[SERVER]Start world regenerate...");
                            if (worldInfo != null) {
                                GenerateWorld(worldInfo.GetWorldSizeX(), worldInfo.GetWorldSizeY(), worldInfo.GetChunkSizeX(), worldInfo.GetChunkSizeY(), worldInfo.GetHeightRange(), worldInfo.GetSeed());
                            } else {
                                Send(connectionList[i], "FGN");
                            }
                        }

                        // 切断処理...なんでDisconnectイベント拾ってくれないんや！
                        if (receivedData.StartsWith("DCN")) {
                            if (debugMode) Debug.Log("[SERVER]User disconnected.");
                        }
                    } else {
                        Debug.Log("Unknown Event");
                    }
                }
            }

        }
    }

    //なんで作らなかったんだ？
    private bool Send(NetworkConnection connection, string sendData) {
        if (connection.IsCreated) {
            var sendDataFS4096 = new FixedString4096Bytes(sendData);
            //if(debugMode) Debug.Log($"[SERVER]Sending {System.Text.ASCIIEncoding.Unicode.GetByteCount(sendDataFS4096.ToString())} bytes data");
            var writer = this.driver.BeginSend(NetworkPipeline.Null, connection, out DataStreamWriter dsw);
            dsw.WriteFixedString4096(sendDataFS4096);
            this.driver.EndSend(dsw);
            if (dsw.HasFailedWrites) {
                Debug.LogWarning($"[SERVER]Failed to sending data:\n{sendData}");
                return false;
            }
            return true;
        } else return false;
    }

    /// <summary>
    /// 内部サーバー(127.0.0.1:11781)を作成する
    /// </summary>
    public void CreateInternalServer() {
        StartServer(11781);
    }

#nullable enable
    /// <summary>
    /// レイヤーデータをファイルから読み込む
    /// </summary>
    /// <param name="layerID">読み込むレイヤーのID</param>
    /// <param name="chunkID">読み込むチャンクのID</param>
    /// <returns></returns>
    public int[][]? LoadChunk(BlockLayer layerID, int chunkID) {
        Debug.Log($"Loading ./worlddata/{layerID}.chunk{chunkID}.dat");
        string fileName = $"./worlddata/{layerID}.chunk{chunkID}.dat";
        if (File.Exists(fileName)) {
            List<int[]> tempList = new List<int[]>();
            using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
                while (0 <= reader.Peek())
                    tempList.Add(Array.ConvertAll(reader.ReadLine().Split(','), int.Parse));
                return tempList.ToArray();
            }
        } else {
            var chunkData = LoadDefaultChunk();
            SaveChunk(layerID, chunkID, chunkData);
            return chunkData;
        }
    }

    /// <summary>
    /// レイヤーデータを保存する
    /// </summary>
    /// <param name="layerID">レイヤーのID</param>
    /// <param name="chunkID">チャンクのID</param>
    /// <param name="chunkData">保存するデータ</param>
    void SaveChunk(BlockLayer layerID, int chunkID, int[][] chunkData) {
        if (!Directory.Exists("./worlddata/"))
            Directory.CreateDirectory("./worlddata");
        string fileName = $"./worlddata/{layerID}.chunk{chunkID}.dat";
        using (var writer = new StreamWriter(fileName, false, Encoding.UTF8)) {
            foreach (int[] row in chunkData)
                writer.WriteLine(string.Join(",", row));
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
