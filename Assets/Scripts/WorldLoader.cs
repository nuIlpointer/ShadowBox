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

    private bool[] liveChunk;


    // Start is called before the first frame update
    void Start()
    {
        if (autoSetCompnents)
        {
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

        liveChunk = new bool[9];
        
        
        lastLoad = new int[]{-1,-1,-1,-1,-1,-1,-1,-1,-1 };
        

        started = true;
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
        if (!started) { Start(); }

        int chunkNumber = PosToChunkNum((int)pos.x, (int)pos.y);
        loaded[0] = chunkNumber;

        bool up = false, lo = false, ri = false, le = false;

        if ((loaded[1] = chunkNumber + cNumX) >= cNumX * cNumY) { loaded[1] = -1; } else { up = true; }
        if ((loaded[2] = chunkNumber - cNumX) < 0)              { loaded[2] = -1; } else { lo = true; }
        if ((loaded[3] = chunkNumber + 1) % cNumX == 0)         { loaded[3] = -1; } else { ri = true; }
        if ((loaded[4] = chunkNumber - 1) % cNumX == cNumX - 1) { loaded[4] = -1; } else { le = true; }

        if (up && ri) { loaded[5] = loaded[1] + 1; } else { loaded[5] = -1; }
        if (lo && ri) { loaded[6] = loaded[2] + 1; } else { loaded[6] = -1; }
        if (up && ri) { loaded[7] = loaded[2] - 1; } else { loaded[7] = -1; }
        if (up && ri) { loaded[8] = loaded[1] - 1; } else { loaded[8] = -1; }

        //ロード被り判定
        for(int i = 0; i < 9; i++){
            if (checkLoaded(loaded[i])){
                loaded[i] = -1;
            }
        }

        //死んだチャンクを検出
        for(int i = 0; i < 9; i++) {
            if(lastLoad[i] != -1) {
                if (checkDie(lastLoad[i])) {
                    for (int j = 1; j <= 4; j++) {
                        layers[j].RemoveChunk(lastLoad[i]);
                        Debug.Log(layers[j].name + "> 消去　チャンクナンバー:" + lastLoad[i]);
                    }
                }
            }
        }


        lastLoad = loaded;

        for(int i = 0; i < 9; i++){
            if(loaded[i] != -1)
            {
                for(int j = 1; j <= 4; j++)
                {
                    Debug.Log(layers[j].name + "> 生成　チャンクナンバー:" + loaded[i]);
                    layers[j].MakeChunk(loaded[i]);
                }
            }
        }



    }
    /*public void LoadChunks(Vector2 pos)
    {

        if (!started) { Start(); }




        //Debug.LogWarning(layers[1] + " " + layers[2] + " " + layers[3] + " " + layers[4]);
        int chunkNumber = ((int)pos.x / cSize) + cNumX * ((int)pos.y / cSize);
        loading[0] = chunkNumber;
        Debug.Log("number:"+chunkNumber);
        if(chunkNumber != lastMakePoint) {
            for (int i = 1; i <= 4; i++) {
                layers[i].MakeChunk(chunkNumber);
                Debug.LogWarning(i);
                bool up = false, lo = false, le = false, ri = false;
                if((loading[1] = chunkNumber - cNumX) >= 0) {
                    if (!checkLoaded(loading[1])) { layers[i].MakeChunk(loading[1]); } 
                    up = true; 
                }
                if((loading[2] = chunkNumber + cNumX) < cNumX * cNumY) {
                    if (!checkLoaded(loading[2])) { layers[i].MakeChunk(loading[2]); }
                    lo = true; 
                }
                if((loading[3] = chunkNumber - 1) / cNumX != cNumX - 1)  {
                    if (!checkLoaded(loading[3])) { layers[i].MakeChunk(loading[3]); }
                    ri = true;
                }
                if((loading[4] = chunkNumber + 1) / cNumX != 0) {
                    if (!checkLoaded(loading[4])) { layers[i].MakeChunk(loading[4]); }
                    le = true;
                }
                if(!checkLoaded((loading[5] = chunkNumber - cNumX + 1)) && up && ri) layers[i].MakeChunk(loading[5]); 
                if(!checkLoaded((loading[6] = chunkNumber + cNumX + 1)) && lo && ri) layers[i].MakeChunk(loading[6]); 
                if(!checkLoaded((loading[7] = chunkNumber + cNumX - 1)) && lo && le) layers[i].MakeChunk(loading[7]);
                if(!checkLoaded((loading[8] = chunkNumber - cNumX - 1)) && up && le) layers[i].MakeChunk(loading[8]);

                for(int j = 0; j < 9; j++){
                    bool diedChunk = false;
                    for(int k = 0; k < 9; k++){
                        if(lastLoad[j] == loading[k]){
                            diedChunk = true; Debug.Log("loaded:"+lastLoad[i]);
                        }
                    }
                    if (diedChunk){
                        layers[i].RemoveChunk(lastLoad[j]);
                    }
                }

                lastLoad = loaded;


                lastMakePoint = chunkNumber;
            }
        }
    }
    */
    public bool checkLoaded(int cn)
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

    public bool checkDie(int cn) {
        for(int i = 0; i < 9; i++) {
            if(loaded[i] == cn) { return false; }
        }
        return true;
    }

    /// <summary>
    /// チャンクを更新します（未実装）
    /// </summary>
    /// <param name="blocks"></param>
    /// <param name="layer"></param>
    /// <param name="chunkNumber"></param>
    /// <returns></returns>
    public bool ChunkUpdate(int[][] blocks, int layer, int chunkNumber)
    {

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
