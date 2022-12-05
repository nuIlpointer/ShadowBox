using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateController : MonoBehaviour
{
    public WorldLoader worldLoader;
    public ShadowBoxClientWrapper wrapper;

    //操作する変数
    public int useBlock;
    public int lineWidth;


    /// <summary>
    /// useBlockとlineWidthの値に応じてブロックを設置します。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="leyerNumber"></param>
    public void DrawBlock(int x, int y, int leyerNumber) {
        if (wrapper.IsConnectionActive()) {

        }
        worldLoader.BlockUpdate(useBlock, leyerNumber, x, y);
        //wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)leyerNumber, x, y, useBlock);
    }



    // Start is called before the first frame update
    void Start()
    {
        useBlock = 0;
        lineWidth = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
