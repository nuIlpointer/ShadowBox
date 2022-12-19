using UnityEngine;
using UnityEngine.SceneManagement;
public class InGameMenu : MonoBehaviour {
    [SerializeField] private GameObject escapeMenuObject;
    
    private bool isOpen = false;
    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape))
            escapeMenuObject.SetActive((isOpen = !isOpen));
    }

    public void BackToTitle() {
        SceneManager.LoadScene("Title");
    }
}
