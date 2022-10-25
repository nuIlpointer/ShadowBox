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



    // Start is called before the first frame update
    void Start()
    {
        
        InitialProcess ip = gameObject.GetComponent<InitialProcess>();
        layers[1] = transform.Find("LayerInsideWall").GetComponent<LayerManager>();
        layers[2] = transform.Find("LayerInsideBlock").GetComponent<LayerManager>();
        layers[3] = transform.Find("LayerOutsideWall").GetComponent<LayerManager>();
        layers[4] = transform.Find("LayerOutsideBlock").GetComponent<LayerManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    ///指定位置周辺のチャンクを生成
    /// </summary>
    /// <param name="chunkPos">マップ位置を指定</param>
    public void LoadChunks(Vector2 chunkPos)
    {
        
    }

    public bool BlockMake(int x, int y, int bData)
    {
        
        return true;
    }


}
