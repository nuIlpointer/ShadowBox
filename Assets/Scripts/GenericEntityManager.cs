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
    /// ���v���C���[�𐶐����܂�
    /// </summary>
    /// <param name="id">�ǉ�����v���C���[��Guid</param>
    /// <param name="name">�o�^����v���C���[��</param>
    /// <param name="skinID">�v���C���[�̌����� �iskinName���Q�Ɓj</param>
    /// <returns>�v���C���[�̐����ɐ���������</returns>
    public bool AddPlayer(Guid id, String name, int skinID) {

        try {
            players.Add(id, Instantiate((GameObject)Resources.Load("Characters/" + Enum.GetName(typeof(skinName), skinID)), transform));
            if (players[id] == null) {
                Debug.LogWarning($"skinID:{skinID}�@�̃L�����N�^�[��������܂���ł����B�G���[�}�����o�����܂�");
                players[id] = Instantiate((GameObject)Resources.Load("Characters/error_man"), transform);
            }
        }catch(Exception e) {
            Debug.LogError("<<<AddPlayer �G���[>>>\n" + e);
            return false;
        }
        return true;
    }

    /// <summary>
    /// �v���C���[�����X�V���܂�
    /// </summary>
    /// <param name="id">�X�V����v���C���[��Guid</param>
    /// <param name="pos">�v���C���[�̈ʒu�i���[���h���W�j</param>
    /// <param name="playerState">�v���C���[�̏�ԁiskinState���Q�Ɓj</param>
    /// <returns></returns>
    public bool SyncPlayer(Guid id, Vector3 pos, int playerState) {
        if (!players.ContainsKey(id)) {
            Debug.LogWarning($"Guid:{id}�@�̃L�����N�^�[��������܂���ł����B");
            return false;
        }
        players[id].transform.position = pos;


        return true;


    }
}
