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


    Dictionary<Guid, GameObject> players;



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
            players.Add(id, Instantiate((GameObject)Resources.Load("Characters/" + Enum.GetName(typeof(skinName), skinID)), transform));
            if (players[id] == null) {
                Debug.LogWarning($"skinID:{skinID}　のキャラクターが見つかりませんでした。エラーマンが出動します");
                players[id] = Instantiate((GameObject)Resources.Load("Characters/error_man"), transform);
            }
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
            Debug.LogWarning($"Guid:{id}　のキャラクターが見つかりませんでした。");
            return false;
        }
        players[id].transform.position = pos;


        return true;


    }
}
