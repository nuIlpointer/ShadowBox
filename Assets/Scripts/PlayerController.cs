using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Networking.UnityWebRequest;

public class PlayerController : MonoBehaviour {
    enum Skins {
        error_man = 0,
        test_kun_1 = 1,
        knit_chan = 2
    }


    public GameObject cameraObj;
    public GameObject wrapperObject;
    public GameObject serverObject;
    public ShadowBoxServer server;
    public ShadowBoxClientWrapper wrapper;
    public WorldLoader worldLoader;
    public CharacterController controller;
    public Animator anim;
    public CreateController creater;
    private string ipAddress = "127.0.0.1";
    private int port = 11781;
    public string playerName = "Player";
    public int skinID;
    private int oldSkinID;
    private bool createServer = false;
    /// <summary>
    /// true時、ゲーム起動時にサーバにワールド再生成リクエストを送ります
    /// </summary>
    public bool wakeUpWithWorldRegenerate = false;

    //マウス系
    private Vector3 pointerPos;
    private Vector3 mouse;
    public float pointerLayer;
    public Material outsideMaterial; 
    public Material insideMaterial;

    private GameObject pointer;
    private GameObject pointerForMousePos;
    private PointerEventData checkUIExist;

    /// <summary>
    /// プレイヤーデータ送信レート
    /// </summary>
    public float syncTimeLate = (float)0.05;
    float syncCnt = 0;


    /// <summary>
    /// 1秒あたりの移動量を記憶
    /// </summary>
    private Vector3 movedir = new Vector3(0, 0, 0);

    private float jumpCnt = 0;
    private float fallCnt = 0;
    private float loadCnt = 0;

    //移動制御等
    private bool runR = false, runL = false, jump = false, moveB = false, moveF = false, 
        underTheWorld = false, atRightBorder, atLeftBorder;
    private int inLayer = 4;
    private Vector3 safePos;

    //プレイヤーstate
    /// <summary>
    /// プレイヤーのアクションによって変動する値です（ 1の位{ 0:Standby 1:run 3:jump 4:fall } 10の位{ 0:右 1:左} ）
    /// </summary>
    public int actState;

    private Vector2Int worldSize;


    bool started = false;

    //test
    public bool testUseWrapper = true;
    public generaTester gt;

    private bool firstUpdate = true;

    private bool actPermittion;

    //Start()前の初期化完了最初のタイミングで実行
    void Awake() {
        ipAddress = TitleData.ipAddress;
        port = TitleData.port;
        playerName = TitleData.playerName;
        createServer = !TitleData.isMultiPlay;
        skinID = TitleData.skinID;
        Debug.Log($"{ipAddress}:{port} singlemode:{createServer} Player {playerName}(skinID:{skinID}) ");
    }

    // Start is called before the first frame update
    void Start() {
        if (!started) {
            //クライアントサーバ系
            wrapper = wrapperObject.GetComponent<ShadowBoxClientWrapper>();
            if (createServer) {
                server = Instantiate(serverObject).GetComponent<ShadowBoxServer>();
                server.CreateInternalServer();
                //ipAddress = "172.16.103.111";
            }

            wrapper.Connect(ipAddress, port);

            //スキンid

            string sid = Enum.GetName(typeof(Skins), skinID);

            anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load($"Characters/Animator/{sid}/{sid}_player");
            if(sid == null) {
                Debug.LogWarning($"[PlayerController] > 指定のスキンIDのスキンは見つかりませんでした。エラーマンが出動します。　ID : {skinID}");
                anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load($"Characters/Animator/error_man/error_man_player");
            }
            oldSkinID = skinID;

            //ポインタ
            pointer = (GameObject)Resources.Load("Generic/Pointer");
            pointerForMousePos = (GameObject)Resources.Load("Generic/PointerBox");
            pointer = Instantiate(pointer);
            pointerForMousePos = Instantiate(pointerForMousePos);
            mouse = Input.mousePosition;
            pointerLayer = 4;

            checkUIExist = new PointerEventData(EventSystem.current);

        }



    }

    // Update is called once per frame
    void Update() {
        if (firstUpdate) {
            firstUpdate = false;
            wrapper.SetPlayerData(playerName, skinID, 0, transform.position.x, transform.position.y, ShadowBoxClientWrapper.BlockLayer.InsideBlock);

            if (wakeUpWithWorldRegenerate) {
                worldLoader.WakeUp();
            }
            
        }

        //ワールドローダに動いていいか聞く

        actPermittion = worldLoader.permitToPlayerAct;

        //スキンID変更時処理
        if (oldSkinID != skinID) {
            string sid = Enum.GetName(typeof(Skins), skinID);
            anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load($"Characters/Animator/{sid}/{sid}_player");
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
        if (Input.GetKey(KeyCode.Space)) {
            jumpCnt += Time.deltaTime;
            if (jumpCnt < 0.4) {
                if (controller.isGrounded) {
                    jump = true;
                }
            } else {
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
        if (Input.GetKeyDown(KeyCode.D)) {
            transform.localScale = new Vector3(1, 1, 1);
            //actStateセット
            actState %= 10;
            actState += 0;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
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
            UnityEngine.Debug.Log(worldLoader.CheckToBack(transform.position));
            if (worldLoader.CheckToBack(transform.position)) {

                moveB = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) {

            UnityEngine.Debug.Log(worldLoader.CheckToFront(transform.position));
            if (worldLoader.CheckToFront(transform.position)) {
                moveF = true;
            }
        }


        //チャンクローディング
        loadCnt += Time.deltaTime;
        if (loadCnt > 0.5) {
            worldLoader.LoadChunks(transform.position);
            loadCnt = 0;
        }

        //プレイヤーデータ送信

        syncCnt += Time.deltaTime;
        if (syncCnt > syncTimeLate) {
            if (testUseWrapper) {
                //Debug.Log(wrapper.IsConnectionActive());
                if (wrapper.IsConnectionActive()) {
                    wrapper.SendPlayerMove((ShadowBoxClientWrapper.BlockLayer)inLayer, transform.position.x, transform.position.y, actState);
                }

                //wrapper.SendPlayerMove((ShadowBoxClientWrapper.BlockLayer)inLayer, (float)2.0, (float)2.0);
            } else {//generaTesterを使った仮テスト用のやーつ
                gt.inLayer = inLayer;
                gt.inPos = transform.position;
                gt.actState = actState;
            }
            syncCnt = 0;
        }


        //建築操作
        //ポインタ

        mouse = Input.mousePosition;
        //Debug.Log("mouse " + mouse);
        mouse.z = -cameraObj.transform.position.z + (1.2f - (((int)pointerLayer - 1) * 0.4f));
        pointerPos = Camera.main.ScreenToWorldPoint(mouse);

        pointerPos = new Vector3((float)Mathf.Floor(pointerPos.x + 0.5f), (float)Mathf.Floor(pointerPos.y + 0.5f), 0);

        Vector3 fpPos = new Vector3(pointerPos.x, pointerPos.y, 1.2f - ((int)pointerLayer - 1) * 0.4f);
        Vector3 fpfmPos = new Vector3(pointerPos.x, pointerPos.y, 0);

        int pSizeX, pSizeY;
        bool isAllLayer;
        creater.PointerSize(out pSizeX, out pSizeY, out isAllLayer);
        fpPos += new Vector3(pSizeX / 2, pSizeY / 2 );
        fpfmPos += new Vector3(pSizeX / 2, pSizeY / 2 );

        pointer.transform.position = fpPos;
        pointerForMousePos.transform.position = fpfmPos;
        pointer.transform.GetChild(0).localScale = new Vector3(pSizeX, pSizeY);
        pointerForMousePos.transform.GetChild(0).localScale = new Vector3(pSizeX, pSizeY, 1.2f);

        pointerLayer -= (Input.GetAxis("Mouse ScrollWheel") * 6);

        if (pointerLayer > 4) pointerLayer = 4;
        else if (pointerLayer < 1) pointerLayer = 1;
        if (pointerLayer < 3 && outsideMaterial.GetFloat("_Alpha") > 0.2) {
            outsideMaterial.SetFloat("_Alpha", outsideMaterial.GetFloat("_Alpha") - (6 * Time.deltaTime));
        }
        if (pointerLayer >= 3 && outsideMaterial.GetFloat("_Alpha") < 1) {
            outsideMaterial.SetFloat("_Alpha", outsideMaterial.GetFloat("_Alpha") + (6 * Time.deltaTime));
        }


        //建築

        if (pointerPos.x >= 0 && pointerPos.y >= 0 && pointerPos.x < worldLoader.GetWorldSizeX() && pointerPos.y < worldLoader.GetWorldSizeY()) {
            if (Input.GetMouseButton(0)) {
                List<RaycastResult> results = new List<RaycastResult>();
                checkUIExist.position = Input.mousePosition;
                EventSystem.current.RaycastAll(checkUIExist, results);
                if(!results.Exists(result => 
                    result.gameObject.name != "DebugMessageBox" 
                    && result.gameObject.name != "pos_image"
                    && result.gameObject.name != "Xpos"
                    && result.gameObject.name != "Ypos"
                    && result.gameObject.name != "Operation_Text"
                    && result.gameObject.name != "Operation_image"
                    && result.gameObject.name != "Operation_Text2"
                    && result.gameObject.name != "Oparation_image2"
                )) {
                    creater.DrawBlock((int)pointerPos.x, (int)pointerPos.y, (int)pointerLayer);
                }
                
            }
                
            if (Input.GetMouseButton(1)) {
                List<RaycastResult> results = new List<RaycastResult>();
                checkUIExist.position = Input.mousePosition;
                EventSystem.current.RaycastAll(checkUIExist, results);
                if (!results.Exists(result =>
                    result.gameObject.name != "DebugMessageBox"
                    && result.gameObject.name != "pos_image"
                    && result.gameObject.name != "Xpos"
                    && result.gameObject.name != "Ypos"
                    && result.gameObject.name != "Operation_Text"
                    && result.gameObject.name != "Operation_image"
                    && result.gameObject.name != "Operation_Text2"
                    && result.gameObject.name != "Oparation_image2"
                )) {
                    creater.DeleteBlock((int)pointerPos.x, (int)pointerPos.y, (int)pointerLayer);
                }
            }
        }


        //ワールド外判定
        safePos = transform.position;
        if (transform.position.y < 0.8) {
            for (int i = 20; i < 100; i++) {
                safePos.y = (float)i;
                if (this.inLayer == 2) {
                    if (worldLoader.CheckToBack(safePos)) break;
                } else {
                    if (worldLoader.CheckToFront(safePos)) break;
                }
            }
            // 上にあげる
            underTheWorld = true;
        }

        
    }


    void FixedUpdate() {

        //移動
        if (runL) {
            if (movedir.x < 10) {
                movedir.x += 20 * Time.deltaTime;//0.5秒かけて秒速10/sまで加速
                anim.SetBool("run", true);

            }
            //actStateセット
            actState = actState / 10 * 10;
            actState += 1;

        }
        if (runR) {
            if (movedir.x > -10) {
                movedir.x -= 20 * Time.deltaTime;
                anim.SetBool("run", true);


            }
            //actStateセット
            actState = actState / 10 * 10;
            actState += 1;
        }
        if (((!runR && !runL) || (runR && runL)) && controller.isGrounded) {
            anim.SetBool("run", false);

            //actStateセット
            actState = actState / 10 * 10;
            actState += 0;

            if (Mathf.Abs(movedir.x) < 2) {
                movedir.x = 0;
            } else {
                if (movedir.x > 0) {
                    movedir.x -= 40 * Time.deltaTime;
                } else {
                    movedir.x += 40 * Time.deltaTime;
                }
            }
        }

        //落下
        if (!controller.isGrounded) {
            fallCnt += Time.deltaTime;
            if (fallCnt > 0.1) {
                anim.SetBool("fall", true);

                //actStateセット
                actState = actState / 10 * 10;
                actState += 3;
            }

            if (movedir.y > -9.8) {
                movedir.y -= (float)9.8 * 2 * Time.deltaTime;
            }
        } else {
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
        } else {
            anim.SetBool("jump", false);
        }

        //レイヤー移動
        if (moveF) {
            float md = (float)0 - transform.position.z;
            controller.Move(new Vector3(0, 0, md));
            moveF = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "OutsideBlock";
            inLayer = 4;
            transform.GetComponent<SpriteRenderer>().material = outsideMaterial;
        }

        if (moveB) {
            float md = (float)0.8 - transform.position.z;
            controller.Move(new Vector3(0, 0, md));
            moveB = false;

            GetComponent<SpriteRenderer>().sortingLayerName = "InsideBlock";
            inLayer = 2;
            transform.GetComponent<SpriteRenderer>().material = insideMaterial;
        }

        //Z軸ズレ補正
        if (inLayer == 2) {
            Vector3 motion = new Vector3(transform.position.x, transform.position.y, 0.8f);
            controller.Move(motion - transform.position);
        } else {
            Vector3 motion = new Vector3(transform.position.x, transform.position.y, 0f);
            controller.Move(motion - transform.position);
        }

        //ワールド外判定
        if (underTheWorld) {
            //controller.Move(safePos - transform.position);
            controller.enabled = false;
            transform.position = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z);
            controller.enabled = true;
            underTheWorld = false;
        }

        if (transform.position.x + movedir.x * Time.deltaTime < 0.5) {
            movedir.x = 0;
            if (transform.position.x < 0.5) {
                controller.Move(new Vector3(5, 0, 0));
            }
        }

        if (transform.position.x + movedir.x * Time.deltaTime >= worldLoader.GetWorldSizeX() - 1.5) {
            movedir.x = 0;
            if (transform.position.x >= worldLoader.GetWorldSizeX() - 1.5) {
                controller.Move(new Vector3(-5, 0, 0));
            }
        }

        if (transform.position.y + movedir.y * Time.deltaTime >= worldLoader.GetWorldSizeY() - 1.5) {
            movedir.y = 0;
            if (transform.position.y >= worldLoader.GetWorldSizeY() - 1.5) {
                controller.Move(new Vector3(0, -5, 0));
            }
        }

        //移動反映
        if(actPermittion)controller.Move(movedir * Time.deltaTime);

    }

    public void OnDestroy() {
        outsideMaterial.SetFloat("_Alpha", 1f);
    }
}
