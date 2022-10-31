using UnityEngine;

public class Player : MonoBehaviour {

    public Vector3 pos = new Vector3(0, 0, -1);
    public WorldLoader wl;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.W)) {
            pos.y += (float)0.1;
        }
        if (Input.GetKey(KeyCode.S)) {
            pos.y -= (float)0.1;
        }
        if (Input.GetKey(KeyCode.A)) {
            pos.x -= (float)0.1;
        }
        if (Input.GetKey(KeyCode.D)) {
            pos.x += (float)0.1;
        }


        this.gameObject.transform.position = pos;



        if (Input.GetKeyDown(KeyCode.F)) {
            wl.LoadChunks(pos);

        }
    }
}
