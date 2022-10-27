using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
/// <summary>
/// �Q�[���J�n������
/// </summary>
public class InitialProcess : MonoBehaviour
{
    //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
    /// <summary>
    /// �u���b�NID���X�g
    /// </summary>
    public enum BLOCK_ID : int{
        air             = 0,
        cube_white      = 1,
        cube_red        = 2,
        cube_green      = 3,
        cube_blue       = 4,
        cube_black      = 5
    }
    //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
    /// <summary>
    /// �`�����N�\���́@�������T�C�Y��InitialProcess.chunkSize���Q��
    /// </summary>
    public struct Chunk {
        /// <summary>
        /// �`�����N�f�[�^
        /// </summary>
        int[][] blocks;
    }
    //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

    public bool noNetMode = false;
    public int chunkSize = 25;
    public int chunksNumX = 4;
    public int chunksNumY = 2;
    

    void Awake()
    {
        this.chunkSize = chunkSize;



        if (noNetMode) { GetBlockIDReference(); } else {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �}�b�v��x��̃`�����N����Ԃ�
    /// </summary>
    /// <returns>x��̃`�����N��</returns>
    public int GetChunksNumX()
    {
        return chunksNumX;
    }


    /// <summary>
    /// �}�b�v��y��̃`�����N����Ԃ�
    /// </summary>
    /// <returns>y��̃`�����N��</returns>
    public int GetChunksNumY()
    {
        return chunksNumY;
    }

    /// <summary>
    /// �`�����N1������̃u���b�N�T�C�Y��Ԃ�
    /// </summary>
    /// <returns>�`�����N�̂P�ӂ̒���</returns>
    public int GetChunkSize() {
        return chunkSize;
    }

    /// <summary>
    /// �e�X�g�p
    /// </summary>
    void GetBlockIDReference()
    {/*

        //reference���݊m�F

        if (!Directory.Exists(Application.persistentDataPath + "/reference"))
        {
            Debug.LogWarning("�f�B���N�g��������܂���B�V�K�쐬���܂��B:" + Application.persistentDataPath + " /reference");
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/reference");
            }
            catch (Exception e) { Debug.LogError("�f�B���N�g���쐬���ɖ�肪�������܂����B\n" + e); }
        }
        else { Debug.Log("�f�B���N�g�����݂��m�F�F" + Application.persistentDataPath + " / reference"); }


        //�u���b�Nid���X�g�Q��

        if (!File.Exists(Application.persistentDataPath + "/reference/blockID.dat"))
        {
            Debug.LogWarning("�t�@�C��������܂���B�V�K�쐬���܂��B:" + Application.persistentDataPath + " /reference/blockID.dat");
            try
            {
                File.Create(Application.persistentDataPath + "/reference/blockID.dat");
            }
            catch (Exception e) { Debug.LogError("�t�@�C���쐬�Ɏ��s���܂����B\n" + e); }
        }
        else { Debug.Log("�t�@�C�����݂��m�F�F" + Application.persistentDataPath + " /reference/blockID.dat"); }


        //�u���b�NID���X�g�쐬

        try
        {
            int lineIdx = 1;
            StringReader reader = new StringReader(File.ReadAllText((Application.persistentDataPath + "/reference/blockID.dat")));
            Dictionary<int, String> read = new Dictionary<int, string>();
            while (reader.Peek() != -1)
            {
                String[] str = reader.ReadLine().Split(':');
                try
                {
                    read[int.Parse(str[0])] = str[1];
                    //Debug.Log("ID LOADED / ID:" + int.Parse(str[0]) + " NAME:" + str[1]);
                }
                catch (Exception e)
                {
                    Debug.LogError("�u���b�NID�Q�Ƃ̓ǂݍ��݂Ɏ��s���܂����BLine:" + lineIdx + "\n" + e);
                }
                lineIdx++;
            }


            //�z�񒷂����߂邽��key�̍ő�l���擾

            int keyMax = 0;
            foreach (KeyValuePair<int, string> data in read)
            {
                if (data.Key > keyMax)
                {
                    keyMax = data.Key;
                }
            }


            //�z��ɑ}��

            String listMakeLog = "";

            BLOCK_ID_LIST = new string[keyMax + 1];
            for (int i = 0; i < BLOCK_ID_LIST.Length; i++)
            {
                if (!read.ContainsKey(i))
                {
                    BLOCK_ID_LIST[i] = "not_found";
                    listMakeLog += i + " : not_found\n";
                }
                else
                {
                    BLOCK_ID_LIST[i] = read[i];
                    listMakeLog += i + " : " + BLOCK_ID_LIST[i] + "\n";
                }
            }
            Debug.Log("�u���b�NID�Q�ƃ��X�g�𐶐����܂����B\n" + listMakeLog);

        }
        catch (Exception e) { Debug.LogError(e); }
    */}
}
