using System;
using System.Collections;
using System.Collections.Generic;
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

    int[] loaded;
    int[] lastLoad;
    int[] liveChunk;
    bool[] visit;

    private bool started = false;
    private bool waking;
    private int loadChunksQueueID = 0;

    private double time;

    Queue<Vector2Int> chunkGetQueue = new Queue<Vector2Int>();
    Queue<KeyValuePair<int, Vector3>> loadChunksQueue = new Queue<KeyValuePair<int, Vector3>>();

    public bool wideLoadMode = false;
    public bool getChunkFromServer = true; 

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

            /* wideLoadModeの場合
             * 
             *15  8  1  5  9
             *14  4  0  2 10
             *13  7  3  6 11
            */
            loaded = new int[wideLoadMode ? 15 : 9];
            lastLoad = new int[wideLoadMode ? 15 : 9];
            liveChunk = new int[wideLoadMode ? 15 : 9];

            cNumX = ip.chunksNumX;
            cNumY = ip.chunksNumY;
            cSize = ip.chunkSize;
            heightRange = ip.heightRange;

            liveChunk = new int[wideLoadMode ? 15 : 9];
            for(int i = 0; i < liveChunk.Length; i++)liveChunk[i] = -1;  


            lastLoad = new int[wideLoadMode ? 15 : 9];
            for (int i = 0; i < liveChunk.Length; i++) lastLoad[i] = -1;


            visit = new bool[cNumX * cNumY];
            for(int i = 0; i < visit.Length; i++) {
                visit[i] = false;
            }

            started = true;




        }
    }
    private void FixedUpdate() {

        Vector2Int gc = new Vector2Int();
        
        if(chunkGetQueue.Count > 0 && wrapper.IsConnectionActive()) {
            gc = chunkGetQueue.Dequeue();
            //Debug.Log($"gc内容 {gc.x} , {gc.y}");
            if (getChunkFromServer)wrapper.GetChunk((ShadowBoxClientWrapper.BlockLayer)gc.x, gc.y);
            //layers[gc.x].MakeChunk(gc.y);
            Debug.Log($"[WorldLoader] > チャンクデータ要求 : layer : {gc.x}  chunkNumber : {gc.y}");
        }    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (waking) {
            if (wrapper.IsConnectionActive() && !wrapper.IsWorldRegenerateFinished()) {
                wrapper.SetWorldData(cNumX, cNumY, cSize, cSize, heightRange, new System.Random().Next(0, Int32.MaxValue), "new_World");
                wrapper.RequestWorldRegenerate();
                for (int i = 0; i < visit.Length; i++) visit[i] = false;
                waking = false;
                Debug.Log("[WorldLoader] > waked");
            }
        }
    }

    /// <summary>
    /// 再生成に成功したときにWrapperから呼び出されます。
    /// </summary>
    public void OnWorldRegenerateFinish() {
        // do something when world regenerate finished 
        Debug.Log("ワールドが再生成されました。");
        for (int i = 0; i < visit.Length; i++) visit[i] = false;        //visitを初期化し、再度読み込むようにする

    }

    /// <summary>
    /// 再生成が必要な時にWrapperから呼び出されます。
    /// </summary>
    public void OnWorldNeedRegenerate() {
        Debug.Log("サーバー側に地形データが存在しません。再生成が必要です。");
        wrapper.SetWorldData(cNumX, cNumY, cSize, cSize, heightRange, new System.Random().Next(0, Int32.MaxValue), "new_World");
        wrapper.RequestWorldRegenerate();
    }

    /// <summary>
    /// 沙う背う背うが不要な時にWrapperから呼び出されます。
    /// </summary>
    public void OnWorldNoNeedRegenerate() {
        Debug.Log("ワールドの再生成は不要です。");
    }


    /*public bool WakeUp() {
        if (!wrapper.IsConnectionActive()) {
            Debug.LogWarning("[WorldLoader] > 地形の初期生成に失敗（接続が確認できない）");
            return false;
        }
        if (wrapper.IsWorldRegenerateFinished()) {
            Debug.Log("[WorldLoader] > 地形はすでに生成されています");
            return false;
        }
        float timer = 0;
        int sec = 0;
        do {
            timer += Time.deltaTime;

            if (timer > sec) {
                sec++;
                Debug.Log($"[WorldLoader] >　サーバーからの生成完了応答を待っています({sec}s)");
            }
            if (sec > 10) {
                Debug.LogWarning("[WorldLoader] > 地形の初期生成リクエストの応答が１０秒間返ってきませんでした。");
                return false;
            }
        } while (!wrapper.IsWorldRegenerateFinished());

        for (int i = 0; i < visit.Length; i++) visit[i] = false;
        Debug.Log("[WorldLoader] > サーバ側生成完了を確認　地形ロード履歴をリセットしました");

        return true;
    }*/
    public void WakeUp() {
        waking = true;
    }

    /// <summary>
    ///指定位置周辺のチャンクを生成
    /// </summary>
    /// <param name="pos">基準座標を指定(vector3)</param>
    public void LoadChunks(Vector3 pos)
    {
        //UnityEngine.Debug.LogWarning("ワーーーールドッッッ・ロォーーーーードッッッ！！");
        if (!started) { Start(); }
        //サーバ非アクティブ時処理　叩いた内容をキューに保存し、次回実行時に呼び出す
        if (!wrapper.IsConnectionActive() || time < 0.1) {
            Debug.Log($"[WorldLoader] > サーバが未接続な為、ロードチャンク待ちキューに引数を保存しました ID : {loadChunksQueueID}");
            loadChunksQueue.Enqueue(new KeyValuePair<int, Vector3>(loadChunksQueueID, pos));
            loadChunksQueueID++;
            return;
        }
        if(loadChunksQueue.Count > 0) {
            while(loadChunksQueue.Count <= 0) {
                KeyValuePair<int, Vector3> lc = loadChunksQueue.Dequeue();
                LoadChunks(lc.Value);
                Debug.Log($"[WorldLoader] > ロードチャンク待ちキューからロードチャンクを実行しました。 ID : {lc.Key}");
            }
        }

        int chunkNumber = PosToChunkNum((int)pos.x, (int)pos.y);
        loaded[0] = chunkNumber;
        //UnityEngine.Debug.LogWarning(chunkNumber);

        bool up = false, lo = false, ri = false, le = false, riri = false, lele = false;

        /* wideLoadModeの場合
         * 
         *14  8  1  5 11
         *10  4  0  2  9
         *13  7  3  6 12
         *
        */

        if ((loaded[1] = chunkNumber + cNumX) >= cNumX * cNumY) { loaded[1] = -1; } else { up = true; }
        if ((loaded[2] = chunkNumber - cNumX) < 0)              { loaded[2] = -1; } else { lo = true; }
        if ((loaded[3] = chunkNumber + 1) % cNumX == 0)         { loaded[3] = -1; } else { ri = true; }
        if ((loaded[4] = chunkNumber - 1) % cNumX == cNumX - 1 || chunkNumber < 1) { loaded[4] = -1; } else { le = true; }


        if (up && ri) { loaded[5] = loaded[1] + 1; } else { loaded[5] = -1; }
        if (lo && ri) { loaded[6] = loaded[2] + 1; } else { loaded[6] = -1; }
        if (lo && le) { loaded[7] = loaded[2] - 1; } else { loaded[7] = -1; }
        if (up && le) { loaded[8] = loaded[1] - 1; } else { loaded[8] = -1; }

        //wideLoadModeがtureの場合

        if (wideLoadMode) {
            if ((loaded[9] = chunkNumber + 2) / cNumX > chunkNumber / cNumX) loaded[9] = -1; else riri = true;
            if ((loaded[10] = chunkNumber - 2) / cNumX < chunkNumber / cNumX || chunkNumber < 2) loaded[10] = -1; else lele = true;

            if (up && riri) loaded[11] = loaded[5] + 1; else loaded[11] = -1;
            if (lo && riri) loaded[12] = loaded[6] + 1; else loaded[12] = -1; 
            if (lo && lele) loaded[13] = loaded[7] - 1; else loaded[13] = -1;
            if (up && lele) loaded[14] = loaded[8] - 1; else loaded[14] = -1;
        }

        for(int i = 0; i < liveChunk.Length; i++) {
            liveChunk[i] = loaded[i];
            //UnityEngine.Debug.LogWarning($"<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<{loaded[i]}>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        }


        //死んだチャンクを検出
        for (int i = 0; i < lastLoad.Length; i++) {
            if (lastLoad[i] != -1) {
                if (checkDie(lastLoad[i])) {
                    //UnityEngine.Debug.LogWarning("消去　チャンクナンバー:" + lastLoad[i] + $"相対位置：{i}");
                    for (int j = 1; j <= 4; j++)
                        if (lastLoad[i] != -1)
                            layers[j].RemoveChunk(lastLoad[i]);
                }
            }
        }

        //UnityEngine.Debug.Log($"{loaded[0]}\t{loaded[1]}\t{loaded[2]}\t{loaded[3]}\t{loaded[4]}\t{loaded[5]}\t{loaded[6]}\t{loaded[7]}\t{loaded[8]} ");
        //ロード被り判定
        for(int i = 0; i < loaded.Length; i++){
            if (checkLoaded(loaded[i])){
                //UnityEngine.Debug.LogWarning("被り　チャンクナンバー："+loaded[i] + $"相対位置：{i}");
                loaded[i] = -1;
            }
        }

        for(int i = 0; i < lastLoad.Length; i++) {
            lastLoad[i] = liveChunk[i];
        }
        

        for(int i = 0; i < loaded.Length; i++){
            if(loaded[i] != -1){
                
                for (int j = 1; j <= 4; j++){
                    if (!visit[loaded[i]]) {
                        Debug.Log($"[WorldLoader] > チャンクデータ要求キューに追加 chunkNumber : {loaded[i]}  layer : {j}");
                        
                        
                        chunkGetQueue.Enqueue(new Vector2Int(j, loaded[i]));
                    }
                    layers[j].MakeChunk(loaded[i]);
                }
                visit[loaded[i]] = true;
            }
        }
        //if (wideLoadMode) UnityEngine.Debug.LogWarning($"0:{loaded[0]}\t1:{loaded[1]}\t2:{loaded[2]}\t3:{loaded[3]}\t4:{loaded[4]}\t5:{loaded[5]}\t6:{loaded[6]}\t7:{loaded[7]}\t8:{loaded[8]}\t9:{loaded[9]}\t10:{loaded[10]}\t11:{loaded[11]}\t12:{loaded[12]}\t13:{loaded[13]}\t14:{loaded[14]}");



    }


    /// <summary>
    /// 処理用
    /// </summary>
    /// <param name="cn"></param>
    /// <returns></returns>
    private bool checkLoaded(int cn)
    {
        for (int i = 0; i < lastLoad.Length; i++)
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
        for(int i = 0; i < loaded.Length; i++) {
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
        for (int i = 0; i < liveChunk.Length; i++) {
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

        Debug.Log("[WorldLoader] > チャンク更新 :"+layers[layerNumber] +" , " +layerNumber + $"  \n{ChunkToString(blocks)}");
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
        if (!started) Start();
        int x = (int)pos.x % cSize;
        int y = (int)pos.y % cSize;
        int cn = 0, chx = 0, chy = 0;
        bool isSafe = true;

        //判定する場所リストを設定

        float mod = pos.x - x;
        Vector2Int[] checkList;
        if(mod < 0.5) {//横3ブロックにまたがっていなければ
            checkList = new Vector2Int[6] {
                new Vector2Int(x,y),
                new Vector2Int(x+1,y),
                new Vector2Int(x,y+1),
                new Vector2Int(x+1,y+1),
                new Vector2Int(x,y+2),
                new Vector2Int(x+1,y+2)
            };
        } else {
            checkList = new Vector2Int[6] {
                new Vector2Int(x,y),
                new Vector2Int(x+1,y),
                //new Vector2Int(x+2,y),
                new Vector2Int(x,y+1),
                new Vector2Int(x+1,y+1),
                //new Vector2Int(x+2,y+1),
                new Vector2Int(x,y+2),
                new Vector2Int(x+1,y+2),
                //new Vector2Int(x+2,y+2)
            };
        }

        for(int i = 0; i < checkList.Length; i++) {
            cn = PosToChunkNum(checkList[i].x, checkList[i].y);
            chx = checkList[i].x - ChunkNumToOriginPos(cn)[0];
            chy = checkList[i].y - ChunkNumToOriginPos(cn)[1];
            if (!layers[3].checkAir(cn, chx, chy) || !layers[3].checkAir(cn, chx, chy)) {
                isSafe = false;
            }
        }

        return isSafe;
    }

    public bool CheckToBack(Vector3 pos) {

        if (!started) Start();
        int x = (int)pos.x % cSize;
        int y = (int)pos.y % cSize;
        int cn = 0, chx = 0, chy = 0;
        bool isSafe = true;

        //判定する場所リストを設定

        float mod = pos.x - x;
        Vector2Int[] checkList;
        if (mod < 0.5) {//横3ブロックにまたがっていなければ
            checkList = new Vector2Int[6] {
                new Vector2Int(x,y),
                new Vector2Int(x+1,y),
                new Vector2Int(x,y+1),
                new Vector2Int(x+1,y+1),
                new Vector2Int(x,y+2),
                new Vector2Int(x+1,y+2)
            };
        } else {
            checkList = new Vector2Int[6] {
                new Vector2Int(x,y),
                new Vector2Int(x+1,y),
                //new Vector2Int(x+2,y),
                new Vector2Int(x,y+1),
                new Vector2Int(x+1,y+1),
                //new Vector2Int(x+2,y+1),
                new Vector2Int(x,y+2),
                new Vector2Int(x+1,y+2),
                //new Vector2Int(x+2,y+2)
            };
        }

        for (int i = 0; i < checkList.Length; i++) {
            cn = PosToChunkNum(checkList[i].x, checkList[i].y);
            chx = checkList[i].x - ChunkNumToOriginPos(cn)[0];
            chy = checkList[i].y - ChunkNumToOriginPos(cn)[1];
            //Debug.Log($">>>>>>cn{cn} chx{chx} chy{chy} clx{checkList[i].x} cly{checkList[i].y}    \n{ChunkNumToOriginPos(1)[0]} {ChunkNumToOriginPos(1)[1]}");
            if (!layers[2].checkAir(cn, chx, chy) || !layers[3].checkAir(cn, chx, chy)) {
                isSafe = false;
            }
        }

        return isSafe;
    }

    public int GetBlock(int x, int y, int layerNumber) {
        int cn = PosToChunkNum(x, y);
        int dx = x - ChunkNumToOriginPos(cn)[0];
        int dy = y - ChunkNumToOriginPos(cn)[1];
        
        return layers[layerNumber].GetBlock(cn, dx, dy);
    }

    public String ChunkToString(int[][] blocks) {
        String result = "";
        for (int i = 0; i < blocks.Length; i++) {
            for (int j = 0; j < blocks[i].Length; j++) {
                result += blocks[i][j].ToString() + " ";
            }
            result += "\n";
        }
        return result;
    }
}
