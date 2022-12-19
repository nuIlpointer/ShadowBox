using UnityEngine;
using UnityEngine.UI;

public class SkinItem : MonoBehaviour {
    public void OnClick() {
        transform.parent.gameObject.GetComponent<LoadSkinList>().SetUserInfo(gameObject);
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("SkinItem")) {
            if (o.Equals(gameObject)) {
                o.transform.Find("SkinBackground").gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.1f);
            } else {
                o.transform.Find("SkinBackground").gameObject.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0f);
            }
        }
    }
}
