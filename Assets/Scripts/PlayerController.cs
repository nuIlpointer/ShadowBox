using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum Skins {
        error_man = 0,
        test_kun_1 = 1
    }
    
    
    
    public GameObject wrapperObject;
    public GameObject serverObject;
    public ShadowBoxServer server;
    public ShadowBoxClientWrapper wrapper;
    public WorldLoader worldLoader;
    public CharacterController controller;
    public Animator anim;
    public string ipAddress = "127.0.0.1";
    public int port = 11781;
    public string playerName = "Player";
    public int skinID;
    private int oldSkinID;
    public bool createServer = false;

    /// <summary>
    /// プレイヤーデータ送信レート
    /// </summary>
    public float syncTimeLate = (float)0.05;
    float syncCnt = 0;
    

    /// <summary>
    /// 1秒あたりの移動量を記憶
    /// </summary>
    private Vector3 movedir = new Vector3(0,0,0);
    
    private float jumpCnt = 0;
    private float fallCnt = 0;
    private float loadCnt = 0;

    //移動制御等
    private bool runR = false, runL = false, jump = false, moveB = false, moveF = false;
    private int inLayer = 2;

    //プレイヤーstate
    /// <summary>
    /// プレイヤーのアクションによって変動する値です（ 1の位{ 0:Standby 1:run 3:jump 4:fall } 10の位{ 0:右 1:左} ）
    /// </summary>
    public int actState;
    

    bool started = false;

    //test
    public bool testUseWrapper = true;
    public generaTester gt;

    private bool firstUpdate = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!started)
        {
            //クライアントサーバ系
            wrapper = wrapperObject.GetComponent<ShadowBoxClientWrapper>();
            if(createServer) {
                server = Instantiate(serverObject).GetComponent<ShadowBoxServer>();
                server.CreateInternalServer();
                ipAddress = "127.0.0.1";
            }
            wrapper.Connect(ipAddress, port);

            //スキンid
            oldSkinID = skinID;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (firstUpdate)
        {
            firstUpdate = false;
            wrapper.SetPlayerData(playerName, skinID, 0, transform.position.x, transform.position.y, ShadowBoxClientWrapper.BlockLayer.InsideBlock);
        }
        //スキンID変更時処理
        if(oldSkinID != skinID) {
            string sid = Enum.GetName(typeof(Skins), skinID);
            anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Characters/Animator/error_man/error_man_entity");
            oldSkinID = skinID;
        }



        //移動
        //(ミスによりrunLが右移動、runRが左移動になっています)
        runR = false;
        runL = false;

        if (Input.GetKey(KeyCode.A)) {
            runR = true;
        }
        if (Input.GetKey(KeyCode.D)) {
            runL = true;
        }

        //ジャンプ
        if(Input.GetKey(KeyCode.Space)) {
            jumpCnt += Time.deltaTime;
            if(jumpCnt < 0.4 ) {
                if (controller.isGrounded) {
                    jump = true;
                }
            }
            else {
                jump = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            jump = false;
            jumpCnt = (float)0.5;
        }
        if (controller.isGrounded) {
            jumpCnt = 0;
        }

        //キャラ反転
        if(Input.GetKeyDown(KeyCode.D)) {
            transform.localScale = new Vector3(1, 1, 1);
            //actStateセット
            actState %= 10;
            actState += 0;
        }
        if(Input.GetKeyDown(KeyCode.A)) {
            transform.localScale = new Vector3(-1, 1, 1);
            //actStateセット
            actState %= 10;
            actState += 10;
        }
        if (Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.A)) {
            transform.localScale = new Vector3(-1, 1, 1);
            //actStateセット
            actState %= 10;
            actState += 10;
        }
        if (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            transform.localScale = new Vector3(1, 1, 1);
            //actStateセット
            actState %= 10;
            actState += 0;
        }

        //レイヤー移動
        if (Input.GetKeyDown(KeyCode.W)) {
            if (worldLoader.CheckToBack(transform.position)) {
                UnityEngine.Debug.Log(worldLoader.CheckToBack(transform.position));
                moveB = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            if (worldLoader.CheckToFront(transform.position)) {
                UnityEngine.Debug.Log(worldLoader.CheckToFront(transform.position));
                moveF = true;
            }
        }


        //チャンクローディング
        loadCnt += Time.deltaTime;
        if(loadCnt > 0.5) {
            worldLoader.LoadChunks(transform.position);
            loadCnt = 0;
        }

        //プレイヤーデータ送信

        syncCnt += Time.deltaTime;
        if(syncCnt > syncTimeLate) {
            if (testUseWrapper) {
                Debug.Log(wrapper.IsConnectionActive());
                if (wrapper.IsConnectionActive())
                {
                    wrapper.SendPlayerMove((ShadowBoxClientWrapper.BlockLayer)inLayer, transform.position.x, transform.position.y, actState);
                }

                //wrapper.SendPlayerMove((ShadowBoxClientWrapper.BlockLayer)inLayer, (float)2.0, (float)2.0);
            }
            else {
                gt.inLayer = inLayer;
                gt.inPos = transform.position;
                gt.actState = actState;
            }
            syncCnt = 0;
        }
    }


    void FixedUpdate() {

        //移動
        if (runL) {
            if(movedir.x < 10) {
                movedir.x += 20 * Time.deltaTime;//0.5秒かけて秒速10/sまで加速
                anim.SetBool("run", true);

            }
            //actStateセット
            actState = actState / 10 * 10;
            actState += 1;

        }
        if (runR) {
            if(movedir.x > -10) {
                movedir.x -= 20 * Time.deltaTime;
                anim.SetBool("run", true); 
                
                
            }
            //actStateセット
            actState = actState / 10 * 10;
            actState += 1;
        }
        if(((!runR && !runL)||(runR && runL)) && controller.isGrounded) {
            anim.SetBool("run", false);
            
            //actStateセット
            actState = actState / 10 * 10;
            actState += 0;
            
            if (Mathf.Abs(movedir.x) < 2) {
                movedir.x = 0;
            }
            else {
                if(movedir.x > 0) {
                    movedir.x -= 40 * Time.deltaTime;
                }
                else {
                    movedir.x += 40 * Time.deltaTime;
                }
            }
        }

        //落下
        if (!controller.isGrounded) {
            fallCnt += Time.deltaTime;
            if(fallCnt > 0.1) {
                anim.SetBool("fall", true);

                //actStateセット
                actState = actState / 10 * 10;
                actState += 3;
            }
            
            if(movedir.y > -9.8) {
                movedir.y -= (float)9.8 * 2 * Time.deltaTime;
            }
        }
        else {
            fallCnt = 0;
            anim.SetBool("fall", false);
            movedir.y = 0;
        }
        //ジャンプ
        if (jump) {
            anim.SetBool("jump", true);

            //actStateセット
            actState = actState / 10 * 10;
            actState += 2;

            movedir.y = 9;
        }
        else {
            anim.SetBool("jump", false);
        }

        //レイヤー移動
        if (moveF) {
            float md = (float)0 - transform.position.z;
            controller.Move(new Vector3(0, 0, md));
            moveF = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "OutsideBlock";
            inLayer = 4;
        }

        if (moveB) {
            float md = (float)0.8 - transform.position.z;
            controller.Move(new Vector3(0, 0, md));
            moveB = false;

            GetComponent<SpriteRenderer>().sortingLayerName = "InsideBlock";
            inLayer = 2;
        }
        //移動反映
        controller.Move(movedir * Time.deltaTime);

    }

}
