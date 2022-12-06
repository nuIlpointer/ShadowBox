using System;
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
    private int heightRange;
    public InitialProcess ip;
    public GameObject wObj;

    private ShadowBoxClientWrapper wrapper;
    int lastMakePoint = -1;


    public bool autoSetCompnents = false;

    int[] loaded = new int[9];
    int[] lastLoad = new int[9];
    int[] liveChunk = new int[9];
    bool[] visit;

    private bool started = false;

    


    // Start is called before the first frame update
    void Start()
    {
        if (!started) {
            wrapper = wObj.GetComponent<ShadowBoxClientWrapper>();

            if (autoSetCompnents) {
                //wrapper = gameObject.GetComponent<ShadowBoxClientWrapper>();
                ip = gameObject.GetComponent<InitialProcess>();
                layers[1] = transform.Find("LayerInsideWall").GetComponent<LayerManager>();
                layers[2] = transform.Find("LayerInsideBlock").GetComponent<LayerManager>();
                layers[3] = transform.Find("LayerOutsideWall").GetComponent<LayerManager>();
                layers[4] = transform.Find("LayerOutsideBlock").GetComponent<LayerManager>();

            }

            cNumX = ip.chunksNumX;
            cNumY = ip.chunksNumY;
            cSize = ip.chunkSize;
            heightRange = ip.heightRange;

            liveChunk = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };


            lastLoad = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };


            visit = new bool[cNumX * cNumY];
            for(int i = 0; i < visit.Length; i++) {
                visit[i] = false;
            }

            started = true;
        }
    }

    // Update is called once per frame
    /*void Update()
    {

    }*/

    /// <summary>
    /// 再生成に成功したときにWrapperから呼び出されます。
    /// </summary>
    public void OnWorldRegenerateFinish() {
        // do something when world regenerate finished 
        Debug.Log("ワールドが再生成されました。");
    }

    /// <summary>
    /// 再生成が必要な時にWrapperから呼び出されます。
    /// </summary>
    public void OnWorldNeedRegenerate() {
        Debug.Log("サーバー側に地形データが存在しません。再生成が必要です。");
        wrapper.SetWorldData(cNumX, cNumY, cSize, cSize, heightRange, new System.Random().Next(0, Int32.MaxValue), "new_World");
        wrapper.RequestWorldRegenerate();
    }

    public bool WakeUp() {
        if (!wrapper.IsConnectionActive()) {
            Debug.LogWarning("[WorldLoader] > 地形の初期生成に失敗（接続が確認できない）");
            return false;
        }
        /*if (wrapper.GetWorldGenerated()) {
            Debug.Log("[WorldLoader] > 地形はすでに生成されています");
            return false;
        }*/
        float timer = 0;
        int sec = 0;
        while (wrapper.IsWorldRegenerateFinished()) {
            timer += Time.deltaTime;

            if (timer > sec) {
                sec++;
                Debug.Log($"[WorldLoader] >　サーバーからの生成完了応答を待っています({sec}s)");
            }
            if (sec > 10) {
                Debug.LogWarning("[WorldLoader] > 地形の初期生成リクエストの応答が１０秒間返ってきませんでした。");
                return false;
            }
        }

        for(int i = 0; i < visit.Length; i++) visit[i] = false;
        Debug.Log("[WorldLoader] > サーバ側生成完了を確認　地形ロード履歴をリセットしました");

        return true;
    }
    

    /// <summary>
    ///指定位置周辺のチャンクを生成
    /// </summary>
    /// <param name="pos">基準座標を指定(vector3)</param>
    public void LoadChunks(Vector3 pos)
    {
        //UnityEngine.Debug.LogWarning("ワーーーールドッッッ・ロォーーーーードッッッ！！");
        if (!started) { Start(); }
        int chunkNumber = PosToChunkNum((int)pos.x, (int)pos.y);
        loaded[0] = chunkNumber;
        //UnityEngine.Debug.LogWarning(chunkNumber);

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
            liveChunk[i] = loaded[i];//UnityEngine.Debug.LogWarning($"<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<{loaded[i]}>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        }


        //死んだチャンクを検出
        for (int i = 0; i < 9; i++) {
            if (lastLoad[i] != -1) {
                if (checkDie(lastLoad[i])) {
                    //UnityEngine.Debug.Log("消去　チャンクナンバー:" + lastLoad[i]);
                    for (int j = 1; j <= 4; j++) {
                        layers[j].RemoveChunk(lastLoad[i]);
                        
                    }
                }
            }
        }

        //UnityEngine.Debug.Log($"{loaded[0]} {loaded[1]} {loaded[2]} {loaded[3]} {loaded[4]} {loaded[5]} {loaded[6]} {loaded[7]} {loaded[8]} ");
        //ロード被り判定
        for(int i = 0; i < 9; i++){
            if (checkLoaded(loaded[i])){
                //UnityEngine.Debug.Log("被り　チャンクナンバー："+loaded[i]);
                loaded[i] = -1;
            }
        }

        //UnityEngine.Debug.Log($"{loaded[0]} {loaded[1]} {loaded[2]} {loaded[3]} {loaded[4]} {loaded[5]} {loaded[6]} {loaded[7]} {loaded[8]} ");
        

        for(int i = 0; i < 9; i++) {
            lastLoad[i] = liveChunk[i];
        }
        

        for(int i = 0; i < 9; i++){
            if(loaded[i] != -1){
                //UnityEngine.Debug.Log("生成　チャンクナンバー:" + loaded[i]);
                for (int j = 1; j <= 4; j++){
                    if (!visit[loaded[i]]) {
                        Debug.Log($"チャンクデータ要求 {loaded[i]}");
                        
                        wrapper.GetChunk((ShadowBoxClientWrapper.BlockLayer)j, loaded[i]);

                        visit[loaded[i]] = true;
                    }
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
            //UnityEngine.Debug.Log(liveChunk[i]);
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
        //UnityEngine.Debug.Log($"{layers[1]} {layers[2]} {layers[3]} {layers[4]} {layerNumber}");

        layers[layerNumber].UpdateChunk(blocks, chunkNumber);
        
        //UnityEngine.Debug.Log($"checkLive({chunkNumber}):"+checkLive(chunkNumber));
        if (checkLive(chunkNumber)) {
            layers[layerNumber].RemoveChunk(chunkNumber);
            layers[layerNumber].MakeChunk(chunkNumber);
        }
        return true;
    }

    /// <summary>
    /// 指定座標のブロックを置き換えます
    /// </summary>
    /// <param name="blockID">置き換えるブロックのブロックid</param>
    /// <param name="LayerNumber">対象のレイヤー</param>
    /// <param name="x">置き換える位置（絶対座標）</param>
    /// <param name="y">置き換える位置（絶対座標）</param>
    /// <returns></returns>
    public bool BlockUpdate(int blockID, int layerNumber, int x, int y) {
        layers[layerNumber].BlockChange(blockID, PosToChunkNum(x, y), x - ChunkNumToOriginPos(PosToChunkNum(x, y))[0], y - ChunkNumToOriginPos(PosToChunkNum(x, y))[1]);
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


    public bool CheckToFront(Vector3 pos) {
        int cn = PosToChunkNum((int)pos.x, (int)pos.y);
        int x = (int)pos.x % cSize;
        int y = (int)pos.y % cSize;
        return layers[3].checkAir(cn, x, y) && layers[4].checkAir(cn, x, y);
    }

    public bool CheckToBack(Vector3 pos) {
        int cn = PosToChunkNum((int)pos.x, (int)pos.y);
        int x = (int)pos.x % cSize;
        int y = (int)pos.y % cSize;
        return layers[1].checkAir(cn, x, y) && layers[2].checkAir(cn, x, y);
    }


}
