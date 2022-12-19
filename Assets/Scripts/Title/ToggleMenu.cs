using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleMenu : MonoBehaviour {
    [SerializeField] private GameObject topObject;
    
    private GameObject currentObj;

    // Start is called before the first frame update
    void Start() {
        currentObj = topObject;
    }

    // Update is called once per frame
    void Update() {

    }

    public void EnableObject(GameObject go) {
        currentObj.SetActive(false);
        go.SetActive(true);
        currentObj = go;
    }

    public void SetAsLocal() {
        TitleData.port = 11781;
        TitleData.ipAddress = "127.0.0.1";
        TitleData.isMultiPlay = false;
    }
}
