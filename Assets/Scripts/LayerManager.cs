using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{

    /// <summary>
    /// ブロックIDリスト
    /// </summary>
    /// 

    public String layerName;

    public enum BLOCK_ID {
        unknown     = -1,
        air         = 0,
        cube_white  = 1,
        cube_red    = 2,
        cube_green  = 3,
        cube_blue   = 4,
        cube_black  = 5,

        glass_0     = 10
    }


    struct Chunk {
        public int[][] blocks;
    }
    Chunk[] chunks;
    int[][] blocks;
    GameObject block;
    InitialProcess ip;
    int cNumX, cNumY, cSize;
    /// <summary>
    /// 生成チャンクをログに記録
    /// </summary>
    public bool makeTest = true;
    /// <summary>
    /// テスト用ブロックを挿入
    /// </summary>
    public bool insTestCase = true;
    /// <summary>
    /// 表示透明化
    /// </summary>
    public bool transparency = false;
    private bool started = false;

    

    int[][] testcase1 = {
        new int[] { 5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        new int[] { 5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5 }
    };

    public GameObject CHUNK_FRAME;
    public GameObject chunkFrame;

    
    


    // Start is called before the first frame update


    void Start()
    {
        ip = GetComponentInParent<InitialProcess>();
        blocks =  new int[ip.chunkSize][];
        for(int i = 0; i < ip.chunkSize; i++) {
            blocks[i] = new int[ip.chunkSize];
        }


        //blocks初期化
        for(int i = 0; i < ip.chunkSize; i++) {
            for(int j = 0; j < ip.chunkSize; j++) {
                blocks[i][j] = 0;
            }
        }
        
        //chunks初期化
        cNumX = ip.chunksNumX;
        cNumY = ip.chunksNumY;
        cSize = ip.chunkSize;

        chunks = new Chunk[cNumX * cNumY];
        
        for(int i = 0; i < chunks.Length; i++) {
            if (!insTestCase) {
                chunks[i].blocks = blocks;
            }
            else {
                chunks[i].blocks = testcase1;
            }
        }
        //chunks初期化ここまで

        //CHUNK_FRAME = new GameObject();

        

        started = true;
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 指定チャンク内のブロックを生成[テスト版]
    /// </summary>
    /// <param name="chankNumber">チャンク番号</param>
    /*public void MakeChunk(int chunkNumber)
    {
        if (!started) { Start(); }


        if (makeTest)
        {
            Debug.Log(layerName + " > チャンクを生成:" + chunkNumber);
        }

        chunkFrame = Instantiate(chunkFrame);
        chunkFrame.transform.parent = transform;
        chunkFrame.name = "chunk" + chunkNumber


    }*/
    public void MakeChunk(int chunkNumber)
    {
        if (!started)
        {
            Start();
        }

        //テスト用ログ
        if (makeTest)
        {
            //Debug.Log(this.gameObject.name + " > seisei:"+ chunkNumber + " ");
        }
        //Debug.Log(name + " > "+chunkNumber + "  " + chunks.Length);
        //chunks[chunkNumber].blocks[0][0] = chunkNumber;//テスト用



        chunkFrame = Instantiate(CHUNK_FRAME);
        chunkFrame.transform.parent = this.gameObject.transform;
        chunkFrame.name = "chunk" + chunkNumber;
        
        

        Vector2Int posBase = new Vector2Int(chunkNumber % cNumX * cSize, chunkNumber / cNumX * cSize);
        Vector3Int pos = new Vector3Int(0, 0, 0);
        chunkFrame.transform.localPosition = new Vector3(posBase.x, posBase.y, 0);
        Transform frame = chunkFrame.transform;

        if (makeTest)
        {
            //Debug.Log(this.gameObject.name + " > frame seisei:" + chunkNumber + "   " + chunkFrame.name + " pos:" + chunkFrame.transform.position.x + " , " + chunkFrame.transform.position.y);
        }


        foreach (int id in Enum.GetValues(typeof(BLOCK_ID)))
        {
            //Debug.Log(" id:"+Enum.GetName(typeof(BLOCK_ID), id));

            //ブロックプレハブ取得
            block = (GameObject)Resources.Load("Blocks/" + Enum.GetName(typeof(BLOCK_ID), id));
            if (block == null) { block = (GameObject)Resources.Load("Blocks/unknown"); }

            for (int px = 0; px < cSize; px++)
            {
                for (int py = 0; py < cSize; py++)
                {
                    if (chunks[chunkNumber].blocks[py][ px] == id && id != 0)
                    {
                        pos.x = px;
                        pos.y = py;
                        block = Instantiate(block, frame);
                        block.transform.localPosition = pos;
                        block.name = px + "_" + py + "_" + Enum.GetName(typeof(BLOCK_ID), id);
                        //block.transform.SetParent(this.gameObject.transform);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 指定チャンク内のブロックを消去
    /// </summary>
    /// <param name="chunkNumber"></param>
    public void RemoveChunk(int chunkNumber) {
        try {
            Destroy(transform.Find("chunk" + chunkNumber).gameObject);
        }catch(Exception e) {
            //Debug.Log(e);
        }
    }

    /// <summary>
    /// 指定チャンクの内容を更新
    /// </summary>
    /// <param name="blocks"></param>
    /// <param name="chunkNumber"></param>
    public void UpdateChunk(int[][] blocks, int chunkNumber) {
        chunks[chunkNumber].blocks = blocks;
    }
}   

