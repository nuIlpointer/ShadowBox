using UnityEngine;
using Unity.Networking.Transport;
using System.Net;
using Unity.Collections;

public class ShadowBoxClient : MonoBehaviour {
    public enum BlockLayer {
        InsideWall   = 1,
        InsideBlock  = 2,
        OutsideWall  = 3,
        OutsideBlock = 4
    };

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
    /// <param name="layerId">�v������`�����N�����݂��郌�C���[��ID</param>
    /// <param name="chunkId">�v������`�����N</param>
    /// <returns>�`�����N���(int�^2�����z��)</returns>
    public int[][] getChunk(int layerId, int chunkId) {
        // TODO �������Ƃ��
        return null;
    }

    /// <summary>
    /// �`�����N�����ݐڑ����Ă���T�[�o�[�ɑ��M����
    /// </summary>
    /// <param name="layerId">���M����`�����N�����݂��郌�C���[��ID</param>
    /// <param name="chunkId">���M����`�����N�̏ꏊ</param>
    /// <param name="chunkData">���M����`�����N���</param>
    /// <returns>���M�ɐ���������</returns>
    public bool sendChunk(int layerId, int chunkId, int[][] chunkData) {
        // �܂����r���ł���
        if(this.connection.IsCreated) {
            string sendDataTemp = "";
            foreach (int[] chunkRow in chunkData)
                for (int i = 0; i < chunkRow.Length; i++)
                    sendDataTemp += chunkRow[i] + (i == chunkRow.Length - 1 ? "\n" : ",");
            if((this.driver.BeginSend(this.connection, out DataStreamWriter dsw) >= 0)) {
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
    public void connect(string ipAddress, int port) {
        this.connectAddress = IPAddress.Parse(ipAddress);
        endPoint = NetworkEndPoint.AnyIpv4;
        using(NativeArray<byte> rawIpAddr = new NativeArray<byte>(this.connectAddress.GetAddressBytes().Length, Allocator.Temp)) {
            endPoint.SetRawAddressBytes(rawIpAddr);
        }

        if (port < 1 || port > 65535) port = 11781;
        endPoint.Port = (ushort)port;
        this.connectPort = port;
        this.connection = this.driver.Connect(endPoint);
    }
}
