using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UseBrushViewer : MonoBehaviour
{
    public CreateController cc;
    GameObject listUp;
    GameObject listDown;
    GameObject nameView;
    private int selectBrush;
    private int selectBArreyIdx;
    int[] BrushArrey;
    // Start is called before the first frame update
    void Start()
    {
        BrushArrey = (int[])Enum.GetValues(typeof(CreateController.BrushTypes));
    }

    // Update is called once per frame
    void SetBrush(int o) {

        selectBArreyIdx += o;

        //リスト上端の時
        if (selectBArreyIdx <= 0) {
            selectBArreyIdx = 0;
            listUp.GetComponent<Image>().color = new Color(100, 100, 100);
        } else {
            listUp.GetComponent<Image>().color = new Color(255, 255, 255);
        }
        
        //リスト下端の時
        if (selectBArreyIdx >= BrushArrey.Length - 1) {
            selectBArreyIdx = BrushArrey.Length - 1;
            listDown.GetComponent<Image>().color = new Color(100, 100, 100);
        } else {
            listDown.GetComponent <Image>().color = new Color(255, 255, 255);
        }
        
        selectBrush = BrushArrey[o];
        //nameView.GetComponent<TextMeshPro>().text =;

        cc.lineWidth = selectBrush;
        
    }
}
