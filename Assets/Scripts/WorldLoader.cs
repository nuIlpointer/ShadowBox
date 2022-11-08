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
    int[] liveChunk = new int[9];

    private bool started = false;

    


    // Start is called before the first frame update
    void Start()
    {
        if (!started) {

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

            liveChunk = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };


            lastLoad = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };


            started = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    ///指定位置周辺のチャンクを生成
    /// </summary>
    /// <param name="pos">基準座標を指定(vector3)</param>
    public void LoadChunks(Vector3 pos)
    {
        //Debug.LogWarning("ワーーーールドッッッ・ロォーーーーードッッッ！！");
        if (!started) { Start(); }

        int chunkNumber = PosToChunkNum((int)pos.x, (int)pos.y);
        loaded[0] = chunkNumber;
        //Debug.LogWarning(chunkNumber);

        bool up = false, lo = false, ri = false, le = false;

        if ((loaded[1] = chunkNumber + cNumX) >= cNumX * cNumY) { loaded[1] = -1; } else { up = true; }
        if ((loaded[2] = chunkNumber - cNumX) < 0)              { loaded[2] = -1; } else { lo = true; }
        if ((loaded[3] = chunkNumber + 1) % cNumX == 0)         { loaded[3] = -1; } else { ri = true; }
        if ((loaded[4] = chunkNumber - 1) % cNumX == cNumX - 1 || chunkNumber == 0) { loaded[4] = -1; } else { le = true; }

        if (up && ri) { loaded[5] = loaded[1] + 1; } else { loaded[5] = -1; }
        if (lo && ri) { loaded[6] = loaded[2] + 1; } else { loaded[6] = -1; }
        if (lo && le) { loaded[7] = loaded[2] - 1; } else { loaded[7] = -1; }
        if (up && le) { loaded[8] = loaded[1] - 1; } else { loaded[8] = -1; }
        
        for(int i = 0; i < 9; i++) {
            liveChunk[i] = loaded[i];Debug.LogWarning($"<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<{loaded[i]}>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        }


        //死んだチャンクを検出
        for (int i = 0; i < 9; i++) {
            if (lastLoad[i] != -1) {
                if (checkDie(lastLoad[i])) {
                    Debug.Log("消去　チャンクナンバー:" + lastLoad[i]);
                    for (int j = 1; j <= 4; j++) {
                        layers[j].RemoveChunk(lastLoad[i]);
                        
                    }
                }
            }
        }

        //Debug.Log($"{loaded[0]} {loaded[1]} {loaded[2]} {loaded[3]} {loaded[4]} {loaded[5]} {loaded[6]} {loaded[7]} {loaded[8]} ");
        //ロード被り判定
        for(int i = 0; i < 9; i++){
            if (checkLoaded(loaded[i])){
                Debug.Log("被り　チャンクナンバー："+loaded[i]);
                loaded[i] = -1;
            }
        }

        //Debug.Log($"{loaded[0]} {loaded[1]} {loaded[2]} {loaded[3]} {loaded[4]} {loaded[5]} {loaded[6]} {loaded[7]} {loaded[8]} ");
        

        for(int i = 0; i < 9; i++) {
            lastLoad[i] = liveChunk[i];
        }
        

        for(int i = 0; i < 9; i++){
            if(loaded[i] != -1){
                Debug.Log("生成　チャンクナンバー:" + loaded[i]);
                for (int j = 1; j <= 4; j++){
                    
                    layers[j].MakeChunk(loaded[i]);
                }
            }
        }



    }
    
    
    /// <summary>
    /// 処理用
    /// </summary>
    /// <param name="cn"></param>
    /// <returns></returns>
    private bool checkLoaded(int cn)
    {
        for (int i = 0; i < 9; i++)
        {
            if (lastLoad[i] == cn)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 処理用
    /// </summary>
    /// <param name="cn"></param>
    /// <returns></returns>
    private bool checkDie(int cn) {
        for(int i = 0; i < 9; i++) {
            if(loaded[i] == cn) { return false; }
        }
        return true;
    }

    /// <summary>
    /// 処理用
    /// </summary>
    /// <param name="cn"></param>
    /// <returns></returns>
    private bool checkLive(int cn) {
        for (int i = 0; i < 9; i++) {
            //Debug.Log(liveChunk[i]);
            if (liveChunk[i] == cn) {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// チャンクを更新します（実装中）
    /// </summary>
    /// <param name="blocks">チャンクの内容をブロックidの2次元配列(ジャグ)で渡します</param>
    /// <param name="layer">レイヤーを指定します[1:LayerInsideWall 2:LayerInsideBlock 3:LayerOutsideWall 4:LayerOutsideBlock]</param>
    /// <param name="chunkNumber">チャンクナンバーを指定します</param>
    /// <returns></returns>
    public bool ChunkUpdate(int[][] blocks, int layerNumber, int chunkNumber)
    {
        if (!started) { Start();}
        //Debug.Log($"{layers[1]} {layers[2]} {layers[3]} {layers[4]} {layerNumber}");

        layers[layerNumber].UpdateChunk(blocks, chunkNumber);
        
        Debug.Log($"checkLive({chunkNumber}):"+checkLive(chunkNumber));
        if (checkLive(chunkNumber)) {
            layers[layerNumber].RemoveChunk(chunkNumber);
            layers[layerNumber].MakeChunk(chunkNumber);
        }
        return true;
    }


    /// <summary>
    /// ワールド座標をチャンクナンバーに変換して返します
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int PosToChunkNum(int x, int y)
    {
        if (!started)
        {
            Start();
        }
        int ChunkNum = (x / cSize) + cNumX * (y / cSize);
        return ChunkNum;
    }


    /// <summary>
    /// チャンクの基点のワールド座標を返します　チャンク左下が始点となります
    /// </summary>
    /// <param name="ChunkNumber"></param>
    /// <returns>pos[2]{x,y}</returns>
    public int[] ChunkNumToOriginPos(int ChunkNumber)
    {
        if (!started) { Start(); }
        int[] pos = new int[2];
        pos[0] = ChunkNumber % cNumX * cSize;
        pos[1] = ChunkNumber / cNumX * cSize;
        return pos;
    }


}
