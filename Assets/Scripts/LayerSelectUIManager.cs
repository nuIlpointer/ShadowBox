using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

    public float selectingItemSize;
    public float baseItemSize;
    public Color selectOutColor;
    public float moveTime;

    public Vector3 moveVectorForSelect = new Vector3(100,20);
    public Vector3 layerItemSpace;

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
                layerItemTransform[i].localPosition = (layerItemSpace * i) + moveVectorForSelect;
            }else if(i < selectingLayer) {
                layerItemTransform[i].sizeDelta = new Vector2(baseItemSize, baseItemSize);
                layerItemTransform[i].localPosition = layerItemSpace * i;
            }else {
                layerItemTransform[i].sizeDelta = new Vector2(baseItemSize, baseItemSize);
                layerItemTransform[i].localPosition = (layerItemSpace * i) + (moveVectorForSelect * 2);
                Debug.LogWarning(moveVectorForSelect + " " + i);
            }


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
