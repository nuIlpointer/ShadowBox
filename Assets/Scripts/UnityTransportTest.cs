using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;

public class UnityTransportTest : MonoBehaviour {
    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    /// <summary>
    /// セットアップ ネットワークドライバのセットアップを行う。
    /// </summary>
    void Start() {
        this.driver = NetworkDriver.Create();

        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 11781; //いい中井？

        if (this.driver.Bind(endpoint) != 0)
            Debug.Log("Port binding failed");
        else 
            this.driver.Listen();

        this.connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        Debug.Log("Ready.");
    }
    
    /// <summary>
    /// ドライバと接続情報の破棄を行う
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
            //コネクションが作成されているかどうか
            Assert.IsTrue(this.connections[i].IsCreated);

            NetworkEvent.Type cmd;
            while((cmd = this.driver.PopEventForConnection(this.connections[i], out stream)) != NetworkEvent.Type.Empty) {
                if(cmd == NetworkEvent.Type.Data) {
                    //データを受信したとき
                    Debug.Log("Received Data: " + stream.ToString());
                } else if(cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("Disconnected.");
                    this.connections[i] = default(NetworkConnection);
                }
            }
        }
    }

}
