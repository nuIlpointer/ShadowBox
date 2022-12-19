using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBrushViewer : MonoBehaviour
{
    public CreateController cc;

    private int selectBrush;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void SetBrush(int o) {
        selectBrush += o;
        cc.lineWidth = selectBrush;

    }
}
