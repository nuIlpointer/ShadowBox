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

    int[][] testcase1 = {
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,2,2,2,2,2,2,2,2,2,2,2,2,2,2,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,2,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 },
        new int[] { 5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5 }
        };
    void Start()
    {
        GameObject block;
        block = (GameObject)Resources.Load("Blocks/cube_red");
        if (block == null) {
            Debug.LogError("NULLÇæÇüÇüÇüÇüÇüÇüÇüÇüÇüÇü");
        }

        //layers[0] = gameObject.AddComponent<LayerManager>();
        wl = GetComponent<WorldLoader>();
        wl.LoadChunks(new Vector2((float)30.0,(float)10.0));//chunkNumber 1(ç∂Ç©ÇÁ2î‘ñ⁄â∫Ç©ÇÁÇPî‘ñ⁄)
        wl.LoadChunks(new Vector2((float)55.0, (float)10.0));//chunkNumber 2(ç∂Ç©ÇÁ3î‘ñ⁄â∫Ç©ÇÁÇPî‘ñ⁄)


        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            wl.ChunkUpdate(testcase1, 4, 0);

        }
        if (Input.GetKeyDown(KeyCode.L)) {
            wl.LoadChunks(new Vector2((float)30.0, (float)10.0));//chunkNumber 1(ç∂Ç©ÇÁ2î‘ñ⁄â∫Ç©ÇÁÇPî‘ñ⁄)
        }
    }
}
