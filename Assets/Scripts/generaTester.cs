using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class generaTester : MonoBehaviour
{
    WorldLoader wl;
    LayerManager[] layers;
    public int chunkNumber = 3;
    // Start is called before the first frame update
    void Start()
    {
        GameObject block;
        block = (GameObject)Resources.Load("Blocks/cube_red");
        if (block == null) {
            Debug.LogError("NULL‚¾‚Ÿ‚Ÿ‚Ÿ‚Ÿ‚Ÿ‚Ÿ‚Ÿ‚Ÿ‚Ÿ‚Ÿ");
        }

        //layers[0] = gameObject.AddComponent<LayerManager>();
        wl = GetComponent<WorldLoader>();
        wl.LoadChunks(new Vector2((float)1.0,(float)10.0));
        /*int cNumX = 4;
        int cNumY = 2;
        int cSize = 25;
        int i = 0;
        layers[i].MakeChunk(chunkNumber);
        bool up = false, lo = false, le = false, ri = false;
        if (chunkNumber - cNumX >= 0) { layers[i].MakeChunk(chunkNumber - cNumX); up = true; }
        if (chunkNumber + cNumX < cNumX * cNumY) { layers[i].MakeChunk(chunkNumber + cNumX); lo = true; }
        if ((chunkNumber + 1) / cNumX != 0) { layers[i].MakeChunk(chunkNumber + 1); ri = true; }
        if ((chunkNumber - 1) / cNumX != cNumX - 1) { layers[i].MakeChunk(chunkNumber - 1); le = true; }
        if (up && ri) { layers[i].MakeChunk(chunkNumber - cNumX + 1); }
        if (lo && ri) { layers[i].MakeChunk(chunkNumber + cNumX + 1); }
        if (lo && le) { layers[i].MakeChunk(chunkNumber + cNumX - 1); }
        if (up && le) { layers[i].MakeChunk(chunkNumber - cNumX - 1); }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
