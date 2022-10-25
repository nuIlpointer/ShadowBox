using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldLoader : MonoBehaviour


{
    //      �t�@�C����
    private Vector2 loadChunkPos;
    private string[] blockIDList;
    /// <summary>
    /// �e���C���[�̎Q�Ƃ��i�[�@�Y������enum blockLayer�Ƒ������1~4(0�͌���)
    /// </summary>
    LayerManager[] layers = new LayerManager[5];

    private int cNumX;
    private int cNumY;



    // Start is called before the first frame update
    void Start()
    {
        
        InitialProcess ip = gameObject.GetComponent<InitialProcess>();
        layers[1] = transform.Find("LayerInsideWall").GetComponent<LayerManager>();
        layers[2] = transform.Find("LayerInsideBlock").GetComponent<LayerManager>();
        layers[3] = transform.Find("LayerOutsideWall").GetComponent<LayerManager>();
        layers[4] = transform.Find("LayerOutsideBlock").GetComponent<LayerManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    ///�w��ʒu���ӂ̃`�����N�𐶐�
    /// </summary>
    /// <param name="chunkPos">�}�b�v�ʒu���w��</param>
    public void LoadChunks(Vector2 chunkPos)
    {
        
    }

    public bool BlockMake(int x, int y, int bData)
    {
        
        return true;
    }


}
