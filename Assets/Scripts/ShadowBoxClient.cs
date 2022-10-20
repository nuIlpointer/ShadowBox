using UnityEngine;
using Unity.Networking.Transport;
using System.Net;
using Unity.Collections;
using Unity.Collections;

public class ShadowBoxClient : MonoBehaviour {
    enum BlockLayer {
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

    void Start() {
        this.driver = NetworkDriver.Create();
        
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// ���ݐڑ����Ă���T�[�o�[�փ`�����N�f�[�^��v������
    /// </summary>
    /// <param name="layerId">��M����`�����N�����݂��郌�C���[��ID</param>
    /// <param name="chunkId">�v������`�����N</param>
    /// <returns>�`�����N���(int�^2�����z��)</returns>
    public int[,] getChunk(int layerId, int chunkId) {
        return null;
    }

    /// <summary>
    /// �`�����N�����ݐڑ����Ă���T�[�o�[�ɑ��M����
    /// </summary>
    /// <param name="layerId">���M����`�����N�����݂��郌�C���[��ID</param>
    /// <param name="chunkId">���M����`�����N�̏ꏊ</param>
    /// <param name="sendChunkData">���M����`�����N���</param>
    /// <returns>���M�ɐ���������</returns>
    public bool sendChunk(int layerId, int chunkId, int[,] sendChunkData) {

        return false;
    }

    /// <summary>
    /// �ڑ���̃|�[�g/IP�A�h���X���w�肵�A�ڑ�����B
    /// �|�[�g��null�̎��́u11781�v�B
    /// </summary>
    /// <param name="ipAddress">IP�A�h���X�B"127.0.0.1"�Ń��[�J���B</param>
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
