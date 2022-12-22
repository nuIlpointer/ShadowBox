using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour {

    /// <summary>
    /// ブロックIDリスト
    /// </summary>
    /// 

    public String layerName;
    public bool isWall = false;

    public enum BLOCK_ID {
        unknown = -1,
        air = 0,
        cube_white = 1,
        cube_red = 2,
        cube_green = 3,
        cube_blue = 4,
        cube_black = 5,

        //自然生成（極力変えない）
        grass_0 = 10,
        stone_0 = 11,
        dirt_0 = 12,
        leaf = 13,

        clay_brick = 20,
        brick_stone = 21,
        tile = 22,
        glass = 23,

        Planks = 40,
        whiteplanks = 41,
        darkplanks = 42,

        //左右上下反転(60~79　 mod(4)が　0:デフォ　1:左右反転　2:上下反転　3:上下左右反転)


        //複数ブロック(80~)
        door_0 = 80,
        door_1 = 81,

        //制御用
        usb_0 = -2,
        usb_1 = -3,
        usb_2 = -4,
        usb_3 = -5,
        usb_4 = -6,
        usb_5 = -7,
        
    }

    Dictionary<String, Vector2Int> UNNORMAL_SIZE_BLOCKS = new Dictionary<string, Vector2Int>() {
        {"door_0", new Vector2Int(2,3)},
        {"door_1", new Vector2Int(1,3)},

    };



    struct Chunk {
        public int[][] blocks;
        public GameObject[][] blockObj;
        bool live;

    }

    Chunk[] chunks;

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

    int[][] blocks;

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

    public bool DebugChunkAct = false;

    public GameObject CHUNK_FRAME;
    public GameObject[] chunkFrame;





    // Start is called before the first frame update


    void Start() {
        string a = "-1";
        Debug.LogWarning(int.Parse(a));

        if (!started) {
            ip = GetComponentInParent<InitialProcess>();
            blocks = new int[ip.chunkSize][];
            for (int i = 0; i < ip.chunkSize; i++) {
                blocks[i] = new int[ip.chunkSize];
            }


            //blocks初期化
            for (int i = 0; i < ip.chunkSize; i++) {
                for (int j = 0; j < ip.chunkSize; j++) {
                    blocks[i][j] = 0;
                }
            }

            //chunks初期化
            cNumX = ip.chunksNumX;
            cNumY = ip.chunksNumY;
            cSize = ip.chunkSize;

            chunks = new Chunk[cNumX * cNumY];
            chunkFrame = new GameObject[chunks.Length];



            for (int i = 0; i < chunks.Length; i++) {
                if (!insTestCase) chunks[i].blocks = blocks;
                else chunks[i].blocks = testcase1;

                chunks[i].blockObj = new GameObject[cSize][];
                for (int j = 0; j < cSize; j++) {
                    chunks[i].blockObj[j] = new GameObject[cSize];
                }

                chunkFrame[i] = Instantiate(CHUNK_FRAME);
                chunkFrame[i].transform.parent = this.gameObject.transform;
                chunkFrame[i].name = "chunk" + i;



                Vector2Int posBase = new Vector2Int(i % cNumX * cSize, i / cNumX * cSize);
                chunkFrame[i].transform.localPosition = new Vector3(posBase.x, posBase.y, 0);
            }
            //chunks初期化ここまで

            //CHUNK_FRAME = new GameObject();




            started = true;
        }
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
            //Debug.Log(layerName + " > チャンクを生成:" + chunkNumber);
        }

        chunkFrame = Instantiate(chunkFrame);
        chunkFrame.transform.parent = transform;
        chunkFrame.name = "chunk" + chunkNumber


    }*/
    public void MakeChunk(int chunkNumber) {
        if (!started) {
            Start();
        }

        //テスト用ログ
        if (makeTest) {
            //Debug.Log(this.gameObject.name + " > seisei:"+ chunkNumber + " ");
        }

        Vector3Int pos = new Vector3Int(0, 0, 0);
        Transform frame = chunkFrame[chunkNumber].transform;

        //if (makeTest)Debug.Log(this.gameObject.name + " > frame seisei:" + chunkNumber + "   " + chunkFrame.name + " pos:" + chunkFrame.transform.position.x + " , " + chunkFrame.transform.position.y);

        BoxCollider bcl;

        foreach (int id in Enum.GetValues(typeof(BLOCK_ID))) {
            //Debug.Log(" id:"+Enum.GetName(typeof(BLOCK_ID), id));

            //ブロックプレハブ取得
            block = (GameObject)Resources.Load("Blocks/" + Enum.GetName(typeof(BLOCK_ID), id));
            if (block == null) { block = (GameObject)Resources.Load("Blocks/unknown"); }

            for (int px = 0; px < cSize; px++) {
                for (int py = 0; py < cSize; py++) {
                    if (chunks[chunkNumber].blocks[py][px] == id && id != 0) {
                        pos.x = px;
                        pos.y = py;
                        block = Instantiate(block, frame);
                        block.transform.localPosition = pos;
                        block.name = px + "_" + py + "_" + Enum.GetName(typeof(BLOCK_ID), id);


                        block.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
                        if (isWall) {
                            bcl = block.GetComponent<BoxCollider>();
                            if (bcl != null) Destroy(bcl);
                        }

                        chunks[chunkNumber].blockObj[py][px] = block;
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
            //Debug.Log(name + " > リムーブ："+ chunkNumber);
            Destroy(chunkFrame[chunkNumber]);
            chunkFrame[chunkNumber] = Instantiate(CHUNK_FRAME);
            chunkFrame[chunkNumber].transform.parent = this.gameObject.transform;
            chunkFrame[chunkNumber].name = "chunk" + chunkNumber;

            Vector2Int posBase = new Vector2Int(chunkNumber % cNumX * cSize, chunkNumber / cNumX * cSize);
            chunkFrame[chunkNumber].transform.localPosition = new Vector3(posBase.x, posBase.y, 0);

        } catch (Exception e) {
            //Debug.Log(e);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blockID"></param>
    /// <param name="chunkNumber"></param>
    /// <param name="x">！チャンク内の座標</param>
    /// <param name="y">！チャンク内の座標</param>
    public void BlockChange(int id, int chunkNumber, int x, int y) {


        //既存ブロックを削除
        int oldBlockID = GetBlock(chunkNumber, x, y);
        if (oldBlockID > 0) {      //指定位置にブロックが存在

            Destroy(chunks[chunkNumber].blockObj[y][x]);

        }

        if(oldBlockID <= (int)Enum.Parse(typeof(BLOCK_ID), "usb_0") && oldBlockID >= (int)Enum.Parse(typeof(BLOCK_ID), "usb_5")) {
            //アルゴリズム：IDが基のブロックのY座標を示している　そこから左（x-方向）に遡っていくと基のブロックにぶつかる
            int usbStart = (int)Enum.Parse(typeof(BLOCK_ID), "usb_0");
            int[] usbID = { usbStart, usbStart - 1, usbStart - 2, usbStart - 3, usbStart - 4 };
            int searchX = x, searchY;
            int i;

            for(i = usbStart; i < usbID.Length + usbStart; i++) if (id == usbID[i]) break;
            searchY = i;
            int searchStertChunk = chunkNumber;
            while (chunks[chunkNumber].blocks[searchY][searchX] >= 80) {
                searchX--;
                if (searchX < 0) { 
                    searchX = cSize - 1;
                    if (!(searchStertChunk % cNumX == 0)) searchStertChunk--;
                    else {
                        Debug.LogError($"[LayerManager {name}] > Unnormal size block origin search error : 特殊サイズブロックの削除において、ブロック原点を探索しましたが、見つかりませんでした。（x : {x} y : {y}）/n" +
                                    $"Description : 探索がワールド外に出ました");
                    }
                }
            }

            Vector2Int blockSize;
            if (Enum.GetName(typeof(BLOCK_ID), chunks[chunkNumber].blocks[searchY][searchX]) != null ) {
                 blockSize = UNNORMAL_SIZE_BLOCKS[Enum.GetName(typeof(BLOCK_ID), chunks[chunkNumber].blocks[searchY][searchX])];
            } else {
                Debug.LogError($"[LayerManager {name}] > Unnormal size block origin search error : 特殊サイズブロックの削除において、ブロック原点を探索しましたが、見つかりませんでした。（x : {x} y : {y}）/n" +
                                    $"Description : id {blocks[searchY][searchX]} に該当する特殊サイズブロックが見つかりませんでした。 列挙型BLOCK_IDか、Dictionary型UNNORMAL_SIZE_BLOCKSに記述がない可能性があります 座標:({searchX},{searchY})");
                return;
            }
            
            for(i = 0; i < blockSize.y; i++) {
                for(int j = 0; j < blockSize.x; j++) {
                    //111111111111111111111111111111111111111111111111111111111111air置く
                }
            }

        }



        chunks[chunkNumber].blocks[y][x] = id;
        if (id != 0) {                          //指定IDがair以外

            block = (GameObject)Resources.Load("Blocks/" + Enum.GetName(typeof(BLOCK_ID), id));
            if (block == null) { block = (GameObject)Resources.Load("Blocks/unknown"); }

            Transform frame = chunkFrame[chunkNumber].transform;
            block = Instantiate(block, frame);
            block.transform.localPosition = new Vector3(x, y, 0);
            block.name = x + "_" + y + "_" + Enum.GetName(typeof(BLOCK_ID), id);
            block.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
            BoxCollider bcl;
            if (isWall) {
                bcl = block.GetComponent<BoxCollider>();
                if (bcl != null) Destroy(bcl);
            }

            chunks[chunkNumber].blockObj[y][x] = block;

            //id = unnormal size block の時

        }



    }

    /// <summary>
    /// 指定チャンクの内容を更新
    /// </summary>
    /// <param name="blocks"></param>
    /// <param name="chunkNumber"></param>
    public void UpdateChunk(int[][] blocks, int chunkNumber) {
        if (!started) Start();
        chunks[chunkNumber].blocks = blocks;

        String ch = $"cn : {chunkNumber}\n";

        if (DebugChunkAct) {
            for (int i = 0; i < blocks.Length; i++) {
                for (int j = 0; j < blocks[i].Length; j++) {
                    ch += blocks[i][j].ToString() + ",";
                }
                ch += "\n";
            }
        }





    }

    public bool checkAir(int cn, int x, int y) {
        if (chunks[cn].blocks[y][x] == 0) {
            return true;
        } else {
            return false;
        }
    }

    public int GetBlock(int chunkNumber, int x, int y) {
        return (x >= 0 && y >= 0 && y < chunks[chunkNumber].blocks.Length && x < chunks[chunkNumber].blocks[y].Length) ? chunks[chunkNumber].blocks[y][x] : 0;
    }
}