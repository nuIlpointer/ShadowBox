using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenNextScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    public void Open() {
        SceneManager.LoadScene(nextSceneName);
    }

}
