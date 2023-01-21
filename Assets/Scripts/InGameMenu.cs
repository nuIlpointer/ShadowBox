using UnityEngine;
using UnityEngine.SceneManagement;
public class InGameMenu : MonoBehaviour {
    [SerializeField] private GameObject escapeMenuObject;
    [SerializeField] private GameObject audioSourceObject;
    [SerializeField] private float targetVolume = 0.4f;
    private AudioSource audioSource;
    private bool isOpen = false;

    void Start() {
        audioSource = audioSourceObject.GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape))
            escapeMenuObject.SetActive((isOpen = !isOpen));

        if (isOpen && audioSource.volume == 1.0)
            audioSource.volume = targetVolume;
        else if (!isOpen && audioSource.volume != 1.0)
            audioSource.volume = 1.0f;

    }

    public void BackToTitle() {
        SceneManager.LoadScene("Title");
    }

}
