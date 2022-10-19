
using UnityEngine;
using Unity.Networking.Transport;
using System.Net;
using System.Text;
using Unity.Collections;
public class UnityTransportTestClient : MonoBehaviour {
    private NetworkDriver driver;
    private NetworkConnection connection;
    private IPAddress address;
    private float time = 0.0f;
    void Start() {
        NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
        this.driver = NetworkDriver.Create();
        this.address = IPAddress.Parse("127.0.0.1");
        var endpoint = NetworkEndPoint.AnyIpv4;
        using (var rawIp = new NativeArray<byte>(this.address.GetAddressBytes().Length, Allocator.TempJob)) {
            rawIp.CopyFrom(address.GetAddressBytes());
            endpoint.SetRawAddressBytes(rawIp);
        }
        endpoint.Port = 11781;
        this.connection = this.driver.Connect(endpoint);

    }

    void Update() {
        time += Time.deltaTime;
        if(time > 1.5) {
            time = 0;
            this.driver.ScheduleUpdate().Complete();
            if (!this.connection.IsCreated) {
                return;
            }

            var writer = this.driver.BeginSend(this.connection, out DataStreamWriter dsw, 128);
            if (writer >= 0) {
                dsw.WriteFixedString32(new FixedString32Bytes("testaaaaaaaaa"));
                this.driver.EndSend(dsw);
                this.connection.Disconnect(this.driver);
                this.connection = default(NetworkConnection);
            } else Debug.Log("Failed to create writer: " + writer);
        }
    }

    public void OnDestroy() {
        this.driver.Dispose();
    }
}
