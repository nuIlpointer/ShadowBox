using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sousa : MonoBehaviour
{
    public GameObject image;
    private bool flg = true;
    public void OnClick() {
        flg = !flg;
        image.SetActive(flg);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
