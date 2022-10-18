using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class UnityTransportTest : MonoBehaviour {
    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    /// <summary>
    /// �Z�b�g�A�b�v �l�b�g���[�N�h���C�o�̃Z�b�g�A�b�v���s���B
    /// </summary>
    void Start() {
        this.driver = NetworkDriver.Create();

        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 11781; //��������H

        if (this.driver.Bind(endpoint) != 0)
            Debug.Log("Port binding failed");
        else 
            this.driver.Listen();

        this.connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        Debug.Log("Ready.");
    }
    
    /// <summary>
    /// �h���C�o�Ɛڑ����̔j�����s��
    /// </summary>
    public void OnDestroy() {
        this.driver.Dispose();
        this.connections.Dispose();
    }

    void Update() {
        this.driver.ScheduleUpdate().Complete();

        for(int i = 0; i < this.connections.Length; i++) {
            if(!this.connections[i].IsCreated) {
                this.connections.RemoveAtSwapBack(i);
                i--;
            }
        }

        NetworkConnection connection;
        while ((connection = this.driver.Accept()) != default(NetworkConnection)) {

            this.connections.Add(connection);
        }

        DataStreamReader stream;
        for(int i = 0; i < this.connections.Length; i++) {
            //�R�l�N�V�������쐬����Ă��邩�ǂ���
            Assert.IsTrue(this.connections[i].IsCreated);

            NetworkEvent.Type cmd;
            while((cmd = this.driver.PopEventForConnection(this.connections[i], out stream)) != NetworkEvent.Type.Empty) {
                if(cmd == NetworkEvent.Type.Data) {
                    //�f�[�^����M�����Ƃ�
                    Debug.Log("Received Data: " + stream.ToString());
                } else if(cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("Disconnected.");
                    this.connections[i] = default(NetworkConnection);
                }
            }
        }
    }

}
