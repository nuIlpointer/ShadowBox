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
    public LayerManager[] layers = new LayerManager[5];

    private int cNumX;
    private int cNumY;
    private int cSize;
    public InitialProcess ip;
    public ShadowBoxClientWrapper wrapper;
    int lastMakePoint = -1;
    

    public bool autoSetCompnents = false;

    int[] loaded = new int[9];
    int[] lastLoad = new int[9];

    private bool started = false;


    // Start is called before the first frame update
    void Start()
    {
        if (autoSetCompnents) {
            wrapper = gameObject.GetComponent<ShadowBoxClientWrapper>();
            ip = gameObject.GetComponent<InitialProcess>();
            layers[1] = transform.Find("LayerInsideWall").GetComponent<LayerManager>();
            layers[2] = transform.Find("LayerInsideBlock").GetComponent<LayerManager>();
            layers[3] = transform.Find("LayerOutsideWall").GetComponent<LayerManager>();
            layers[4] = transform.Find("LayerOutsideBlock").GetComponent<LayerManager>();
            
        }
        cNumX = ip.chunksNumX;
        cNumY = ip.chunksNumY;
        cSize = ip.chunkSize;

        started = true;
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

        if (!started) { Start(); }


        //Debug.LogWarning(layers[1] + " " + layers[2] + " " + layers[3] + " " + layers[4]);
        int chunkNumber = ((int)pos.x / cSize) + cNumX * ((int)pos.y / cSize);
        loaded[0] = chunkNumber;
        Debug.Log("number:"+chunkNumber);
        if(chunkNumber != lastMakePoint) {
            for (int i = 1; i <= 4; i++) {
                layers[i].MakeChunk(chunkNumber);
                Debug.LogWarning(i);
                bool up = false, lo = false, le = false, ri = false;
                if((loaded[1] = chunkNumber - cNumX) >= 0) { layers[i].MakeChunk(chunkNumber - cNumX); up = true; }
                if((loaded[2] = chunkNumber + cNumX) < cNumX * cNumY) { layers[i].MakeChunk(chunkNumber + cNumX); lo = true; }
                if((loaded[3] = chunkNumber - 1) / cNumX != cNumX - 1)  { layers[i].MakeChunk(chunkNumber + 1); ri = true; }
                if((loaded[4] = chunkNumber + 1) / cNumX != 0) { layers[i].MakeChunk(chunkNumber - 1); le = true; }
                if(up && ri) { layers[i].MakeChunk(chunkNumber - cNumX + 1); }
                if(lo && ri) { layers[i].MakeChunk(chunkNumber + cNumX + 1); }
                if(lo && le) { layers[i].MakeChunk(chunkNumber + cNumX - 1); }
                if(up && le) { layers[i].MakeChunk(chunkNumber - cNumX - 1); }
                loaded[5] = loaded[1] + 1;
                loaded[6] = loaded[2] - 1;
                loaded[7] = loaded[2] + 1;
                loaded[8] = loaded[1] - 1;



                lastMakePoint = chunkNumber;
            }
        }
    }

    public bool ChunkUpdate(int[][] chunk, int layer, int chunkNumber) {
        
        return true;
    }


}
