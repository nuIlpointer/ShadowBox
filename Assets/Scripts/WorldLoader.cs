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
    public LayerManager[] layers = new LayerManager[5];

    private int cNumX;
    private int cNumY;
    private int cSize;
    public InitialProcess ip;
    public ShadowBoxClientWrapper wrapper;
    int lastMakePoint = -1;

    public bool autoSetCompnents = false;


    // Start is called before the first frame update
    void Start()
    {
        if (autoSetCompnents) {
            wrapper = gameObject.GetComponent<ShadowBoxClientWrapper>();
            ip = gameObject.GetComponent<InitialProcess>();
            layers[1] = transform.Find("LayerInsideWall").GetComponent<LayerManager>();
            layers[2] = transform.Find("LayerInsideBlock").GetComponent<LayerManager>();
            layers[3] = transform.Find("LayerOutsideWall").GetComponent<LayerManager>();
            layers[4] = transform.Find("LayerOutsideBlock").GetComponent<LayerManager>();
            
        }
        
    }

    /// <summary>
    /// InitialProcess����l���擾����
    /// Start()���\�b�h����ɑ��̃��\�b�h��葁�����s�����Ƃ͌���Ȃ���
    /// LoadChunks()���s�O�ɕK���l���擾�����悤�ɂ���
    /// </summary>
    void GetInitializeProcessValues() {
        cNumX = ip.GetChunksNumX();
        cNumY = ip.GetChunksNumY();
        cSize = ip.GetChunkSize();
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    ///�w��ʒu���ӂ̃`�����N�𐶐�
    /// </summary>
    /// <param name="pos">����W���w��</param>
    public void LoadChunks(Vector2 pos)
    {
        GetInitializeProcessValues();
        Debug.Log(cSize +" "+cNumX);

        cNumX = 4;
        cNumY = 2;
        cSize = 25;
        
        int chunkNumber = ((int)pos.x / cSize) + cNumX * (((int)pos.y / cSize));
        Debug.Log("number:"+chunkNumber);
        if(chunkNumber != lastMakePoint) {
            for (int i = 1; i <= 4; i++) {
                layers[i].MakeChunk(chunkNumber);
                bool up = false, lo = false, le = false, ri = false;
                if(chunkNumber - cNumX >= 0) { layers[i].MakeChunk(chunkNumber - cNumX); up = true; Debug.Log("ue"); }
                if(chunkNumber + cNumX < cNumX * cNumY) { layers[i].MakeChunk(chunkNumber + cNumX); lo = true; Debug.Log("sita"); }
                if((chunkNumber + 1) / cNumX != 0) { layers[i].MakeChunk(chunkNumber + 1); ri = true; }
                if((chunkNumber - 1) / cNumX != cNumX - 1) { layers[i].MakeChunk(chunkNumber - 1); le = true; }
                if(up && ri) { layers[i].MakeChunk(chunkNumber - cNumX + 1); }
                if(lo && ri) { layers[i].MakeChunk(chunkNumber + cNumX + 1); }
                if(lo && le) { layers[i].MakeChunk(chunkNumber + cNumX - 1); }
                if(up && le) { layers[i].MakeChunk(chunkNumber - cNumX - 1); }
                lastMakePoint = chunkNumber;
            }
        }

        
    }

    public bool ChunkUpdate(int[][] chunk, int layer, int chunkNumber) {
        
        return true;
    }


}
