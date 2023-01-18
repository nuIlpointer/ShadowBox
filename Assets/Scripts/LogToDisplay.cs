using UnityEngine;
using UnityEngine.UI;

public class LogToDisplay : MonoBehaviour {
    private Text textObject;
    public float displayTime = 7f;
    private float dispCnt = 0f;
    public float textMinAlpha = 0.2f ;
    private Color bCol;    //base color
    // Start is called before the first frame update
    void Start() {
        textObject = GetComponent<Text>();
        bCol = textObject.color;
    }

    // Update is called once per frame
    void Update() {
        if(dispCnt < displayTime) {
            dispCnt += Time.deltaTime;
        }else if(dispCnt < displayTime + 2) {
            dispCnt += Time.deltaTime;
            textObject.color = new Color(bCol.r, bCol.g, bCol.b, 1 - ((dispCnt - 7) / 2) + textMinAlpha);
        }
    }

    public void addText(string text) {
        textObject.text += $"\n{text}";
        dispCnt = 0;
    }
}
