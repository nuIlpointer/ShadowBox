using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelector : MonoBehaviour
{
    private GameObject content;
    private GameObject block;
    private GameObject buttonPrefab;

    private Sprite blockImg;

    public CreateController cc;
    
    public int rightMargin;
    public int topMargin;
    public int ButtonSpacing = 100;
    public int ColumnsNum = 5;



    // Start is called before the first frame update
    void Start()
    {
        content = transform.Find("Content").gameObject;
        buttonPrefab = (GameObject)Resources.Load("BlockButton/Button");

        int buttonPosX = rightMargin;
        int buttonPosY = topMargin;
        int setColumn = 0;
        int setRow = 0;
        GameObject operatingButton;
        foreach(string blockName in Enum.GetNames(typeof(LayerManager.BLOCK_ID))) {
            if(blockName != "air" && blockName != "unknown") {
                buttonPosX = rightMargin + (ButtonSpacing * setColumn);
                buttonPosY = 0 - (topMargin + (ButtonSpacing * setRow));
                operatingButton = Instantiate(buttonPrefab, content.transform);
                operatingButton.transform.position = new Vector3(buttonPosX, buttonPosY);
                
                operatingButton.GetComponent<Image>().sprite = Resources.Load($"blockImages/{blockName}", typeof(Sprite)) as Sprite;
                operatingButton.GetComponent<BlockButton>().cc = cc;
                operatingButton.GetComponent<BlockButton>().ccChange = (int)Enum.Parse(typeof(LayerManager.BLOCK_ID),blockName);
                setColumn++;
                if (setColumn >= ColumnsNum) {
                    setColumn = 0;
                    setRow++;

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
