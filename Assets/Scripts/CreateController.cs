using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateController : MonoBehaviour
{
    public enum BrushTypes {
        sharp = 0,
        bold = 1
    }
    public WorldLoader worldLoader;
    public ShadowBoxClientWrapper wrapper;

    //操作する変数
    public int useBlock;
    public int lineWidth;

    private int toBlock = -1;


    /// <summary>
    /// useBlockとlineWidthの値に応じてブロックを設置します。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="leyerNumber"></param>
    public void DrawBlock(int x, int y, int layerNumber) {
        if(worldLoader.GetBlock(x, y, layerNumber) != useBlock) {
            if (wrapper.IsConnectionActive()) {

                wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, useBlock);

            } else {

                worldLoader.BlockUpdate(useBlock, layerNumber, x, y); 

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
