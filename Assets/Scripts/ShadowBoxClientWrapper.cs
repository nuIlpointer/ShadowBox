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
        // TODO �������Ƃ��
    }

    // Update is called once per frame
    void Update() {
        // TODO �������Ƃ��
    }

    /// <summary>
    /// ���ݐڑ����Ă���T�[�o�[�փ`�����N�f�[�^��v������
    /// </summary>
    /// <param name="layerID">�v������`�����N�����݂��郌�C���[��ID</param>
    /// <param name="chunkID">�v������`�����N</param>
    /// <returns>�`�����N���(int�^2�����z��)</returns>
    public int[][] GetChunk(BlockLayer layerID, int chunkID) {
        // TODO �������Ƃ��
        return null;
    }

    /// <summary>
    /// �`�����N�����ݐڑ����Ă���T�[�o�[�ɑ��M����
    /// </summary>
    /// <param name="layerID">���M����`�����N�����݂��郌�C���[��ID</param>
    /// <param name="chunkID">���M����`�����N�̏ꏊ</param>
    /// <param name="chunkData">���M����`�����N���</param>
    /// <returns>���M�ɐ���������</returns>
    public bool SendChunk(BlockLayer layerID, int chunkID, int[][] chunkData) {
        // �܂����r���ł���
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
    /// �ڑ���̃|�[�g/IP�A�h���X���w�肵�A�ڑ�����B
    /// �|�[�g���͈͊O�̎��͎����I�Ɂu11781�v�B
    /// </summary>
    /// <param name="ipAddress">�ڑ���IP�A�h���X�B</param>
    /// <param name="port">�ڑ���|�[�g�ԍ��B�f�t�H���g��11781�B</param>
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
    /// �T�[�o�[�Ƀv���C���[���𑗐M����B
    /// </summary>
    /// <param name="name">���l�ɕ\������閼�O</param>
    /// <param name="skinID">���l����\������錩����(���邩�H����)</param>
    /// <returns>�T�[�o�[�ɓo�^���ꂽPlayerData</returns>
    public PlayerData SetPlayerData(string name, int skinID) {
        return new PlayerData { };
    }

    /// <summary>
    /// �ڑ����̃v���C���[�����ׂĎ擾����BSetPlayerData()�̖����s���œo�^���Ȃ��ꍇ��null�B
    /// </summary>
    /// <returns>�ڑ����̃v���C���[���܂܂�� PlayerData �z��</returns>
    public PlayerData[]? GetPlayers() {
        return null;
    }

    /// <summary>
    /// �v���C���[�̏����擾����B�w�肵��Guid�̃v���C���[�����݂��Ȃ��ꍇ��null�B
    /// </summary>
    /// <param name="PlayerID">�����擾����PlayerID�B</param>
    /// <returns></returns>
    public PlayerData? GetPlayer(Guid PlayerID) {
        return null;
    }
    /// <summary>
    /// �v���C���[�̈ړ����𑗐M����B
    /// </summary>
    /// <param name="layer">�v���C���[�����݂��郌�C���[</param>
    /// <param name="x">�v���C���[��X���W</param>
    /// <param name="y">�v���C���[��Y���W</param>
    public void SendPlayerMove(BlockLayer layer, float x, float y) {
    }

    /// <summary>
    /// �u���b�N�P�ʂ̕ύX�𑗐M����B
    /// </summary>
    /// <param name="layer">���C���[�ԍ�</param>
    /// <param name="x">�u���b�N��X���W</param>
    /// <param name="y">�u���b�N��Y���W</param>
    /// <param name="blockID">�ύX���ꂽ��̃u���b�NID</param>
    public void SendBlockChange(BlockLayer layer, int x, int y, int blockID) {

    }

    /// <summary>
    /// ���[�N�X�y�[�X�̏��𑗐M����B
    /// </summary>
    /// <param name="workSpace">���M����Workspace �\���̔z��</param>
    public void SendWSInfo(WorkSpace workspace) {

    }

    /// <summary>
    /// ���[���h�ɑ��݂��邷�ׂẴ��[�N�X�y�[�X���擾����B
    /// </summary>
    /// <returns>���݂���Workspace �\���̔z��(���[�N�X�y�[�X�����݂��Ȃ��ꍇ��null)</returns>
    public Workspace[]? GetWorkspaces() {
        return null;
    }

    /// <summary>
    /// �w�肵���v���C���[�����L���郏�[�N�X�y�[�X���擾����B
    /// </summary>
    /// <param name="wsOwnerGuid">��������v���C���[��Guid</param>
    /// <returns>�������ʂƂ��Ă�Workspace �\���̔z��(���[�N�X�y�[�X�����݂��Ȃ��A�v���C���[�����݂��Ȃ��ꍇ��null</returns>
    public Workspace[]? GetWorkspacesOfPlayer(Guid wsOwnerGuid) {
        return null;
    }

    /// <summary>
    /// ���[�N�X�y�[�X���폜����
    /// </summary>
    /// <param name="removeWorkSpaceGuid">�폜���郏�[�N�X�y�[�X��Guid</param>
    public void SendWSRemove(Guid removeWorkspaceGuid) {

    }
}