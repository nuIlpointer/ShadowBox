using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ShadowBoxClientWrapper : MonoBehaviour {
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
        public override string ToString() => $"{name},{skinType},{playerID.ToString("N")},{playerX},{playerY},{playerLayer}";
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
    }

    private IPAddress connectAddress;
    private int connectPort;
    private NetworkDriver driver;
    private NetworkEndPoint endPoint;
    private NetworkConnection connection;
    private bool active = false;
    private PlayerData player;
    void Start() {
        // TODO さっさと�?�?
    }

    // Update is called once per frame
    void Update() {
        // TODO さっさと�?�?
        this.driver.ScheduleUpdate().Complete();

        if (!this.connection.IsCreated)
            return;

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = this.connection.PopEvent(this.driver, out stream)) != NetworkEvent.Type.Empty) {
            if (cmd == NetworkEvent.Type.Connect) {
                Debug.Log("[WRAPPER]Success to connect.");
                active = true;
                if (!player.Equals(default(PlayerData))) {
                    var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
                    if (writer >= 0) {
                        dsw.WriteFixedString4096(new FixedString4096Bytes("SPD," + player));
                        Debug.Log(new FixedString4096Bytes("[WRAPPER]Sending user data:\n" + player));
                        this.driver.EndSend(dsw);
                    }
                }
            } else if (cmd == NetworkEvent.Type.Data) {
                String receivedData = ("" + stream.ReadFixedString4096());
                if(receivedData.StartsWith("CKD")) {
                    receivedData = receivedData.Replace("CKD,", "");
                    var dataArr = receivedData.Split(',');
                    var blockLayer = (BlockLayer)Enum.Parse(typeof(BlockLayer), dataArr[0]);
                    var chunkID = Int32.Parse(dataArr[1]);
                    receivedData = receivedData.Replace($"{blockLayer},{chunkID},", "");
                    List<int[]> chunkTemp = new List<int[]>();
                    foreach (String line in receivedData.Split('\n'))
                        if(line != "")
                            chunkTemp.Add(Array.ConvertAll(line.Split(','), int.Parse));
                    Debug.Log("[WRAPPER]Received chunk data:\n");
                    foreach (int[] arrLine in chunkTemp.ToArray())
                        Debug.Log(string.Join(",", arrLine));
                }
            } else if (cmd == NetworkEvent.Type.Disconnect) {
                Debug.Log("[WRAPPER]Disconnect.");
                this.connection = default(NetworkConnection);
            }
        }
    }

    /// <summary>
    /// ドライバと接続情報の破�?を行う
    /// </summary>
    public void OnDestroy() {
        this.driver.Dispose();
    }

    /// <summary>
    /// 現在接続して�?るサーバ�?�へチャンク�?ータを要求す�?
    /// </summary>
    /// <param name="layerID">要求するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkID">要求するチャンク</param>
    /// <returns>送信に成功したかを示すbool値</returns>
    public bool GetChunk(BlockLayer layerID, int chunkID) {
        if (this.connection.IsCreated) {
            this.driver.ScheduleUpdate().Complete();
            if (!this.connection.IsCreated)
                return false;
            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes($"RQC,{layerID},{chunkID}"));
                Debug.Log("[WRAPPER]Requesting chunk data");
                this.driver.EndSend(dsw);
            } else return false;
            return true;
        } else return false;
    }

    /// <summary>
    /// チャンクを現在接続して�?るサーバ�?�に送信する
    /// </summary>
    /// <param name="layerID">送信するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkID">送信するチャンクの場所</param>
    /// <param name="chunkData">送信するチャンク�?報</param>
    /// <returns>送信に成功した�?</returns>
    public bool SendChunk(BlockLayer layerID, int chunkID, int[][] chunkData) {
        if (this.connection.IsCreated) {
            this.driver.ScheduleUpdate().Complete();
            if (!this.connection.IsCreated)
                return false;
            string sendDataTemp = "";
            foreach (int[] chunkRow in chunkData)
                for (int i = 0; i < chunkRow.Length; i++)
                    sendDataTemp += chunkRow[i] + (i == chunkRow.Length - 1 ? "\n" : ",");


            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes("SCH," + layerID + "," + chunkID + "," + sendDataTemp));
                Debug.Log("[WRAPPER]Sending chunk data:\n" + sendDataTemp);
                this.driver.EndSend(dsw);
            } else return false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 接続�?��?�ポ�?��?/IPアドレスを指定し、接続する�?
    /// ポ�?�トが�?囲外�?�時�?�自動的に�?11781」�?
    /// </summary>
    /// <param name="ipAddress">接続�??IPアドレス�?</param>
    /// <param name="port">接続�?��?��?�ト番号。デフォルト�?�11781�?</param>
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
        Debug.Log("[WRAPPER]Connect to " + endPoint);
    }

    /// <summary>
    /// サーバ�?�にプレイヤー�?報を送信する。接続完�?前に実行された場合、完�?時に送信される�?
    /// </summary>
    /// <param name="name">他人に表示される名�?</param>
    /// <param name="skinID">他人から表示される見た目</param>
    /// <param name="playerX">スポ�?�ンするX座�?</param>
    /// <param name="playerY">スポ�?�ンするY座�?</param>
    /// <param name="blockLayer">スポ�?�ン先�?�BlockLayer</param>
    /// <returns>サーバ�?�に登録されるPlayerData</returns>
    public PlayerData SetPlayerData(string name, int skinID, float playerX, float playerY, BlockLayer blockLayer) {
        // ラ�?パ�?�に対応するPlayerDataを設�?
        player.name = name;
        player.skinType = skinID;
        player.playerID = Guid.NewGuid();
        player.playerX = playerX;
        player.playerY = playerY;
        player.playerLayer = blockLayer;

        //接続が完�?して�?る�?�合、即時送信する(完�?して�?な�?場合�?�接続時に一括処�?)
        if (active) {
            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw);
            if (writer >= 0) {
                dsw.WriteFixedString4096(new FixedString4096Bytes("SPD," + player));
                Debug.Log(new FixedString4096Bytes("[WRAPPER]Sending user data:\n" + player));
                this.driver.EndSend(dsw);
            }
        }
        return player;
    }

#nullable enable
    /// <summary>
    /// 接続中のプレイヤーをすべて取得する。SetPlayerData()の未実行等で登録がな�?場合�?�null�?
    /// </summary>
    /// <returns>接続中のプレイヤーが含まれる PlayerData 配�??</returns>
    public PlayerData[]? GetPlayers() {
        return null;
    }

    /// <summary>
    /// プレイヤーの�?報を取得する。指定したGuidのプレイヤーが存在しな�?場合�?�null�?
    /// </summary>
    /// <param name="PlayerID">�?報を取得するPlayerID�?</param>
    /// <returns></returns>
    public PlayerData? GetPlayer(Guid PlayerID) {
        return null;
    }
    /// <summary>
    /// プレイヤーの移動情報を送信する�?
    /// </summary>
    /// <param name="layer">プレイヤーが存在するレイヤー</param>
    /// <param name="x">プレイヤーのX座�?</param>
    /// <param name="y">プレイヤーのY座�?</param>
    public void SendPlayerMove(BlockLayer layer, float x, float y) {
    }

    /// <summary>
    /// ブロ�?ク単位�?�変更を送信する�?
    /// </summary>
    /// <param name="layer">レイヤー番号</param>
    /// <param name="x">ブロ�?クのX座�?</param>
    /// <param name="y">ブロ�?クのY座�?</param>
    /// <param name="blockID">変更された後�?�ブロ�?クID</param>
    public void SendBlockChange(BlockLayer layer, int x, int y, int blockID) {

    }

    /// <summary>
    /// ワークスペ�?�スの�?報を送信する。存在するワークスペ�?�スの場合�?�上書きされる�?
    /// ワークスペ�?�ス設定�?�変更もこのMethodを利用する�?
    /// </summary>
    /// <param name="workspace">送信するWorkspace 構�?体�?��??</param>
    public void SendWorkspace(Workspace workspace) {

    }

    /// <summary>
    /// ワークスペ�?�スを削除する
    /// </summary>
    /// <param name="removeWorkspaceGuid">削除するワークスペ�?�スのGuid</param>
    public void SendWorkspaceRemove(Guid removeWorkspaceGuid) {

    }

    /// <summary>
    /// ワールドに存在するすべてのワークスペ�?�スを取得する�?
    /// </summary>
    /// <returns>存在するWorkspace 構�?体�?��??(ワークスペ�?�スが存在しな�?場合�?�null)</returns>
    public Workspace[]? GetWorkspaces() {
        return null;
    }

    /// <summary>
    /// �?定した�?�レイヤーが所有するワークスペ�?�スを取得する�?
    /// </summary>
    /// <param name="wsOwnerGuid">検索するプレイヤーのGuid</param>
    /// <returns>検索結果としてのWorkspace 構�?体�?��??(ワークスペ�?�スが存在しな�?、�?�レイヤーが存在しな�?場合�?�null</returns>
    public Workspace[]? GetWorkspacesOfPlayer(Guid wsOwnerGuid) {
        return null;
    }

    /// <summary>
    /// バッファを送信する�?
    /// </summary>
    /// <param name="workspaceGuid">送信するEditBufferが属するWorkspaceのGuid</param>
    /// <param name="editBuffer">送信するEditBuffer</param>
    /// <param name="layer">EditBufferの中で更新を通知するLayer</param>
    public void SendEditBuffer(Guid workspaceGuid, EditBuffer editBuffer, BlockLayer layer) {

    }

    /// <summary>
    /// ブロ�?ク単位�?�Workspaceに発生した変更を通知する�?
    /// </summary>
    /// <param name="workspaceGuid">変更が発生したWorkspaceのGuid</param>
    /// <param name="layer">変更が発生したEditBufferレイヤー</param>
    /// <param name="relativeX">EditBufferの左上を起点とした変更点の相対座標X</param>
    /// <param name="relativeY">EditBufferの左上を起点とした変更点の相対座標Y</param>
    /// <param name="blockID">変更先�?�ブロ�?クID</param>
    public void SendEditBufferBlockChange(Guid workspaceGuid, BlockLayer layer, int relativeX, int relativeY, int blockID) {

    }

    /// <summary>
    /// 手動でバッファの更新状況を取得する�?
    /// </summary>
    /// <param name="workspaceGuid">変更を取得するWorkspaceのGuid</param>
    /// <param name="layer">変更を取得するWorkspaceのレイヤー</param>
    /// <returns></returns>
    public int[][]? GetEditBufferManual(Guid workspaceGuid, BlockLayer layer) {
        return null;
    }
}