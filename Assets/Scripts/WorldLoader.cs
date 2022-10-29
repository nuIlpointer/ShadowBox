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

    private bool started = false;


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
        cNumX = ip.chunksNumX;
        cNumY = ip.chunksNumY;
        cSize = ip.chunkSize;

        started = true;
    }

    // Update is called once per frame
    /*void Update()
    {

    }*/


    /// <summary>
    ///�w��ʒu���ӂ̃`�����N�𐶐�
    /// </summary>
    /// <param name="pos">����W���w��</param>
    public void LoadChunks(Vector2 pos)
    {

        if (!started) { Start(); }

        for(int i = 1; i <= 4; i++) {
            if(layers[i] == null) {
                switch (i) {
                    case 1:
                        layers[1] = transform.Find("LayerInsideWall").GetComponent<LayerManager>();
                        break;
                    case 2:
                        layers[2] = transform.Find("LayerInsideBlock").GetComponent<LayerManager>();
                        break;
                    case 3:
                        layers[3] = transform.Find("LayerOutsideWall").GetComponent<LayerManager>();
                        break;
                    case 4:
                        layers[4] = transform.Find("LayerOutsideBlock").GetComponent<LayerManager>();
                        break;
                }
            }
        }
        Debug.LogWarning(layers[1] + " " + layers[2] + " " + layers[3] + " " + layers[4]);
        int chunkNumber = ((int)pos.x / cSize) + cNumX * (((int)pos.y / cSize));
        Debug.Log("number:"+chunkNumber);
        if(chunkNumber != lastMakePoint) {
            for (int i = 1; i <= 4; i++) {
                layers[i].MakeChunk(chunkNumber);
                Debug.LogWarning(i);
                bool up = false, lo = false, le = false, ri = false;
                if(chunkNumber - cNumX >= 0) { layers[i].MakeChunk(chunkNumber - cNumX); up = true; Debug.Log("ue"); }
                if(chunkNumber + cNumX < cNumX * cNumY) { layers[i].MakeChunk(chunkNumber + cNumX); lo = true; Debug.Log("sita"); }
                if ((chunkNumber - 1) / cNumX != cNumX - 1)  { layers[i].MakeChunk(chunkNumber + 1); ri = true; }
                if ((chunkNumber + 1) / cNumX != 0) { layers[i].MakeChunk(chunkNumber - 1); le = true; }
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
