using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LoadServerList : MonoBehaviour {
    [SerializeField] private GameObject serverItemPrefab;
    [SerializeField] private float margin = 30;
    private float vIndex = 0;

    // Start is called before the first frame update
    void Start() {
        if (File.Exists("./server.dat")) {
            bool first = false;
            GameObject firstObject = new GameObject();
            ///起きてからの俺へ serverItemPrefabに良い感じの設定をしてなんとかしてください
            using (var reader = new StreamReader("./server.dat", Encoding.UTF8)) {
                while (reader.Peek() >= 0) {
                    var line = reader.ReadLine().Split(',');
                    var item = Instantiate(serverItemPrefab, transform);
                    item.transform.Find("ServerName").gameObject.GetComponent<TextMeshProUGUI>().text = line[0];
                    item.transform.Find("ServerIP").gameObject.GetComponent<TextMeshProUGUI>().text = $"{line[1]}:{line[2]}";
                    item.transform.position -= new Vector3(0, vIndex, 0);
                    vIndex += margin + item.GetComponent<RectTransform>().rect.height;
                    if (!first) {
                        firstObject = item;
                        first = true;
                    }
                }
                SetServerInfo(firstObject);

                foreach (GameObject o in GameObject.FindGameObjectsWithTag("ServerItem")) {
                    if (o.Equals(firstObject)) {
                        o.transform.Find("ServerBackground").gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.1f);
                    } else {
                        o.transform.Find("ServerBackground").gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0f);
                    }
                }
            }
        }
    }

    public void SetServerInfo(GameObject serverItem) {
        var address = serverItem.transform.Find("ServerIP").gameObject.GetComponent<TextMeshProUGUI>().text;
        TitleData.ipAddress = address.Split(':')[0];
        TitleData.port = int.Parse(address.Split(':')[1]);
        TitleData.isMultiPlay = true;
    }
}
