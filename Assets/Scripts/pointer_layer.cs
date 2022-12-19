using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointer_layer : MonoBehaviour
{
    // Start is called before the first frame update
   
    public PlayerController pc;
   

    private bool flg = true;

    public void OnClickUp() {
        pc.pointerLayer = pc.pointerLayer + 1;    
    }
    public void OnClickDown() {
        pc.pointerLayer = pc.pointerLayer - 1;
    }
}
