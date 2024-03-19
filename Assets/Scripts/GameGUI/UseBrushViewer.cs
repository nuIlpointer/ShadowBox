using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UseBrushViewer : MonoBehaviour
{
    public CreateController cc;
    public GameObject listUp;
    public GameObject listDown;
    public GameObject nameView;
    
    private int selectBrush = 0;
    private int selectBArreyIdx = 0;
    private int[] BrushArrey;
    // Start is called before the first frame update
    void Start()
    {
        BrushArrey = (int[])Enum.GetValues(typeof(CreateController.BrushTypes));
        SetBrush(0);
    }

    // Update is called once per frame
    public void SetBrush(int o) {

        selectBArreyIdx += o;

        //リスト上端の時
        if (selectBArreyIdx <= 0) {
            selectBArreyIdx = 0;
            listUp.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        } else {
            listUp.GetComponent<Image>().color = new Color(1,1,1);
        }
        
        //リスト下端の時
        if (selectBArreyIdx >= BrushArrey.Length - 1) {
            selectBArreyIdx = BrushArrey.Length - 1;
            listDown.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        } else {
            listDown.GetComponent <Image>().color = new Color(1,1,1);
        }

        selectBrush = BrushArrey[selectBArreyIdx];
        this.gameObject.GetComponent<Image>().sprite = Resources.Load($"Generic/PointerImage/{Enum.GetName(typeof(CreateController.BrushTypes), selectBrush)}", typeof(Sprite)) as Sprite;

        nameView.GetComponent<TextMeshProUGUI>().text = cc.BRUSH_NAMES[selectBrush];

        cc.lineWidth = selectBrush;
        
    }

    public void UpList() {
        SetBrush(-1);
    }
    public void DownList() {
        SetBrush(+1);
    }
}
