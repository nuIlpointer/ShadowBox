using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{

    /// <summary>
    /// �u���b�NID���X�g
    /// </summary>
    /// 

    public String layerName;

    public enum BLOCK_ID {
        unknown     = -1,
        air         = 0,
        cube_white  = 1,
        cube_red    = 2,
        cube_green  = 3,
        cube_blue   = 4,
        cube_black  = 5,

        glass_0     = 10
    }


    struct Chunk {
        public int[,] blocks;
    }
    Chunk[] chunks;
    int[,] blocks;
    GameObject block;
    InitialProcess ip;
    int cNumX, cNumY, cSize;
    /// <summary>
    /// �����`�����N�����O�ɋL�^
    /// </summary>
    public bool makeTest = true;
    /// <summary>
    /// �e�X�g�p�u���b�N��}��
    /// </summary>
    public bool insTestCase = true;
    /// <summary>
    /// �\��������
    /// </summary>
    public bool transparency = false;
    private bool started = false;

    

    int[,] testcase1 = new int[,]{
        { 5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5 }
    };

    public GameObject CHUNK_FRAME;
    public GameObject chunkFrame;
    


    // Start is called before the first frame update


    void Start()
    {
        ip = GetComponentInParent<InitialProcess>();
        blocks = new int[ip.chunkSize, ip.chunkSize];
        //blocks������
        for(int i = 0; i < ip.chunkSize; i++) {
            for(int j = 0; j < ip.chunkSize; j++) {
                blocks[i,j] = 0;
            }
        }

        //chunks������
        cNumX = ip.chunksNumX;
        cNumY = ip.chunksNumY;
        cSize = ip.chunkSize;

        chunks = new Chunk[cNumX * cNumY];
        
        for(int i = 0; i < chunks.Length; i++) {
            if (!insTestCase) {
                chunks[i].blocks = blocks;
            }
            else {
                chunks[i].blocks = testcase1;
            }
        }
        //chunks�����������܂�

        CHUNK_FRAME = new GameObject();
        Destroy(CHUNK_FRAME);

        started = true;
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// �w��`�����N���̃u���b�N�𐶐�[�e�X�g��]
    /// </summary>
    /// <param name="chankNumber">�`�����N�ԍ�</param>
    /*public void MakeChunk(int chunkNumber)
    {
        if (!started) { Start(); }


        if (makeTest)
        {
            Debug.Log(layerName + " > �`�����N�𐶐�:" + chunkNumber);
        }

        chunkFrame = Instantiate(chunkFrame);
        chunkFrame.transform.parent = transform;
        chunkFrame.name = "chunk" + chunkNumber


    }*/
    public void MakeChunk(int chunkNumber)
    {
        //�e�X�g�p���O
        if (!started)
        {
            Start();
        }

        if (makeTest)
        {
            //Debug.Log(this.gameObject.name + " > seisei:"+ chunkNumber + " ");
        }
        Debug.LogWarning(name + " > "+chunkNumber + "  " + chunks.Length);
        chunks[chunkNumber].blocks[0, 0] = chunkNumber;//�e�X�g�p



        chunkFrame = Instantiate(CHUNK_FRAME);
        chunkFrame.transform.parent = this.transform;
        chunkFrame.name = "chunk" + chunkNumber;
        Transform frame = chunkFrame.transform;

        Vector2Int posBase = new Vector2Int(chunkNumber % cNumX * cSize, chunkNumber / cNumX * cSize);
        Vector3Int pos = new Vector3Int(0, 0, 0);
        chunkFrame.transform.localPosition = new Vector3(posBase.x, posBase.y, 0);

        if (makeTest)
        {
            Debug.Log(this.gameObject.name + " > seisei:" + chunkNumber + "   " + chunkFrame.name + " pos:" + chunkFrame.transform.position.x + " , " + chunkFrame.transform.position.y);
        }


        foreach (int id in Enum.GetValues(typeof(BLOCK_ID)))
        {
            //Debug.Log(" id:"+Enum.GetName(typeof(BLOCK_ID), id));

            //�u���b�N�v���n�u�擾
            block = (GameObject)Resources.Load("Blocks/" + Enum.GetName(typeof(BLOCK_ID), id));
            if (block == null) { block = (GameObject)Resources.Load("Blocks/unknown"); }

            for (int px = 0; px < cSize; px++)
            {
                for (int py = 0; py < cSize; py++)
                {
                    if (chunks[chunkNumber].blocks[py, px] == id && id != 0)
                    {
                        pos.x = posBase.x + px;
                        pos.y = posBase.y + py;
                        block = Instantiate(block, frame);
                        block.transform.localPosition = pos;
                        block.name = px + "_" + py + "_" + Enum.GetName(typeof(BLOCK_ID), id);
                        //block.transform.SetParent(this.gameObject.transform);
                    }
                }
            }
        }



    }

    /// <summary>
    /// �w��`�����N���̃u���b�N������[������]
    /// </summary>
    /// <param name="chunkNumber"></param>
    public void RemoveChunk(int chunkNumber) {
        
    }

}   
