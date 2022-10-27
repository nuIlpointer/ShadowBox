using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generaTester : MonoBehaviour
{
    WorldLoader wl;
    // Start is called before the first frame update
    void Start()
    {
        wl = GetComponent<WorldLoader>();
        wl.LoadChunks(new Vector2((float)5.0,(float)5.0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
