using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnHover() {
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnExit() {
        transform.localScale = Vector3.one;
    }
}
