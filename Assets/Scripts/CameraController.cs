using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.UpArrow)) {
            pos.y += (float)0.1;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            pos.y -= (float)0.1;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            pos.x -= (float)0.1;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            pos.x += (float)0.1;
        }

        this.gameObject.transform.position = pos;
    }
}
