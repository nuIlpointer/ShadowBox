using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kentiku : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject image1;
    public GameObject image2;
    private bool flg = true;
    public void Onclick() {
        flg = !flg;
        image2.SetActive(flg);
        image1.SetActive(!flg);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
