using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ShadowBoxClientWrapper : MonoBehaviour {
    public bool debugMode = false;
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

    void Start() {
        // TODO さっさとやれ
        userList = new Dictionary<Guid, PlayerData>();
        entityManager = entityManagerObject.GetComponent<GenericEntityManager>();
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
                if(debugMode) Debug.Log("[WRAPPER]Success to connect.");
                active = true;
                if (!player.Equals(default(PlayerData))) {
                    var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
                    if (writer >= 0) {
                        dsw.WriteFixedString4096(new FixedString4096Bytes("SPD," + player));
                        if(debugMode) Debug.Log(new FixedString4096Bytes("[WRAPPER]Sending user data:\n" + player));
                        this.driver.EndSend(dsw);
                    }
                }
            } else if (cmd == NetworkEvent.Type.Data) {
                String receivedData = ("" + stream.ReadFixedString4096());
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
                    var chunkStr = "";
                    foreach (int[] arrLine in chunkTemp.ToArray())
                        chunkStr += string.Join(",", arrLine) + "\n";

                    if(debugMode) Debug.Log("[WRAPPER]Received chunk data:\n" + chunkStr);
                }
                if(receivedData.StartsWith("PLM")) { //プレイヤーの移動情報を受信したときの処理
                    receivedData = receivedData.Replace("PLM,", "");
                    var dataArr = receivedData.Split(',');
                    Guid playerId = Guid.Parse(dataArr[0]);
                    BlockLayer playerLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[1]);
                    float playerX = float.Parse(dataArr[2]);
                    float playerY = float.Parse(dataArr[3]);
                    int actState = Int32.Parse(dataArr[4]);
                    PlayerData newPlayer;
                    newPlayer.playerID = playerId;
                    newPlayer.playerX = playerX;
                    newPlayer.playerY = playerY;
                    newPlayer.playerLayer = playerLayer;
                    if(!playerId.Equals(player.playerID)) {
                        //そのプレイヤーが現在のローカルデータに存在するか確認し、なければ仮のプレイヤーとして情報を保持
                        //そのままだとまずいので、プレイヤーの一覧を自動的に要求する。←お前ができてなかったんや
                        if (!userList.ContainsKey(playerId) || !entityManager.HasPlayer(playerId)) {
                            newPlayer.name = "Player";
                            newPlayer.skinType = 0;
                            newPlayer.actState = 0;
                            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
                            if (writer >= 0) {
                                dsw.WriteFixedString4096(new FixedString4096Bytes("RPL"));
                                if (debugMode) Debug.Log("[WRAPPER]Requesting player data because this wrapper doesn't have this player data");
                                this.driver.EndSend(dsw);
                            }
                        } else {
                            newPlayer.name = userList[playerId].name;
                            newPlayer.skinType = userList[playerId].skinType;
                            newPlayer.actState = userList[playerId].actState;
                        }
                        userList[playerId] = newPlayer;
                        if(debugMode) Debug.Log($"[WRAPPER]Player {newPlayer.playerID} moving to {newPlayer.playerX}, {newPlayer.playerY}");
                        entityManager.SyncPlayer(playerId, playerX, playerY, (int)playerLayer, actState);
                    } else {
                        if(debugMode) Debug.Log("[WRAPPER]Player move event received but it's same as local player, so skipping it.");
                    }
                    
                }

                if(receivedData.StartsWith("NPD")) { //新規のプレイヤーデータを受信したとき
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
                    if(debugMode) Debug.Log("[WRAPPER]Received new player data\n" + newPlayer.ToString());

                    if (newPlayer.playerID.Equals(player.playerID))
                        if(debugMode) Debug.Log("[WRAPPER]Received data is same as local player, skipping it.");
                    else {
                        //追加する
                        userList[newPlayer.playerID] = newPlayer;

                        entityManager.AddPlayer(newPlayer.playerID, newPlayer.name, newPlayer.skinType);
                        entityManager.SyncPlayer(newPlayer.playerID, newPlayer.playerX, newPlayer.playerY, (int)newPlayer.playerLayer, newPlayer.actState);

                    }
                }

                if (receivedData.StartsWith("PDL")) { //プレイヤー一覧を受信した場合
                    receivedData = receivedData.Replace("PDL,", "");
                    if(debugMode) Debug.Log(receivedData);
                    var dataArr = receivedData.Split('\n');
                    foreach (string playerDataLine in dataArr) {
                        if(playerDataLine != "") {
                            PlayerData newPlayer;
                            var pDataArr = playerDataLine.Split(',');
                            newPlayer.name = pDataArr[0];
                            newPlayer.skinType = Int32.Parse(pDataArr[1]);
                            newPlayer.actState = Int32.Parse(pDataArr[2]);
                            newPlayer.playerID = Guid.Parse(pDataArr[3]);
                            newPlayer.playerX = float.Parse(pDataArr[4]);
                            newPlayer.playerY = float.Parse(pDataArr[5]);
                            newPlayer.playerLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), pDataArr[6]);
                            if(!userList.ContainsKey(newPlayer.playerID)) {

                                entityManager.AddPlayer(newPlayer.playerID, newPlayer.name, newPlayer.skinType);
                                entityManager.SyncPlayer(newPlayer.playerID, newPlayer.playerX, newPlayer.playerY, (int)newPlayer.playerLayer, newPlayer.actState);
                                if(debugMode) Debug.Log("[WRAPPER]Generate new Player");
                            }
                            userList[newPlayer.playerID] = newPlayer;
                        }
                    }
                    if(debugMode) Debug.Log($"[WRAPPER]{dataArr.Length} players data received");
                    if(debugMode) Debug.Log(userList.Values);
                }

                if(receivedData.StartsWith("UDC")) { //他のユーザーが切断したときの処理
                    if(debugMode) Debug.Log($"[WRAPPER]User {receivedData.Replace("UDC,", "")} has disconnected from server");
                    entityManager.OnPlayerDisconnect(Guid.Parse(receivedData.Split(',')[1]));
                }
            } else if (cmd == NetworkEvent.Type.Disconnect) {
                if(debugMode) Debug.Log("[WRAPPER]Disconnect.");
                this.connection = default(NetworkConnection);
            }
        }
    }

    /// <summary>
    /// ドライバと接続情報の破棄を行う
    /// </summary>
    public void OnDestroy() {
        this.connection.Disconnect(driver);
        this.connection.Close(driver);
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
                if(debugMode) Debug.Log("[WRAPPER]Requesting chunk data");
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


            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes("SCH," + layerID + "," + chunkID + "," + sendDataTemp));
                if(debugMode) Debug.Log("[WRAPPER]Sending chunk data:\n" + sendDataTemp);
                this.driver.EndSend(dsw);
            } else return false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// バッファの編集状況を送信する。
    /// </summary>
    /// <param name="workspaceId">バッファの変更を送信するワークスペースのID</param>
    /// <param name="blockLayer">バッファの変更が発生したレイヤー</param>
    /// <param name="chunkId">変更するバッファのチャンクID</param>
    /// <param name="bufferChunkData">変更後のチャンクデータ</param>
    public void SendBuffer(Guid workspaceId, BlockLayer blockLayer, int chunkId, int[][] bufferChunkData) {
        //TODO 実装する
        if (this.connection.IsCreated) {
            string sendDataTemp = "";
            foreach (int[] bufferRow in bufferChunkData)
                sendDataTemp += string.Join(",", bufferRow) + "\n";

            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if(writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes($"SBF,{workspaceId.ToString("N")},{blockLayer},{chunkId},{sendDataTemp}"));
                if(debugMode) Debug.Log("[WRAPPER]Sending buffer data:\n" + sendDataTemp);
                this.driver.EndSend(dsw);
            }

        }
    }


    /// <summary>
    /// 送信済のバッファをチャンクデータに反映、全体送信をする
    /// </summary>
    /// <param name="workspaceId">バッファデータを更新したいチャンクが含まれるワークスペースのID</param>
    /// <param name="layer">更新するバッファの存在するレイヤー</param>
    /// <param name="chunkId">更新するチャンクのID</param>
    public void ApplyBuffer(Guid workspaceId, BlockLayer layer, int chunkId) {
        if(this.connection.IsCreated) {
            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if(writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes($"BRQ,{workspaceId.ToString("N")},{layer},{chunkId}"));
                if(debugMode) Debug.Log("[WRAPPER]Requesting applying buffer data to chunk data");
                this.driver.EndSend(dsw);
            }
        }
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
        if(debugMode) Debug.Log("[WRAPPER]Connect to " + endPoint);
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
            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes("SPD," + player));
                if(debugMode) Debug.Log(new FixedString4096Bytes("[WRAPPER]Sending user data:\n" + player));
                this.driver.EndSend(dsw);
            }
        }
        return player;
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
        if (this.connection.IsCreated) {
            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes($"PMV,{player.playerID},{layer},{x},{y},{actState}"));
                if(debugMode) Debug.Log("[WRAPPER]Sending player move data");
                this.driver.EndSend(dsw);
            } else return false;
            return true;
        } else return false;
    }

    /// <summary>
    /// ブロック単位の変更を送信する。
    /// </summary>
    /// <param name="layer">レイヤー番号</param>
    /// <param name="x">ブロックのX座標</param>
    /// <param name="y">ブロックのY座標</param>
    /// <param name="blockID">変更された後のブロックID</param>
    public void SendBlockChange(BlockLayer layer, int x, int y, int blockID) {
        if (this.connection.IsCreated) {
            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes($"SBC,{layer},{x},{y},{blockID}"));
                if(debugMode) Debug.Log("[WRAPPER]Sending block changing Data");
                this.driver.EndSend(dsw);
            }
        }
    }

    /// <summary>
    /// ワークスペースの情報を送信する。存在するワークスペースの場合は上書きされる。
    /// ワークスペース設定の変更もこのMethodを利用する。
    /// </summary>
    /// <param name="workspace">送信するWorkspace 構造体配列</param>
    public void SendWorkspace(Workspace workspace) {

    }

    /// <summary>
    /// ワークスペースを削除する
    /// </summary>
    /// <param name="removeWorkspaceGuid">削除するワークスペースのGuid</param>
    public void SendWorkspaceRemove(Guid removeWorkspaceGuid) {

    }

    /// <summary>
    /// ワールドに存在するすべてのワークスペースを取得する。
    /// </summary>
    /// <returns>存在するWorkspace 構造体配列(ワークスペースが存在しない場合はnull)</returns>
    public Workspace[]? GetWorkspaces() {
        return null;
    }

    /// <summary>
    /// 接続が有効か確認する。
    /// </summary>
    /// <returns>接続が有効かどうかを示す bool 値</returns>
    public bool IsConnectionActive() {
        return active;
    }

    /// <summary>
    /// 指定したプレイヤーが所有するワークスペースを取得する。
    /// </summary>
    /// <param name="wsOwnerGuid">検索するプレイヤーのGuid</param>
    /// <returns>検索結果としてのWorkspace 構造体配列(ワークスペースが存在しない、プレイヤーが存在しない場合はnull</returns>
    public Workspace[]? GetWorkspacesOfPlayer(Guid wsOwnerGuid) {
        return null;
    }

    /// <summary>
    /// ブロック単位のWorkspaceに発生した変更を通知する。
    /// </summary>
    /// <param name="workspaceGuid">変更が発生したWorkspaceのGuid</param>
    /// <param name="layer">変更が発生したEditBufferレイヤー</param>
    /// <param name="relativeX">EditBufferの左上を起点とした変更点の相対座標X</param>
    /// <param name="relativeY">EditBufferの左上を起点とした変更点の相対座標Y</param>
    /// <param name="blockID">変更先のブロックID</param>
    public void SendEditBufferBlockChange(Guid workspaceGuid, BlockLayer layer, int relativeX, int relativeY, int blockID) {
        if (this.connection.IsCreated) {
            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes($"BBC,{workspaceGuid.ToString("N")},{layer},{relativeX},{relativeY},{blockID}"));
                if(debugMode) Debug.Log("[WRAPPER]Sending buffer block data:\n" + $"BBC,{workspaceGuid.ToString("N")},{layer},{relativeX},{relativeY},{blockID}");
                this.driver.EndSend(dsw);
            }

        }
    }

    /// <summary>
    /// 手動でバッファの更新状況を取得する。
    /// </summary>
    /// <param name="workspaceGuid">変更を取得するWorkspaceのGuid</param>
    /// <param name="layer">変更を取得するWorkspaceのレイヤー</param>
    /// <returns>キャッシュされていない場合はnullを返し、取得要求を行う</returns>
    public int[][]? GetEditBufferManual(Guid workspaceGuid, BlockLayer layer) {
        return null;
    }
}