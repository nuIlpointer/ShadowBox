using UnityEngine;
using Unity.Networking.Transport;
using System.Net;
using System;
using Unity.Collections;

public class ShadowBoxClientWrapper : MonoBehaviour {
    public enum BlockLayer {
        InsideWall = 1,
        InsideBlock = 2,
        OutsideWall = 3,
        OutsideBlock = 4
    };


    public struct PlayerData {
        string name;
        int skinType;
        Guid playerID;
        float playerX;
        float playerY;
        BlockLayer playerLayer;
    }

    public struct Workspace {
        Guid workspaceID;
        Guid wsOwnerID;
        int x1;
        int y1;
        int x2;
        int y2;
    }
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
        // まだ作り途中ですよ
        if (this.connection.IsCreated) {
            string sendDataTemp = "";
            foreach (int[] chunkRow in chunkData)
                for (int i = 0; i < chunkRow.Length; i++)
                    sendDataTemp += chunkRow[i] + (i == chunkRow.Length - 1 ? "\n" : ",");
            if ((this.driver.BeginSend(this.connection, out DataStreamWriter dsw) >= 0)) {
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
    public void Connect(string ipAddress, int port) {
        this.connectAddress = IPAddress.Parse(ipAddress);
        endPoint = NetworkEndPoint.AnyIpv4;
        using (NativeArray<byte> rawIpAddr = new NativeArray<byte>(this.connectAddress.GetAddressBytes().Length, Allocator.Temp)) {
            endPoint.SetRawAddressBytes(rawIpAddr);
        }

        if (port < 1 || port > 65535) port = 11781;
        endPoint.Port = (ushort)port;
        this.connectPort = port;
        this.connection = this.driver.Connect(endPoint);
    }

    /// <summary>
    /// サーバーにプレイヤー情報を送信する。
    /// </summary>
    /// <param name="name">他人に表示される名前</param>
    /// <param name="skinID">他人から表示される見た目(いるか？これ)</param>
    /// <returns>サーバーに登録されたPlayerData</returns>
    public PlayerData SetPlayerData(string name, int skinID) {
        return new PlayerData { };
    }

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
    /// ワークスペースの情報を送信する。
    /// </summary>
    /// <param name="workSpace">送信するWorkspace 構造体配列</param>
    public void SendWSInfo(WorkSpace workspace) {

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
    /// ワークスペースを削除する
    /// </summary>
    /// <param name="removeWorkSpaceGuid">削除するワークスペースのGuid</param>
    public void SendWSRemove(Guid removeWorkspaceGuid) {

    }
}