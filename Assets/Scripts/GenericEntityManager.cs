using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

public class GenericEntityManager : MonoBehaviour
{
    public Vector3 spawnPos = new Vector3(20,20,0);

    public enum skinName {
        error_man   = 0,
        test_kun    = 1
    }

    public enum ActState {
        standby     = 0,
        run         = 1,
        jump        = 2,
        fall        = 3
    }

    public struct Player {
        public GameObject sprite;
        public Vector3 movement;
        public float moveTime;
        public float interval;
        public Player(GameObject sprite, Vector3 movement, float moveTime, float interval) {
            this.sprite = sprite;
            this.movement = movement;
            this.moveTime= moveTime;
            this.interval = interval;
        }
    };

    public Dictionary<Guid,Player> players;
    
    private Animator anim;

    private bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!started)
        {
            players = new Dictionary<Guid, Player>();
            started = true;
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Dictionary<Guid, Player> keyList = new Dictionary<Guid, Player>();

        foreach(Guid key in players.Keys)
        {
            Guid g = key;
            Player p = players[key];
            keyList.Add(g,p);
        }

        //interval加算

        foreach(Guid k in keyList.Keys)
        {
            float t = players[k].interval;
            t += Time.deltaTime;
            players[k] = new Player(players[k].sprite, players[k].movement, players[k].moveTime, t);
        }
        

        //移動
        foreach (Guid id in players.Keys) {
            Debug.Log((Time.deltaTime / players[id].moveTime));
            players[id].sprite.transform.localPosition = players[id].sprite.transform.localPosition + (players[id].movement * (Time.deltaTime / players[id].moveTime));


        }
    }

    
    /// <summary>
    /// 他プレイヤーを生成します
    /// </summary>
    /// <param name="id">追加するプレイヤーのGuid</param>
    /// <param name="name">登録するプレイヤー名</param>
    /// <param name="skinID">プレイヤーの見た目 （skinNameを参照）</param>
    /// <returns>プレイヤーの生成に成功したか</returns>
    public bool AddPlayer(Guid id, String name, int skinID) {
        if (!started) Start();
        try {

            String sname = Enum.GetName(typeof(skinName), skinID);
            if(Enum.GetName(typeof(skinName), skinID) != null /*&& Instantiate((GameObject)Resources.Load("Characters/" + sname)) != null*/) {
                Debug.Log(transform);
                players.Add(id, new Player(Instantiate((GameObject)Resources.Load("Characters/" + sname), transform.position, transform.rotation),spawnPos, (float)0.1, 0));
                Debug.Log("A");
            }
            else {
                UnityEngine.Debug.LogWarning($"skinID:{skinID}　のキャラクターが見つかりませんでした。エラーマンが出動します");
                players.Add(id, new Player(Instantiate((GameObject)Resources.Load("Characters/error_man"), transform.position, transform.rotation), spawnPos,(float)0.1, 0));
            }
        }catch(Exception e) {
            UnityEngine.Debug.LogError("<<<AddPlayer エラー>>>\n" + e);
            return false;
        }
        return true;
    }

    /// <summary>
    /// プレイヤー情報を更新します
    /// </summary>
    /// <param name="id">更新するプレイヤーのGuid</param>
    /// <param name="x">プレイヤーの位置（ワールド座標）</param>
    /// 
    /// <param name="y">プレイヤーの位置（ワールド座標）</param>
    /// <param name="layer">プレイヤーのレイヤー[1:InsideWall 2:InsideBlock 3:OutsideWall 4:OutsideBlock]</param>
    /// <param name="actState">プレイヤーの状態（actStateを参照）</param>
    /// <returns></returns>
    public bool SyncPlayer(Guid id, float x, float y, int layer, int actState) {
        if (!started) Start();
        //Guid検索
        UnityEngine.Debug.Log(id);
        if (!players.ContainsKey(id)) {
            UnityEngine.Debug.LogWarning($"Guid:{id}　のプレイヤーが見つかりませんでした。");
            return false;
        }
        
        //移動量同期
        Vector3 oldPos =  players[id].sprite.transform.position;
        players[id] = new Player(players[id].sprite, pos - oldPos, players[id].interval, 0.1f);

        //アニメーション同期
        anim = players[id].sprite.GetComponent<Animator>();
        foreach(String stt in Enum.GetNames(typeof(ActState))) {
            anim.SetBool(stt, false);
        }
        String nm;
        int astt = actState % 10;
        if ((nm = Enum.GetName(typeof(ActState), astt)) != null) {
            anim.SetBool(nm, true);
        }
        else {
            UnityEngine.Debug.Log("対象のactStateが見つかりませんでした。");
        }
        if(actState / 10 == 1) {
            players[id].sprite.transform.localScale = new Vector3(-1,1, 1);
        }
        else {
            players[id].sprite.transform.localScale = new Vector3(1, 1, 1);
        }
        return true;


    }
}
