using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerSelectUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerController playerController;
    public GameObject upperButton;
    public GameObject lowerButton;

    public RectTransform LayerItemOrigin;
    public GameObject insideWall;
    public GameObject insideBlock;
    public GameObject outsideWall;
    public GameObject outsideBlock;

    public float selectingItemSize;
    public float baseItemSize;
    public Color selectOutColor;
    public float moveTime;

    public Vector3 moveVectorForSelect;
    public Vector3 layerItemSpace;

    private RectTransform iwTransform, ibTransform, owTransform, obTransform;
    
    private int selectingLayer;


    void Start()
    {
        iwTransform = insideWall.GetComponent<RectTransform>();
        ibTransform = insideBlock.GetComponent<RectTransform>();
        owTransform = outsideWall.GetComponent<RectTransform>();
        obTransform = outsideBlock.GetComponent<RectTransform>();

        iwTransform.localPosition = LayerItemOrigin.localPosition + layerItemSpace;
        ibTransform.localPosition = LayerItemOrigin.localPosition + layerItemSpace * 2;
        owTransform.localPosition = LayerItemOrigin.localPosition + -layerItemSpace;
        obTransform.localPosition = LayerItemOrigin.localPosition + -layerItemSpace * 2;

        selectingLayer = (int)playerController.pointerLayer;
    }

    public void UpperLayer() {
        MoveSelectingLayer(1);
    }

    public void LowerLayer() {
        MoveSelectingLayer(-1);
    }

    public void MoveSelectingLayer(int num) {
        for (int i = 0; i < 4; i++) {
                    


        }
    }



    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
