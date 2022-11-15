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
    /// ���v���C���[�𐶐����܂�
    /// </summary>
    /// <param name="id">�ǉ�����v���C���[��Guid</param>
    /// <param name="name">�o�^����v���C���[��</param>
    /// <param name="skinID">�v���C���[�̌����� �iskinName���Q�Ɓj</param>
    /// <returns>�v���C���[�̐����ɐ���������</returns>
    public bool AddPlayer(Guid id, String name, int skinID) {

        try {

            String sname = Enum.GetName(typeof(skinName), skinID);
            if(Enum.GetName(typeof(skinName), skinID) != null && Instantiate((GameObject)Resources.Load("Characters/" + sname)) != null) {
                players.Add(id, Instantiate((GameObject)Resources.Load("Characters/" + sname), transform));
                
            }
            else {
                Debug.LogWarning($"skinID:{skinID}�@�̃L�����N�^�[��������܂���ł����B�G���[�}�����o�����܂�");
                players.Add(id, Instantiate((GameObject)Resources.Load("Characters/error_man"), transform));
            }
            players[id].transform.position = spawnPos;
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
            Debug.LogWarning($"Guid:{id}�@�̃v���C���[��������܂���ł����B");
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
