using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPosition : MonoBehaviour
{
    public Text xTxt;
    public Text yTxt;
    float x;
    float y;

    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        x = target.transform.position.x;
        y = target.transform.position.y;
        xTxt.text = "x:" + x.ToString("F2");
        yTxt.text = "y:" + y.ToString("F2");
    }
}
