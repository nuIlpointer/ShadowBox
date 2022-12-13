using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kentiku : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject image;
    private bool flg = true;
    public void Onclick() {
        flg = !flg;
        //image2.SetActive(flg);
        if (flg) {
            image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y + 300, image.transform.position.z);
        } else {
            image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y - 300, image.transform.position.z);
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
