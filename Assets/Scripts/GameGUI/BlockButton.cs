using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockButton : MonoBehaviour
{
    public CreateController cc;
    public int ccChange;
    // Start is called before the first frame update
    
    // Update is called once per frame
    
    public void OnClick() {
        cc.useBlock = ccChange;
    }
}
