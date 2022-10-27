using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
/// <summary>
/// ゲーム開始時処理
/// </summary>
public class InitialProcess : MonoBehaviour
{
    //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
    /// <summary>
    /// ブロックIDリスト
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
    /// チャンク構造体　初期化サイズはInitialProcess.chunkSizeを参照
    /// </summary>
    public struct Chunk {
        /// <summary>
        /// チャンクデータ
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
    /// マップのx列のチャンク数を返す
    /// </summary>
    /// <returns>x列のチャンク数</returns>
    public int GetChunksNumX()
    {
        return chunksNumX;
    }


    /// <summary>
    /// マップのy列のチャンク数を返す
    /// </summary>
    /// <returns>y列のチャンク数</returns>
    public int GetChunksNumY()
    {
        return chunksNumY;
    }

    /// <summary>
    /// チャンク1つあたりのブロックサイズを返す
    /// </summary>
    /// <returns>チャンクの１辺の長さ</returns>
    public int GetChunkSize() {
        return chunkSize;
    }

    /// <summary>
    /// テスト用
    /// </summary>
    void GetBlockIDReference()
    {/*

        //reference存在確認

        if (!Directory.Exists(Application.persistentDataPath + "/reference"))
        {
            Debug.LogWarning("ディレクトリがありません。新規作成します。:" + Application.persistentDataPath + " /reference");
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/reference");
            }
            catch (Exception e) { Debug.LogError("ディレクトリ作成中に問題が発生しました。\n" + e); }
        }
        else { Debug.Log("ディレクトリ存在を確認：" + Application.persistentDataPath + " / reference"); }


        //ブロックidリスト参照

        if (!File.Exists(Application.persistentDataPath + "/reference/blockID.dat"))
        {
            Debug.LogWarning("ファイルがありません。新規作成します。:" + Application.persistentDataPath + " /reference/blockID.dat");
            try
            {
                File.Create(Application.persistentDataPath + "/reference/blockID.dat");
            }
            catch (Exception e) { Debug.LogError("ファイル作成に失敗しました。\n" + e); }
        }
        else { Debug.Log("ファイル存在を確認：" + Application.persistentDataPath + " /reference/blockID.dat"); }


        //ブロックIDリスト作成

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
                    Debug.LogError("ブロックID参照の読み込みに失敗しました。Line:" + lineIdx + "\n" + e);
                }
                lineIdx++;
            }


            //配列長を決めるためkeyの最大値を取得

            int keyMax = 0;
            foreach (KeyValuePair<int, string> data in read)
            {
                if (data.Key > keyMax)
                {
                    keyMax = data.Key;
                }
            }


            //配列に挿入

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
            Debug.Log("ブロックID参照リストを生成しました。\n" + listMakeLog);

        }
        catch (Exception e) { Debug.LogError(e); }
    */}
}
