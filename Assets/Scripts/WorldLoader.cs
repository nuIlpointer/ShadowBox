using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldLoader : MonoBehaviour


{
    //      ファイル名
    private Vector2 loadChunkPos;
    private string[] blockIDList;
    /// <summary>
    /// 各レイヤーの参照を格納　添え字はenum blockLayerと揃える為1~4(0は欠番)
    /// </summary>
    LayerManager[] layers = new LayerManager[5];

    private int cNumX;
    private int cNumY;
    private int cSize;
    InitialProcess ip;
    ShadowBoxClientWrapper wrapper;
    int lastMakePoint = -1;


    // Start is called before the first frame update
    void Start()
    {
        wrapper = gameObject.GetComponent<ShadowBoxClientWrapper>();
        ip = gameObject.GetComponent<InitialProcess>();
        layers[1] = transform.Find("LayerInsideWall").GetComponent<LayerManager>();
        layers[2] = transform.Find("LayerInsideBlock").GetComponent<LayerManager>();
        layers[3] = transform.Find("LayerOutsideWall").GetComponent<LayerManager>();
        layers[4] = transform.Find("LayerOutsideBlock").GetComponent<LayerManager>();
        cNumX = ip.chunksNumX;
        cNumY = ip.chunksNumY;
        cSize = ip.chunkSize;
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    ///指定位置周辺のチャンクを生成
    /// </summary>
    /// <param name="pos">基準座標を指定</param>
    public void LoadChunks(Vector2 pos)
    {
        cSize = 25; cNumX = 4; cNumY = 2;
        int chunkNumber = ((int)pos.x / cSize) + cNumX * (((int)pos.y / cSize)+1);
        if(chunkNumber != lastMakePoint) {
            for (int i = 1; i <= 4; i++) {
                layers[i].MakeChunk(chunkNumber);
                bool up = false, lo = false, le = false, ri = false;
                if(chunkNumber - cNumX >= 0) { layers[i].MakeChunk(chunkNumber - cNumX); up = true; }
                if(chunkNumber + cNumX < cNumX * cNumY) { layers[i].MakeChunk(chunkNumber + cNumX); lo = true; }
                if((chunkNumber + 1) / cNumX != 0) { layers[i].MakeChunk(chunkNumber + 1); ri = true; }
                if((chunkNumber - 1) / cNumX != cNumX - 1) { layers[i].MakeChunk(chunkNumber - 1); le = true; }
                if(up && ri) { layers[i].MakeChunk(chunkNumber - cNumX + 1); }
                if(lo && ri) { layers[i].MakeChunk(chunkNumber + cNumX + 1); }
                if(lo && le) { layers[i].MakeChunk(chunkNumber + cNumX - 1); }
                if(up && le) { layers[i].MakeChunk(chunkNumber - cNumX - 1); }
                lastMakePoint = chunkNumber;
            }
        }

        
    }

    public bool ChunkUpdate(int[][] chunk, int layer, int chunkNumber) {
        
        return true;
    }


}
