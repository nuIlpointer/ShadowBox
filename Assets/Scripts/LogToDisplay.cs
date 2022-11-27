using UnityEngine;
using UnityEngine.UI;

public class LogToDisplay : MonoBehaviour {
    private Text textObject;
    // Start is called before the first frame update
    void Start() {
        textObject = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void addText(string text) {
        textObject.text += $"\n{text}";
    }
}
