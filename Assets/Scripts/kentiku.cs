using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class kentiku : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject image;
    public GameObject ChangeButton;
    
    private bool flg = false;
    public void Onclick() {
        flg = !flg;
        //image2.SetActive(flg);
        if (flg) {
            image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y + 300, image.transform.position.z);
            ChangeButton.GetComponentInChildren<Text>().text = "↑";
        } else {
            image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y - 300, image.transform.position.z);
            ChangeButton.GetComponentInChildren<Text>().text = "↓";
        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
