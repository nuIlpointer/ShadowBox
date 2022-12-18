using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class LoadSkinList : MonoBehaviour {
    [SerializeField] private GameObject[] skinList;
    [SerializeField] private GameObject skinPrefab;
    [SerializeField] private GameObject nameInputField;
    [SerializeField] private float margin = 30;
    [SerializeField] private int horizontalMax = 1;

    private InputField playerName;
    private float hIndex = 0;
    // Start is called before the first frame update
    void Start() {
        playerName = nameInputField.GetComponent<InputField>();
        foreach (GameObject skin in skinList) {
            GameObject skinItem = Instantiate(skinPrefab, transform);
            skinItem.transform.Find("Skin").GetComponent<Image>().sprite = skin.GetComponent<SpriteRenderer>().sprite;
            skinItem.transform.position += new Vector3(hIndex, 0, 0);
            hIndex += margin + skinItem.GetComponent<RectTransform>().rect.width;
        }
    }

    public void SetUserInfo(GameObject skinItem) {
        TitleData.playerName = playerName.text;
        TitleData.skinID = System.Array.IndexOf(skinList, skinItem);
    }
}
