using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class LayerSelectUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController playerController;
    //public GameObject upperButton;
    //public GameObject lowerButton;

    public RectTransform LayerItemOrigin;
    public GameObject insideWall;
    public GameObject insideBlock;
    public GameObject outsideWall;
    public GameObject outsideBlock;
    public TextMeshProUGUI layerNameView;

    public float selectingItemSize;
    public float baseItemSize;
    public Color selectOutColor;
    
    [SerializeField] private Vector2 moveVectorForSelect;
    [SerializeField] private Vector2 layerItemSpace;

    /// <summary>
    /// 0:InsideWall 1:InsideBlock 2:OutsideWall 4 OutsideBlock
    /// </summary>
    private RectTransform[] layerItemTransform = new RectTransform[4];
    
    private int selectingLayer = 0;
    private int oldSelectLayer = 0;
    void Start()
    {
        if(playerController == null) {
            playerController = GameObject.Find("player").GetComponent<PlayerController>();
            Debug.LogWarning("auto find");
        }

        layerItemTransform[3] = outsideBlock.GetComponent<RectTransform>();
        layerItemTransform[2] = outsideWall.GetComponent<RectTransform>();
        layerItemTransform[1] = insideBlock.GetComponent<RectTransform>();
        layerItemTransform[0] = insideWall.GetComponent<RectTransform>();

        layerItemTransform[3].localPosition = Vector3.zero;
        layerItemTransform[2].localPosition = layerItemSpace;
        layerItemTransform[1].localPosition = layerItemSpace * 2;
        layerItemTransform[0].localPosition = layerItemSpace * 3;

    }

    public void UpperLayer() {
        MoveSelectingLayer(1);
    }

    public void LowerLayer() {
        MoveSelectingLayer(-1);
    }

    public void MoveSelectingLayer(int num) {
        selectingLayer = num;
        if (selectingLayer < 0) selectingLayer = 0;
        if (selectingLayer > 3) selectingLayer = 3;
        

        for (int i = 0; i < 4; i++) {
            if(i == selectingLayer) {
                layerItemTransform[i].sizeDelta = new Vector2(selectingItemSize, selectingItemSize);
                layerItemTransform[i].anchoredPosition = Vector2.zero;
                layerItemTransform[i].GetComponent<Image>().color = Color.white;
            }else if(i > selectingLayer) {
                layerItemTransform[i].sizeDelta = new Vector2(baseItemSize, baseItemSize);
                layerItemTransform[i].anchoredPosition = layerItemSpace * (selectingLayer - i) - moveVectorForSelect;
                layerItemTransform[i].GetComponent<Image>().color = selectOutColor;
            } else {
                layerItemTransform[i].sizeDelta = new Vector2(baseItemSize, baseItemSize);
                layerItemTransform[i].anchoredPosition = layerItemSpace * (selectingLayer - i) + moveVectorForSelect + new Vector2(0, (-selectingItemSize / 2) + layerItemSpace.y);
                layerItemTransform[i].GetComponent<Image>().color = selectOutColor;
            }

        }

        switch (selectingLayer) {
            case 0:
                layerNameView.SetText("Back\n(背景)\n4/4");
                break;
            case 1:
                layerNameView.SetText("Back\n(足場)\n3/4");
                break;
            case 2:
                layerNameView.SetText("Flont\n(背景)\n2/4");
                break;
            case 3:
                layerNameView.SetText("Flont\n(足場)\n1/4");
                break;

        }
    }

    

    // Update is called once per frame
    void Update() {
        
        //Debug.LogWarning(playerController);
        selectingLayer = (int)playerController.pointerLayer;
        if (oldSelectLayer != selectingLayer) {
            oldSelectLayer = selectingLayer;
            MoveSelectingLayer(selectingLayer-1);
        }
    }
}
