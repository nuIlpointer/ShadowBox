using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ServerItem : MonoBehaviour {
    public void OnClick() {
        transform.parent.gameObject.GetComponent<LoadServerList>().SetServerInfo(gameObject);

        foreach(GameObject o in GameObject.FindGameObjectsWithTag("ServerItem")) {
            if(o.Equals(gameObject)) {
                o.transform.Find("ServerBackground").gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.1f);
            } else {
                o.transform.Find("ServerBackground").gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0f);
            }
        }
    }
}
