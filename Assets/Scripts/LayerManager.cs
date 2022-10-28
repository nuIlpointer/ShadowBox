using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{

    struct Chunk {
        public int[,] blocks;
    }
    Chunk[,] chunks;
    int[,] blocks;
    InitialProcess ip;
    int cNumX, cNumY;
    public bool makeTest = true;

    
    /// <summary>
    /// �\��������
    /// </summary>
    public bool transparency = false;
    
    
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
        /*cNumX = ip.chunksNumX;
        cNumY = ip.chunksNumY;
        chunks = new Chunk[cNumX, cNumY];
        for(int i = 0; i < cNumX; i++) {
            for(int j = 0; j < cNumY; j++) {
                chunks[i, j].blocks = blocks;
            }
        }*/
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �w��`�����N���̃u���b�N�𐶐�[������]
    /// </summary>
    /// <param name="chankNumber">�`�����N�ԍ�</param>
    public void MakeChunk(int chankNumber) {
        if (makeTest) {
            Debug.Log(chankNumber);
        }
    }

    /// <summary>
    /// �w��`�����N���̃u���b�N������[������]
    /// </summary>
    /// <param name="chunkNumber"></param>
    public void RemoveChunk(int chunkNumber) {
        
    }

}   
