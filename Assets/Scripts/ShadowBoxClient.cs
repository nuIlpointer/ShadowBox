using UnityEngine;
using Unity.Networking.Transport;
using System.Net;
using Unity.Collections;

public class ShadowBoxClient : MonoBehaviour {
    private IPAddress connectAddress;
    private int connectPort;
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// ���ݐڑ����Ă���T�[�o�[�փ`�����N�f�[�^��v������
    /// </summary>
    /// <param name="chunkNumber">�v������`�����N</param>
    /// <returns>�`�����N���(int�^2�����z��)</returns>
    public int[,] getChunk(int chunkNumber) {
        return null;
    }

    /// <summary>
    /// �`�����N�����ݐڑ����Ă���T�[�o�[�ɑ��M����
    /// </summary>
    /// <param name="chunkNumber">���M����`�����N�̏ꏊ</param>
    /// <param name="sendChunkData">���M����`�����N���</param>
    /// <returns>���M�ɐ���������</returns>
    public bool sendChunk(int chunkNumber, int[,] sendChunkData) {
        return false;
    }

    /// <summary>
    /// �ڑ���̃|�[�g/IP�A�h���X���w�肷��B
    /// �|�[�g��null�̎��́u11781�v�B
    /// </summary>
    /// <param name="ipAddress">IP�A�h���X�B"127.0.0.1"�Ń��[�J���B</param>
    /// <param name="port">�ڑ���|�[�g�ԍ��B�f�t�H���g��11781�B</param>
    public void setConnection(string ipAddress, int port) {
        if (port == null) port = 11781;

    }
}
