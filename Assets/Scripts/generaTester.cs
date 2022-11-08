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

    int[][] ground = {
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
    };
    void Start()
    {
        wl = GetComponent<WorldLoader>();
        /*GameObject block;
        block = (GameObject)Resources.Load("Blocks/cube_red");
        if (block == null) {
            Debug.LogError("NULL¾");
        }


        //layers[0] = gameObject.AddComponent<LayerManager>();
        
        //wl.LoadChunks(new Vector2((float)30.0,(float)10.0));//chunkNumber 1(¶©ç2ÔÚº©çPÔÚ)
        //wl.LoadChunks(new Vector2((float)55.0, (float)10.0));//chunkNumber 2(¶©ç3ÔÚº©çPÔÚ)
        */
        wl.LoadChunks(new Vector3((float)20.0, (float)20.0, 0));
        for (int i = 0; i < 4; i++) {
            for (int j = 1; j <= 4; j++) {
                wl.ChunkUpdate(ground, j, i);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.K)) {
            wl.ChunkUpdate(testcase1, 4, 0);

        }
        if (Input.GetKeyDown(KeyCode.L)) {
            wl.LoadChunks(new Vector2((float)30.0, (float)10.0));//chunkNumber 1(¶©ç2ÔÚº©çPÔÚ)
        }*/
        

        wl.LoadChunks(new Vector3((float)20.0, (float)20.0, 0));

    }
}
