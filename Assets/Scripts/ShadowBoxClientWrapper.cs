using System;
using System.Net;
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
        // TODO さっさとやれ
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
            } else if (cmd == NetworkEvent.Type.Disconnect) {
                Debug.Log("[WRAPPER]Disconnect.");
                this.connection = default(NetworkConnection);
            }
        }
    }

    /// <summary>
    /// ドライバと接続情報の破棄を行う
    /// </summary>
    public void OnDestroy() {
        this.driver.Dispose();
    }

    /// <summary>
    /// 現在接続しているサーバーへチャンクデータを要求する
    /// </summary>
    /// <param name="layerID">要求するチャンクが存在するレイヤーのID</param>
    /// <param name="chunkID">要求するチャンク</param>
    /// <returns>チャンク情報(int型2次元配列)</returns>
    public int[][] GetChunk(BlockLayer layerID, int chunkID) {
        // TODO さっさとやれ
        return null;
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
            this.driver.ScheduleUpdate().Complete();
            if (!this.connection.IsCreated) {
                return false;
            }
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
        Debug.Log("[WRAPPER]Connect to " + endPoint);
    }

    /// <summary>
    /// サーバーにプレイヤー情報を送信する。接続完了前に実行された場合、完了時に送信される。
    /// </summary>
    /// <param name="name">他人に表示される名前</param>
    /// <param name="skinID">他人から表示される見た目(いるか？これ)</param>
    /// <returns>サーバーに登録されたPlayerData</returns>
    public PlayerData SetPlayerData(string name, int skinID, float playerX, float playerY, BlockLayer blockLayer) {
        player.name = name;
        player.skinType = skinID;
        player.playerID = Guid.NewGuid();
        player.playerX = playerX;
        player.playerY = playerY;
        player.playerLayer = blockLayer;
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
    /// 接続中のプレイヤーをすべて取得する。SetPlayerData()の未実行等で登録がない場合はnull。
    /// </summary>
    /// <returns>接続中のプレイヤーが含まれる PlayerData 配列</returns>
    public PlayerData[]? GetPlayers() {
        return null;
    }

    /// <summary>
    /// プレイヤーの情報を取得する。指定したGuidのプレイヤーが存在しない場合はnull。
    /// </summary>
    /// <param name="PlayerID">情報を取得するPlayerID。</param>
    /// <returns></returns>
    public PlayerData? GetPlayer(Guid PlayerID) {
        return null;
    }
    /// <summary>
    /// プレイヤーの移動情報を送信する。
    /// </summary>
    /// <param name="layer">プレイヤーが存在するレイヤー</param>
    /// <param name="x">プレイヤーのX座標</param>
    /// <param name="y">プレイヤーのY座標</param>
    public void SendPlayerMove(BlockLayer layer, float x, float y) {
    }

    /// <summary>
    /// ブロック単位の変更を送信する。
    /// </summary>
    /// <param name="layer">レイヤー番号</param>
    /// <param name="x">ブロックのX座標</param>
    /// <param name="y">ブロックのY座標</param>
    /// <param name="blockID">変更された後のブロックID</param>
    public void SendBlockChange(BlockLayer layer, int x, int y, int blockID) {

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
    /// 指定したプレイヤーが所有するワークスペースを取得する。
    /// </summary>
    /// <param name="wsOwnerGuid">検索するプレイヤーのGuid</param>
    /// <returns>検索結果としてのWorkspace 構造体配列(ワークスペースが存在しない、プレイヤーが存在しない場合はnull</returns>
    public Workspace[]? GetWorkspacesOfPlayer(Guid wsOwnerGuid) {
        return null;
    }

    /// <summary>
    /// バッファを送信する。
    /// </summary>
    /// <param name="workspaceGuid">送信するEditBufferが属するWorkspaceのGuid</param>
    /// <param name="editBuffer">送信するEditBuffer</param>
    /// <param name="layer">EditBufferの中で更新を通知するLayer</param>
    public void SendEditBuffer(Guid workspaceGuid, EditBuffer editBuffer, BlockLayer layer) {

    }

    /// <summary>
    /// ブロック単位のWorkspaceに発生した変更を通知する。
    /// </summary>00
    /// <param name="workspaceGuid">変更が発生したWorkspaceのGuid</param>
    /// <param name="layer">変更が発生したEditBufferレイヤー</param>
    /// <param name="relativeX">EditBufferの左上を起点とした変更点の相対座標X</param>
    /// <param name="relativeY">EditBufferの左上を起点とした変更点の相対座標Y</param>
    /// <param name="blockID">変更先のブロックID</param>
    public void SendEditBufferBlockChange(Guid workspaceGuid, BlockLayer layer, int relativeX, int relativeY, int blockID) {

    }

    /// <summary>
    /// 手動でバッファの更新状況を取得する。
    /// </summary>
    /// <param name="workspaceGuid">変更を取得するWorkspaceのGuid</param>
    /// <param name="layer">変更を取得するWorkspaceのレイヤー</param>
    /// <returns></returns>
    public int[][]? GetEditBufferManual(Guid workspaceGuid, BlockLayer layer) {
        return null;
    }
}