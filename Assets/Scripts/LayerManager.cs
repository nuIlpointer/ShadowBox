using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{

    struct Chunk {
        public int[,] blocks;
    }
    Chunk[,] chunks;
    int[,] blocks;
    InitialProcess ip;
    int cNumX, cNumY;
    public bool makeTest = true;

    
    /// <summary>
    /// 表示透明化
    /// </summary>
    public bool transparency = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        ip = GetComponentInParent<InitialProcess>();
        blocks = new int[ip.chunkSize, ip.chunkSize];
        //blocks初期化
        for(int i = 0; i < ip.chunkSize; i++) {
            for(int j = 0; j < ip.chunkSize; j++) {
                blocks[i,j] = 0;
            }
        }

        //chunks初期化
        /*cNumX = ip.chunksNumX;
        cNumY = ip.chunksNumY;
        chunks = new Chunk[cNumX, cNumY];
        for(int i = 0; i < cNumX; i++) {
            for(int j = 0; j < cNumY; j++) {
                chunks[i, j].blocks = blocks;
            }
        }*/
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 指定チャンク内のブロックを生成[未実装]
    /// </summary>
    /// <param name="chankNumber">チャンク番号</param>
    public void MakeChunk(int chankNumber) {
        if (makeTest) {
            Debug.Log(chankNumber);
        }
    }

    /// <summary>
    /// 指定チャンク内のブロックを消去[未実装]
    /// </summary>
    /// <param name="chunkNumber"></param>
    public void RemoveChunk(int chunkNumber) {
        
    }

}   
