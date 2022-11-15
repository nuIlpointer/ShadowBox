using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEntityManager : MonoBehaviour
{
    public Vector3 spawnPos;

    public enum skinName {
        error_man   = 0,
        test_kun    = 1
    }

    public enum skinState {
        standby     = 0,
        run         = 1,
        jump        = 2,
        fall        = 3
    }


    public Dictionary<Guid, GameObject> players = new Dictionary<Guid, GameObject>();
    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    /// <summary>
    /// 他プレイヤーを生成します
    /// </summary>
    /// <param name="id">追加するプレイヤーのGuid</param>
    /// <param name="name">登録するプレイヤー名</param>
    /// <param name="skinID">プレイヤーの見た目 （skinNameを参照）</param>
    /// <returns>プレイヤーの生成に成功したか</returns>
    public bool AddPlayer(Guid id, String name, int skinID) {

        try {

            String sname = Enum.GetName(typeof(skinName), skinID);
            if(Enum.GetName(typeof(skinName), skinID) != null && Instantiate((GameObject)Resources.Load("Characters/" + sname)) != null) {
                players.Add(id, Instantiate((GameObject)Resources.Load("Characters/" + sname), transform));
                
            }
            else {
                Debug.LogWarning($"skinID:{skinID}　のキャラクターが見つかりませんでした。エラーマンが出動します");
                players.Add(id, Instantiate((GameObject)Resources.Load("Characters/error_man"), transform));
            }
            players[id].transform.position = spawnPos;
        }catch(Exception e) {
            Debug.LogError("<<<AddPlayer エラー>>>\n" + e);
            return false;
        }
        return true;
    }

    /// <summary>
    /// プレイヤー情報を更新します
    /// </summary>
    /// <param name="id">更新するプレイヤーのGuid</param>
    /// <param name="pos">プレイヤーの位置（ワールド座標）</param>
    /// <param name="playerState">プレイヤーの状態（skinStateを参照）</param>
    /// <returns></returns>
    public bool SyncPlayer(Guid id, Vector3 pos, int playerState) {
        if (!players.ContainsKey(id)) {
            Debug.LogWarning($"Guid:{id}　のプレイヤーが見つかりませんでした。");
            return false;
        }
        players[id].transform.position = pos;
        anim = players[id].GetComponent<Animator>();
        foreach(String stt in Enum.GetNames(typeof(skinState))) {
            anim.SetBool(stt, false);
        }


        anim.SetBool(Enum.GetName(typeof(skinState), playerState), true);
        return true;


    }
}
