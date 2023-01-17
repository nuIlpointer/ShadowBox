using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using static GenericEntityManager;

public class ShadowBoxClientWrapper : MonoBehaviour {
    public bool debugMode = false;
    public GameObject textObj;
    public GameObject worldLoaderObj;

    private bool worldGenerated = true;
    private WorldLoader worldLoader;
    private LogToDisplay log;
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
        public override string ToString() => $"{name},{skinType},{actState},{playerID.ToString("N")},{playerX},{playerY},{playerLayer}";
    }

    public struct EditBuffer {
        Guid workspaceID;
        int[][] editBufferLayer1;
        int[][] editBufferLayer2;
        int[][] editBufferLayer3;
        int[][] editBufferLayer4;
    }

    public struct Workspace {
        string name;
        Guid workspaceID;
        Guid wsOwnerID;
        Guid[] editablePlayerID;
        int x1;
        int y1;
        int x2;
        int y2;
        EditBuffer buffer;
        bool inEdit;
        public override string ToString() => $"{name},{workspaceID.ToString("N")},{wsOwnerID.ToString("N")},!{string.Join(",", editablePlayerID)}!,{x1},{y1},{x2},{y2},{inEdit}";
    }

    public GameObject entityManagerObject;

    private Dictionary<Guid, PlayerData> userList;
    private IPAddress connectAddress;
    private int connectPort;
    private NetworkDriver driver;
    private NetworkEndPoint endPoint;
    private NetworkConnection connection;
    private bool active = false;
    private PlayerData player;
    private GenericEntityManager entityManager;
    private bool worldRegenFinish = false;
    void Start() {
        // TODO さっさとやれ
        userList = new Dictionary<Guid, PlayerData>();
        entityManager = entityManagerObject.GetComponent<GenericEntityManager>();
        log = textObj.GetComponent<LogToDisplay>();
        worldLoader = worldLoaderObj.GetComponent<WorldLoader>();
    }

    // Update is called once per frame
    void Update() {
        // TODO さっさとやれ
        this.driver.ScheduleUpdate().Complete();

        if (!this.connection.IsCreated)
            return;

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = this.connection.PopEvent(this.driver, out stream)) != NetworkEvent.Type.Empty) {
            if (cmd == NetworkEvent.Type.Connect) {
                if (debugMode) Debug.Log("[WRAPPER]Success to connect.");
                log.addText("接続しました。");
                active = true;
                Send(connection, "WGC");
                if (!player.Equals(default(PlayerData)))
                    Send(connection, $"SPD,{player}");
            } else if (cmd == NetworkEvent.Type.Data) {
                String receivedData = ("" + stream.ReadFixedString4096());
                if (stream.HasFailedReads) Debug.Log("[WRAPPER]Failed to read data");
                if (debugMode) Debug.Log($"[WRAPPER]Received data: {receivedData}");
                if (receivedData.StartsWith("CKD")) { //チャンクデータを受信したときの処理
                    receivedData = receivedData.Replace("CKD,", "");
                    var dataArr = receivedData.Split(',');
                    var blockLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[0]);
                    var chunkID = Int32.Parse(dataArr[1]);
                    receivedData = receivedData.Replace($"{blockLayer},{chunkID},", "");
                    List<int[]> chunkTemp = new List<int[]>();
                    foreach (String line in receivedData.Split('\n'))
                        if (line != "")
                            chunkTemp.Add(Array.ConvertAll(line.Split(','), int.Parse));
                    try {
                        worldLoader.ChunkUpdate(chunkTemp.ToArray(), (int)blockLayer, chunkID);
                    } catch(Exception) {
                        Debug.LogError($"[WRAPPER]Failed to run chunk update in {blockLayer},{chunkID}.");
                    };

                    //デバッグ表示用
                    var chunkStr = "";
                    foreach (int[] arrLine in chunkTemp.ToArray())
                        chunkStr += string.Join(",", arrLine) + "\n";
                    if (debugMode) Debug.Log($"[WRAPPER]Received chunk data of {blockLayer}.{chunkID}:\n{chunkStr}");
                }
                if (receivedData.StartsWith("PLM")) { //プレイヤーの移動情報を受信したときの処理
                    receivedData = receivedData.Replace("PLM,", "");
                    var dataArr = receivedData.Split(',');
                    PlayerData newPlayer;
                    newPlayer.name = dataArr[0];
                    newPlayer.skinType = Int32.Parse(dataArr[1]);
                    newPlayer.actState = Int32.Parse(dataArr[2]);
                    newPlayer.playerID = Guid.Parse(dataArr[3]);
                    newPlayer.playerX = float.Parse(dataArr[4]);
                    newPlayer.playerY = float.Parse(dataArr[5]);
                    newPlayer.playerLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[6]);

                    Guid playerId = newPlayer.playerID;
                    if (!playerId.Equals(player.playerID)) {
                        userList[playerId] = newPlayer;
                        if (!entityManager.HasPlayer(playerId))
                            entityManager.AddPlayer(newPlayer.playerID, newPlayer.name, newPlayer.skinType);

                        entityManager.SyncPlayer(newPlayer.playerID, newPlayer.playerX, newPlayer.playerY, (int)newPlayer.playerLayer, newPlayer.actState);
                    } else {
                        if (debugMode) Debug.Log("[WRAPPER]Player move event received but it's same as local player, so skipping it.");
                    }

                }

                if (receivedData.StartsWith("NPD")) { //新規のプレイヤーデータを受信したとき
                    receivedData = receivedData.Replace("NPD,", "");
                    var dataArr = receivedData.Split(',');
                    PlayerData newPlayer;
                    newPlayer.name = dataArr[0];
                    newPlayer.skinType = Int32.Parse(dataArr[1]);
                    newPlayer.actState = Int32.Parse(dataArr[2]);
                    newPlayer.playerID = Guid.Parse(dataArr[3]);
                    newPlayer.playerX = float.Parse(dataArr[4]);
                    newPlayer.playerY = float.Parse(dataArr[5]);
                    newPlayer.playerLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[6]);
                    if (debugMode) Debug.Log("[WRAPPER]Received new player data\n" + newPlayer.ToString());

                    if (newPlayer.playerID.Equals(player.playerID)) {
                        if (debugMode) Debug.Log("[WRAPPER]Received data is same as local player, skipping it.");
                    } else {
                        //追加する
                        userList[newPlayer.playerID] = newPlayer;
                        if (!this.player.playerID.Equals(newPlayer.playerID)) {
                            log.addText($"{newPlayer.name}さんが接続しました。");
                        }
                        entityManager.AddPlayer(newPlayer.playerID, newPlayer.name, newPlayer.skinType);
                        Debug.Log($"add new player {newPlayer}");
                        entityManager.SyncPlayer(newPlayer.playerID, newPlayer.playerX, newPlayer.playerY, (int)newPlayer.playerLayer, newPlayer.actState);

                    }
                }

                if (receivedData.StartsWith("PDL")) { //プレイヤー一覧を受信した場合
                    receivedData = receivedData.Replace("PDL,", "");
                    if (debugMode) Debug.Log(receivedData);
                    var dataArr = receivedData.Split('\n');
                    foreach (string playerDataLine in dataArr) {
                        if (playerDataLine != "") {
                            PlayerData newPlayer;
                            var pDataArr = playerDataLine.Split(',');
                            newPlayer.name = pDataArr[0];
                            newPlayer.skinType = Int32.Parse(pDataArr[1]);
                            newPlayer.actState = Int32.Parse(pDataArr[2]);
                            newPlayer.playerID = Guid.Parse(pDataArr[3]);
                            newPlayer.playerX = float.Parse(pDataArr[4]);
                            newPlayer.playerY = float.Parse(pDataArr[5]);
                            newPlayer.playerLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), pDataArr[6]);
                            if (!userList.ContainsKey(newPlayer.playerID) && !player.playerID.Equals(newPlayer.playerID)) {
                                entityManager.AddPlayer(newPlayer.playerID, newPlayer.name, newPlayer.skinType);
                                entityManager.SyncPlayer(newPlayer.playerID, newPlayer.playerX, newPlayer.playerY, (int)newPlayer.playerLayer, newPlayer.actState);
                                if (debugMode) Debug.Log("[WRAPPER]Generate new Player");
                            }
                            userList[newPlayer.playerID] = newPlayer;
                        }
                    }
                }


                if(receivedData.StartsWith("FGN"))
                    Debug.LogWarning("[WRAPPER]Failed to regenerate World");

                if (receivedData.StartsWith("RCP")) {
                    if (debugMode) Debug.Log("[WRAPPER]World regenerate finished.");
                    worldLoader.OnWorldRegenerateFinish();
                    worldRegenFinish = true;
                }

                if (receivedData.StartsWith("BCB")) {
                    receivedData = receivedData.Replace("BCB,", "");
                    var dataArr = receivedData.Split(',');
                    try {
                        worldLoader.BlockUpdate(Int32.Parse(dataArr[3]), (int)(BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[0]), Int32.Parse(dataArr[1]), Int32.Parse(dataArr[2]));
                    } catch(Exception) {
                        Debug.LogError($"[WRAPPER]Failed to run BlockUpdate in {dataArr[0]},{dataArr[1]},{dataArr[2]}");
                    }
                }

                if (receivedData.StartsWith("UDC")) { //他のユーザーが切断したときの処理
                    Guid targetId = Guid.Parse(receivedData.Split(',')[1]);
                    if (debugMode) Debug.Log($"[WRAPPER]User {receivedData.Replace("UDC,", "")} has disconnected from server");
                    log.addText($"{userList[targetId].name}さんが切断しました。");
                    entityManager.OnPlayerDisconnect(targetId);
                    userList.Remove(targetId);
                }

                if(receivedData.StartsWith("WST")) {
                    worldGenerated = Boolean.Parse(receivedData.Split(',')[1]);
                    if (!worldGenerated) worldLoader.OnWorldNeedRegenerate();
                    else worldLoader.OnWorldNoNeedRegenerate();
                }
            } else if (cmd == NetworkEvent.Type.Disconnect) {
                if (debugMode) Debug.Log("[WRAPPER]Disconnect.");
                this.connection = default(NetworkConnection);
            }
        }
    }

    /// <summary>
    /// ドライバと接続情報の破棄を行う
    /// </summary>
    public void OnDestroy() {
        var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
        if (writer >= 0) {
            dsw.WriteFixedString4096(new FixedString4096Bytes("DCN"));
            if (debugMode) Debug.Log("[WRAPPER]Sending Disconnect event");
            this.driver.EndSend(dsw);
        }

        this.driver.Dispose();
    }

    /// <summary>
    /// 現在接続しているサーバーへチャンクデータを要求する
    /// </summary>
    /// <param name="layerID">要求するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkID">要求するチャンク</param>
    /// <returns>送信に成功したかを示すbool値</returns>
    public bool GetChunk(BlockLayer layerID, int chunkID) {
        if (this.connection.IsCreated) {
            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes($"RQC,{layerID},{chunkID}"));
                if (debugMode) Debug.Log($"[WRAPPER]Requesting chunk data in {layerID}.{chunkID}")  ; 
                this.driver.EndSend(dsw);
            } else return false;
            return true;
        } else return false;
    }

    /// <summary>
    /// チャンクを現在接続しているサーバーに送信する
    /// </summary>
    /// <param name="layerID">送信するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkID">送信するチャンクの場所</param>
    /// <param name="chunkData">送信するチャンク情報</param>
    /// <returns>送信に成功したか</returns>
    public bool SendChunk(BlockLayer layerID, int chunkID, int[][] chunkData) {
        if (this.connection.IsCreated) {
            string sendDataTemp = "";
            foreach (int[] chunkRow in chunkData)
                for (int i = 0; i < chunkRow.Length; i++)
                    sendDataTemp += chunkRow[i] + (i == chunkRow.Length - 1 ? "\n" : ","); //ここもstring.Joinに統一したほうがよさげ？
            return Send(connection, $"SCH,{layerID},{chunkID},{sendDataTemp}");
        }
        return false;
    }

    /// <summary>
    /// 接続先のポート/IPアドレスを指定し、接続する。
    /// ポートが範囲外の時は自動的に「11781」。
    /// </summary>
    /// <param name="ipAddress">接続先IPアドレス。</param>
    /// <param name="port">接続先ポート番号。デフォルトは11781。</param>
    public void Connect(string ipAddress, int port) {
        this.driver = NetworkDriver.Create();
        this.connectAddress = IPAddress.Parse(ipAddress);
        this.endPoint = NetworkEndPoint.AnyIpv4;
        using (var rawIpAddr = new NativeArray<byte>(this.connectAddress.GetAddressBytes().Length, Allocator.TempJob)) {
            rawIpAddr.CopyFrom(connectAddress.GetAddressBytes());
            this.endPoint.SetRawAddressBytes(rawIpAddr);
        }

        endPoint.Port = (ushort)port;
        this.connectPort = port;
        this.connection = this.driver.Connect(endPoint);
        active = true;
        if (debugMode) Debug.Log("[WRAPPER]Connect to " + endPoint);
    }

    /// <summary>
    /// サーバーにプレイヤー情報を送信する。接続完了前に実行された場合、完了時に送信される。
    /// </summary>
    /// <param name="name">他人に表示される名前</param>
    /// <param name="skinID">他人から表示される見た目</param>
    /// <param name="actState">表示の詳細情報？</param>
    /// <param name="playerX">スポーンするX座標</param>
    /// <param name="playerY">スポーンするY座標</param>
    /// <param name="blockLayer">スポーン先のBlockLayer</param>
    /// <returns>サーバーに登録されるPlayerData</returns>
    public PlayerData SetPlayerData(string name, int skinID, int actState, float playerX, float playerY, BlockLayer blockLayer) {
        // ラッパーに対応するPlayerDataを設定
        player.name = name;
        player.skinType = skinID;
        player.actState = actState;
        player.playerID = Guid.NewGuid();
        player.playerX = playerX;
        player.playerY = playerY;
        player.playerLayer = blockLayer;

        //接続が完了している場合、即時送信する(完了していない場合は接続時に一括処理)
        if (active) {
            Send(connection, $"SPD,{player}");
        }
        return player;
    }

    /// <summary>
    /// サーバーにワールドの設定を送信する
    /// </summary>
    /// <param name="worldSizeX">ワールドの横チャンク数</param>
    /// <param name="worldSizeY">ワールドの縦チャンク数</param>
    /// <param name="chunkSizeX">チャンクあたりの横ブロック数</param>
    /// <param name="chunkSizeY">チャンクあたりの縦ブロック数</param>
    /// <param name="heightRange">地形生成の高低差</param>
    /// <param name="seed">ワールドのシード値</param>
    /// <param name="worldName">ワールドの名前</param>
    /// <returns></returns>
    public bool SetWorldData(int worldSizeX, int worldSizeY, int chunkSizeX, int chunkSizeY, int heightRange, int seed, string worldName) {
        if (this.connection.IsCreated) {
            string sendDataTemp = $"SWD,{worldSizeX},{worldSizeY},{chunkSizeX},{chunkSizeY},{heightRange},{seed},{worldName}";
            Debug.Log($"SWD,{worldSizeX},{worldSizeY},{chunkSizeX},{chunkSizeY},{heightRange},{seed},{worldName}");
            return Send(this.connection, sendDataTemp);
        }
        return false;
    }

    public bool RequestWorldRegenerate() {
        return worldGenerated = Send(this.connection, "RGN");
    }

    public bool IsWorldRegenerateFinished() {
        return worldRegenFinish;
    }

    public bool GetWorldGenerated() {
        return worldGenerated;
    }

    private bool Send(NetworkConnection c, string sendData) {
        if (this.connection.IsCreated) {
            var writer = this.driver.BeginSend(c, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes(sendData));
                this.driver.EndSend(dsw);
                return true;
            } else return false;
        } else return false;
    }
#nullable enable
    /// <summary>
    /// 接続中のプレイヤーをすべて取得する。SetPlayerData()の未実行等で登録がない場合はnull。
    /// </summary>
    /// <returns>接続中のプレイヤーが含まれる PlayerData 配列</returns>
    public PlayerData[]? GetPlayers() {
        return userList.Values.ToArray();
    }

    /// <summary>
    /// プレイヤーの情報を取得する。指定したGuidのプレイヤーが存在しない場合はnull。
    /// </summary>
    /// <param name="playerID">情報を取得するPlayerID。</param>
    /// <returns></returns>
    public PlayerData? GetPlayer(Guid playerID) {
        return userList.ContainsKey(playerID) ? userList[playerID] : new PlayerData();
    }

    /// <summary>
    /// プレイヤーの移動情報を送信する。
    /// </summary>
    /// <param name="layer">プレイヤーが存在するレイヤー</param>
    /// <param name="x">プレイヤーのX座標</param>
    /// <param name="y">プレイヤーのY座標</param>
    /// <param name="actState">プレイヤーの詳細情報？</param>
    public bool SendPlayerMove(BlockLayer layer, float x, float y, int actState) {
        player.playerX = x;
        player.playerY = y;
        player.playerLayer = layer;
        player.actState = actState; 
        return Send(this.connection, $"PMV,{player.ToString()}");
    }

    /// <summary>
    /// ブロック単位の変更を送信する。
    /// </summary>
    /// <param name="layer">レイヤー番号</param>
    /// <param name="x">ブロックのX座標</param>
    /// <param name="y">ブロックのY座標</param>
    /// <param name="blockID">変更された後のブロックID</param>
    public bool SendBlockChange(BlockLayer layer, int x, int y, int blockID) {
        return Send(this.connection, $"SBC,{layer},{x},{y},{blockID}");
    }
    /// <summary>
    /// 接続が有効か確認する。
    /// </summary>
    /// <returns>接続が有効かどうかを示す bool 値</returns>
    public bool IsConnectionActive() {
        return active;
    }
}