using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateController : MonoBehaviour
{
    public enum BrushTypes {
        sharp           = 0,
        bold            = 1,
        sharp_all_layer = 10,
        bold_all_layer  = 11
    }

    public Dictionary<int, String> BRUSH_NAMES = new Dictionary<int, String>() {
        { 0 ,   "sharp"             },
        { 1 ,   "bold"              },
        { 10,   "sharp\n(all Layer)"  },
        { 11,   "bold\n(all Layer)"   }
    };

    public WorldLoader worldLoader;
    public ShadowBoxClientWrapper wrapper;

    //操作する変数
    public int useBlock;
    public int lineWidth;

    private bool replace;

    private Vector2Int[] SHARP_MARKS = {
        new Vector2Int(0, 0),
    };

    private Vector2Int[] BOLD_MARKS = { 
        new Vector2Int( 0, 2), new Vector2Int( 1, 2), new Vector2Int( 2, 2),
        new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 1),                                
        new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int( 2, 0), 
    };

    //特殊サイズブロック処理用
    private double usbDelay = 0;


    /// <summary>
    /// useBlockとlineWidthの値に応じてブロックを設置します。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="leyerNumber"></param>
    public void DrawBlock(int x, int y, int layerNumber) {
        //Debug.LogWarning($"({x},{y})");
        Vector2Int[] marks = null;
        //ブラシのマーク位置を決定
        switch (lineWidth % 10) {
            case 0:
                marks = SHARP_MARKS;
                replace = true;
                break;
            case 1:
                marks = BOLD_MARKS;
                replace = false;
                break;
            default:
                marks = SHARP_MARKS;
                replace = false;
                break;
        }
        if (useBlock >= 80) {       //useBlock = 特殊サイズブロックの場合
            if (usbDelay < 0.2) return;
            else usbDelay = 0;
            
            int oldBlock = worldLoader.GetBlock(x, y, layerNumber);
            if (oldBlock == useBlock) return;


            Vector2Int blockSize;
            LayerManager.UNNORMAL_SIZE_BLOCKS.TryGetValue(Enum.GetName(typeof(LayerManager.BLOCK_ID), useBlock), out blockSize);
            
            bool col = false;
            int i = 0, j = 0;
            for(i = 0; i < blockSize.y; i++) {//ブロックサイズ分
                for(j = 0; j < blockSize.x; j++) {
                    if (worldLoader.GetBlock(x + j, y + i, layerNumber) <= -2 || worldLoader.GetBlock(x + j, y + i, layerNumber) >= 80) {
                        DeleteUnnormalSizeBlock(x + j, y + i, layerNumber);
                    }
                }
            }

            

            int usbID = -2; //usb_0(usbの最初) のidが-2

            for (i = 0; i < blockSize.y; i++) {
                for (j = 0; j < blockSize.x; j++) {
                    if (i == 0 && j == 0) {
                        if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, useBlock);
                        else worldLoader.BlockUpdate(useBlock, layerNumber, x, y);
                    } else {
                        if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x + j, y + i, usbID);
                        else worldLoader.BlockUpdate(usbID, layerNumber, x + j, y + i);
                    }
                }
                usbID--;
            }

            /*if(usbDelay > 1) {
                //起点ブロック設置
                if (x >= 0 && x < worldLoader.GetWorldSizeX() && y >= 0 && y < worldLoader.GetWorldSizeY()) {
                    
                    if (worldLoader.GetBlock(x, y, layerNumber) <= -2 || worldLoader.GetBlock(x, y, layerNumber) >= 80) {
                        DeleteUnnormalSizeBlock(x, y, layerNumber);
                    }
                    if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, useBlock);
                    else worldLoader.BlockUpdate(useBlock, layerNumber, x, y);
                } else return;

                //補完ブロック(usb)設置 usb_に続く値は起点ブロックとのy座標の差を表す

                //ブロックサイズ取得
                Vector2Int blockSize;
                LayerManager.UNNORMAL_SIZE_BLOCKS.TryGetValue(Enum.GetName(typeof(LayerManager.BLOCK_ID), useBlock), out blockSize);

                //（memo : usb_0は-2）
                int usbID = -2;
                for (int i = 0; i < blockSize.y; i++) {
                    for (int j = i == 0 ? 1 : 0; j < blockSize.x; j++) {
                        if (x >= 0 && x < worldLoader.GetWorldSizeX() && y >= 0 && y < worldLoader.GetWorldSizeY()) {
                            if (worldLoader.GetBlock(x + j, y + i, layerNumber) <= -2 || worldLoader.GetBlock(x + j, y + i, layerNumber) >= 80) {
                                DeleteUnnormalSizeBlock(x + j, y + i, layerNumber);
                            }
                            if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x + j, y + i, usbID);
                            else worldLoader.BlockUpdate(usbID, layerNumber, x + j, y + i);
                        }

                    }
                    usbID--;
                }
                usbDelay = 0;
            }*/


        } else if(lineWidth / 10 == 1) {            //all layer
            for(int i = 1; i <= 4; i++) {                   //4レイヤー
                for (int j = 0; j < marks.Length; j++) {            //ブラシのブロック数分
                    //↓replace = falseなら　airのみブロック設置　：　trueなら　旧ブロックと新ブロックが違う　(かつ　旧ブロックがusbでなければ ← コメントアウト) 
                    if (!replace ? (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, i) == 0) : (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, i) != useBlock) /*&& worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, i) > -2*/) {
                        if( x + marks[j].x >= 0 && x + marks[j].x < worldLoader.GetWorldSizeX() && 
                            y + marks[j].y >= 0 && y + marks[j].y < worldLoader.GetWorldSizeY()) {
                            if (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, i) <= -2 || worldLoader.GetBlock(x, y, layerNumber) >= 80) {
                                DeleteUnnormalSizeBlock(x + marks[j].x, y + marks[j].y, i);
                            }
                            if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)i, x + marks[j].x, y + marks[j].y, useBlock);
                            else worldLoader.BlockUpdate(useBlock, i, x + marks[j].x, y + marks[j].y);

                        }
                        
                    }
                }
            }
        } else {//single layer
            for (int j = 0; j < marks.Length; j++) {
                //Debug.LogWarning( worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, layerNumber) > 0);
                if (!replace ? (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, layerNumber) == 0) : (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, layerNumber) != useBlock) /*&& worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, layerNumber) > -2*/) {
                    if (x + marks[j].x >= 0 && x + marks[j].x < worldLoader.GetWorldSizeX() &&
                        y + marks[j].y >= 0 && y + marks[j].y < worldLoader.GetWorldSizeY()) {
                        if (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, layerNumber) <= -2 || worldLoader.GetBlock(x, y, layerNumber) >= 80) {
                            DeleteUnnormalSizeBlock(x + marks[j].x, y + marks[j].y, layerNumber);
                        }
                        //Debug.Log($"[CreateController] > SendBlockChange() x : {x + marks[j].x} y : {y + marks[j].y}");
                        if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x + marks[j].x, y + marks[j].y, useBlock);
                        else worldLoader.BlockUpdate(useBlock, layerNumber, x + marks[j].x, y + marks[j].y);
                    }
                }
            }
        }
    }


    public void DeleteBlock(int x, int y, int layerNumber) {

        Debug.Log("ijijijijijijijijijijijijijijijijij");

        Vector2Int[] marks = null;
        //ブラシのマーク位置を決定
        switch (lineWidth % 10) {
            case 0:
                marks = SHARP_MARKS;
                break;
            case 1:
                marks = BOLD_MARKS;
                break;
            default:
                marks = SHARP_MARKS;
                break;
        }

        if (lineWidth / 10 == 1) {
            for (int i = 1; i <= 4; i++) {
                for (int j = 0; j < marks.Length; j++) {
                    if (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, i) != 0 /*&& worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, i) > 0*/) {
                        if (x + marks[j].x >= 0 && x + marks[j].x < worldLoader.GetWorldSizeX() &&
                            y + marks[j].y >= 0 && y + marks[j].y < worldLoader.GetWorldSizeY()) {
                            if (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, i) <= -2 || worldLoader.GetBlock(x, y, layerNumber) >= 80) {
                                DeleteUnnormalSizeBlock(x + marks[j].x, y + marks[j].y, i);
                            }
                            if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)i, x + marks[j].x, y + marks[j].y, 0);
                            else worldLoader.BlockUpdate(0, i, x + marks[j].x, y + marks[j].y);
                        }
                    }
                }
            }
        } else {
            for (int j = 0; j < marks.Length; j++) {
                if (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, layerNumber) != 0 /*&& worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, layerNumber) > 0*/) {
                    if (x + marks[j].x >= 0 && x + marks[j].x < worldLoader.GetWorldSizeX() &&
                        y + marks[j].y >= 0 && y + marks[j].y < worldLoader.GetWorldSizeY()) {
                        //Debug.LogWarning("uuuuuuuuuuuuuuuuuuu");
                        if (worldLoader.GetBlock(x + marks[j].x, y + marks[j].y, layerNumber) <= -2 || worldLoader.GetBlock(x, y, layerNumber) >= 80) {
                            
                            DeleteUnnormalSizeBlock(x + marks[j].x, y + marks[j].y, layerNumber);
                        }
                        if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x + marks[j].x, y + marks[j].y, 0);
                        else worldLoader.BlockUpdate(0, layerNumber, x + marks[j].x, y + marks[j].y);
                    }
                        
                }
            }
        }
    }

    void DeleteUnnormalSizeBlock(int x, int y, int layerNumber) {
        Debug.Log("[CreateController] > DeleteUnnormalSizeBlock");
        Vector2Int originPos = new Vector2Int(x, y);
        //usbの場合
        if (worldLoader.GetBlock(x,y,layerNumber) <= -2) {
            int targetUsbID = worldLoader.GetBlock(x, y, layerNumber) + 2;//起点ブロックからのy座標の距離を示す
            originPos = new Vector2Int(x, y + targetUsbID);
            
            //x軸走査
            while(worldLoader.GetBlock(originPos.x, originPos.y, layerNumber) < 80) {
                originPos.x--;
                if(originPos.x < 0) {
                    Debug.LogError($"[CreateController] > Unnormal size block origin search error : 特殊サイズブロックの削除において、ブロック原点を探索しましたが、見つかりませんでした。（x : {x} y : {y} layer number : {layerNumber} ）/n" +
                                    $"Description : 探索がワールド外に出ました");
                    if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, 0);
                    else worldLoader.BlockUpdate(0, layerNumber, x, y);
                    break;
                }
                if(x - originPos.x > 4) {
                    Debug.LogError($"[CreateController] > Unnormal size block origin search error : 特殊サイズブロックの削除において、ブロック原点を探索しましたが、見つかりませんでした。（x : {x} y : {y} layer number : {layerNumber} ）/n" +
                                    $"Description : 4ブロックにわたってx軸を走査しましたが、原点ブロックが見つかりませんでした。");
                    if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, 0);
                    else worldLoader.BlockUpdate(0, layerNumber, originPos.x, originPos.y);
                    break;
                }
            }
        }
        //usb・基点共通
        Vector2Int blockSize;
        if (Enum.GetName(typeof(LayerManager.BLOCK_ID), worldLoader.GetBlock(originPos.x, originPos.y, layerNumber)) != null) {
            Debug.LogWarning(Enum.GetName(typeof(LayerManager.BLOCK_ID), worldLoader.GetBlock(originPos.x, originPos.y, layerNumber)));
            blockSize = LayerManager.UNNORMAL_SIZE_BLOCKS[Enum.GetName(typeof(LayerManager.BLOCK_ID), worldLoader.GetBlock(originPos.x, originPos.y, layerNumber))];
        } else {
            Debug.LogError($"[LayerManager {name}] > Unnormal size block origin search error : 特殊サイズブロックの削除において、ブロック原点を探索しましたが、見つかりませんでした。（x : {x} y : {y} layer number : {layerNumber} ）/n" +
                                $"Description : id {worldLoader.GetBlock(originPos.x, originPos.y, layerNumber)} に該当する特殊サイズブロックが見つかりませんでした。 LayerManagerの列挙型BLOCK_IDか、Dictionary型UNNORMAL_SIZE_BLOCKSに記述がない可能性があります 座標:({originPos.x},{originPos.y},layerNumber)");
            if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, 0);
            else worldLoader.BlockUpdate(0, layerNumber, originPos.x, originPos.y);
            return;
        }
        if (!(x - originPos.x > blockSize.x)) {
            for (int i = 0; i < blockSize.y; i++) {
                for (int j = 0; j < blockSize.x; j++) {
                    if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, originPos.x + j, originPos.y + i, 0);
                    else worldLoader.BlockUpdate(0, layerNumber, originPos.x + j, originPos.y + i);
                }
            }
        } else {
            Debug.LogError($"[LayerManager {name}] > Unnormal size block origin search error : 特殊サイズブロックの削除において、ブロック原点の探索に失敗しました。（x : {x} y : {y} layer number : {layerNumber} ）/n" +
                                $"Description : id {worldLoader.GetBlock(originPos.x, originPos.y, layerNumber)} 座標:({originPos.x},{originPos.y},{layerNumber})のブロックが探索されましたが、ブロックサイズと探索始点座標が一致しませんでした。");
            if (wrapper.IsConnectionActive()) wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)layerNumber, x, y, 0);
            else worldLoader.BlockUpdate(0, layerNumber, originPos.x, originPos.y);
        }
    }


    public void PointerSize(out int x, out int y, out bool isAllLayer) {
        if (useBlock >= 80) {

            x = LayerManager.UNNORMAL_SIZE_BLOCKS[Enum.GetName(typeof(LayerManager.BLOCK_ID), useBlock)].x;
            y = LayerManager.UNNORMAL_SIZE_BLOCKS[Enum.GetName(typeof(LayerManager.BLOCK_ID), useBlock)].y;
            isAllLayer = false;
        } else {
            switch (lineWidth % 10) {
                case 0:
                    x = 1;
                    y = 1;
                    break;
                case 1:
                    x = 3;
                    y = 3;
                    break;
                default:
                    x = 1;
                    y = 1;
                    break;
            }

            switch (lineWidth / 10) {
                case 0:
                    isAllLayer = false;
                    break;
                case 1:
                    isAllLayer = true;
                    break;
                default:
                    isAllLayer = false;
                    break;
            }
        }
    }


    // Start is called before the first frame update
    void Start() {
        useBlock = 10;
        lineWidth = 1;

    }



    // Update is called once per frame
    void Update() {
        if(usbDelay < 0.2) {

            usbDelay += Time.deltaTime;

        }
    }
}
