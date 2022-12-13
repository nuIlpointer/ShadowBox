using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointer_layer : MonoBehaviour
{
    // Start is called before the first frame update
    public float pl;
    public PlayerController pc;
    void Start()
    {
        pl = pc.pointerLayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
