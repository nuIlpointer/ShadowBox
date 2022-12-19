using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateController : MonoBehaviour
{
    public enum BrushTypes {
        sharp = 0,
        bold = 1,
        sharp_all_layer = 10,
        bold_all_layer = 11
    }
    public WorldLoader worldLoader;
    public ShadowBoxClientWrapper wrapper;

    //操作する変数
    public int useBlock;
    public int lineWidth;

    private int toBlock = -1;

    private Vector2Int[] SHARP_MARKS = {
        new Vector2Int(0, 0),
    };

    private Vector2Int[] BOLD_MARKS = { 
        new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 1),
        new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0),                                
        new Vector2Int(-1,-1), new Vector2Int( 0,-1), new Vector2Int( 1,-1), 
    };

    /// <summary>
    /// useBlockとlineWidthの値に応じてブロックを設置します。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="leyerNumber"></param>
    public void DrawBlock(int x, int y, int layerNumber) {
        Vector2Int[] marks;
        //ブラシのマーク位置を決定
        switch (lineWidth % 10) {
            case 0:
                marks = SHARP_MARKS;
                break;
            case 1:
                marks = BOLD_MARKS;
                break;
        }

        if(lineWidth / 10 == 1) {
            for(int i = 1; i <= 4; i++) {
                

                if (worldLoader.GetBlock(x, y, i) != useBlock) {
                    if (wrapper.IsConnectionActive()) {

                        wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)i, x, y, useBlock);

                    } else {

                        worldLoader.BlockUpdate(useBlock, i, x, y);

                    }
                }
            }
        } else {
            if (worldLoader.GetBlock(x, y, layerNumber) != useBlock) {
                if (wrapper.IsConnectionActive()) {

                    wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, useBlock);

                } else {

                    worldLoader.BlockUpdate(useBlock, layerNumber, x, y);

                }
            }
        }
        
    }

    public void DeleteBlock(int x, int y, int layerNumber) {
        if (worldLoader.GetBlock(x, y, layerNumber) != useBlock) {
            if (wrapper.IsConnectionActive()) {

                wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, 0);

            } else {

                worldLoader.BlockUpdate(0, layerNumber, x, y);

            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        useBlock = 10;
        lineWidth = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
