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
        bool first = false;
        foreach (GameObject skin in skinList) {
            GameObject skinItem = Instantiate(skinPrefab, transform);
            skinItem.transform.Find("Skin").GetComponent<Image>().sprite = skin.GetComponent<SpriteRenderer>().sprite;
            skinItem.transform.position += new Vector3(hIndex, 0, 0);
            hIndex += margin + skinItem.GetComponent<RectTransform>().rect.width;
            if(!first) {
                first = true;
                skinItem.GetComponent<SkinItem>().OnClick();
            }
        }
    }

    public void SetName() {
        TitleData.playerName = playerName.text;
    }

    public void SetUserInfo(GameObject skinItem) {
        TitleData.playerName = playerName.text;
        int skinID = -1;
        for (int i = 0; i < skinList.Length; i++)
            if (skinList[i].GetComponent<SpriteRenderer>().sprite.Equals(skinItem.transform.Find("Skin").GetComponent<Image>().sprite)) skinID = i;
        TitleData.skinID = skinID;
        Debug.Log($"Player Name: {TitleData.playerName} skin:{TitleData.skinID}");
    }
}
