using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public class generaTester : MonoBehaviour
{
    WorldLoader wl;
    LayerManager[] layers;
    public int chunkNumber = 3;
    public ShadowBoxClientWrapper wrapper;
    //playertest
    public int inLayer = 2;
    public Vector3 inPos;
    public int actState = 0;
    // Start is called before the first frame update

    int[][] testcase1 = {
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,2,2,2,2,2,2,2,2,2,2,2,2,2,2,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 }
        };

    int[][] ground = {
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    int[][] hut1 = {
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,20,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };
    int[][] hut2 = {
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 0,0,0,0,0,0,0,0,20,20,31,0,20,0,0,0,0,0,0,20,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,0,0,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,0,0,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,20,20,20,20,20,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    public GenericEntityManager em;
    float synclate = 0;

    int f = 0;
    bool firstServerConnectFrame = true;

    
    void Start()
    {

        //id = Guid.NewGuid() ;
        inPos = new Vector3(0, 0, 0);
        wl = GetComponent<WorldLoader>();


        //wl.LoadChunks(new Vector2((float)30.0,(float)10.0));//chunkNumber 1(左から2番目下から１番目)
        //wl.LoadChunks(new Vector2((float)55.0, (float)10.0));//chunkNumber 2(左から3番目下から１番目)


        //----------------------------------------ここから

        UnityEngine.Debug.Log("generaTester > ロードチャンク　chunkNumber : " + 0);
        wl.LoadChunks(new Vector3((float)20.0, (float)20.0, 0));
        for (int i = 0; i < 4; i++) {
            UnityEngine.Debug.Log("generaTester > グラウンド入れる　chunkNumber : " + i);
            for (int j = 1; j <= 4; j++) {
                wl.ChunkUpdate(ground, j, i);
            }
        }

        wl.ChunkUpdate(hut1, 1, 1);
        wl.ChunkUpdate(hut2, 3, 1);
        UnityEngine.Debug.Log("generaTester > ロードチャンク　chunkNumber : " + 0);
        wl.LoadChunks(new Vector3((float)20.0, (float)20.0, 0));

        //em.AddPlayer(id, "mememe", 0);
        //Debug.LogError("");

        //--------------------------------------ここまでコメントアウトで地形プリセットオフ
    }

    // Update is called once per frame
    void Update()
    {
        //ワールド再生成前に呼ばれてしまうため没
        /*if(firstServerConnectFrame && wrapper.IsConnectionActive()) {
            Debug.Log($"[generaTester] >　サーバへ小屋を保存　chunkNunber : 0");
            wrapper.SendChunk((ShadowBoxClientWrapper.BlockLayer)1, 0, hut1);
            wrapper.SendChunk((ShadowBoxClientWrapper.BlockLayer)2, 0, ground);
            wrapper.SendChunk((ShadowBoxClientWrapper.BlockLayer)3, 0, hut2);
            wrapper.SendChunk((ShadowBoxClientWrapper.BlockLayer)4, 0, ground);
            firstServerConnectFrame = false;
        }*/




        if (Input.GetKeyDown(KeyCode.K)) {
            UnityEngine.Debug.Log("入力ｋ　てすとけーす");
            wl.ChunkUpdate(testcase1, 4, 0);

        }
        if (Input.GetKeyDown(KeyCode.L)) {
            UnityEngine.Debug.Log("入力L　ろーど1");
            wrapper.SendChunk((ShadowBoxClientWrapper.BlockLayer)1, 10, hut1);
            wrapper.SendChunk((ShadowBoxClientWrapper.BlockLayer)2, 10, ground);
            wrapper.SendChunk((ShadowBoxClientWrapper.BlockLayer)3, 10, hut2);
            wrapper.SendChunk((ShadowBoxClientWrapper.BlockLayer)4, 10, ground);
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            UnityEngine.Debug.Log("入力J　ろーど2");

            wl.LoadChunks(new Vector2((float)55.0, (float)10.0));//chunkNumber 2(左から3番目下から１番目)
            wl.LoadChunks(new Vector2((float)55.0, (float)10.0));//chunkNumber 2(左から3番目下から１番目)
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            UnityEngine.Debug.Log("入力B　ブロック変更11 4 20 20");
            wrapper.SendBlockChange((ShadowBoxClientWrapper.BlockLayer)2, 20, 18, 4);
            //wl.BlockUpdate(11, 4, 20, 20);
        }


        /*synclate += Time.deltaTime;
        if (synclate > 0.01) {
            synclate = 0;
            //UnityEngine.Debug.Log("kkkk");
            em.SyncPlayer(id, inPos.x + 5, inPos.y, 2, actState);

        }*/

        //wl.LoadChunks(new Vector3((float)20.0, (float)20.0, 0));

    }
}