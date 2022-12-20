using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    /// <summary>
    /// フォローするターゲットを選択します
    /// </summary>
    public GameObject followTarget;
    public bool follow = true;
    public float followingLevel = 3;
    public float cameraDist = 10;
    public float yCorrection = 3;
    public float cameraFOV;
    public WorldLoader wl;
    private Camera cam;
    public float moveLeftLimit, moveRightLimit, moveUpLimit, moveDownLimit;
    

    //計算用フィールド
    private float verticalFOV;

    private Vector3 move = new Vector3(), noCorPos = new Vector3();
    Transform targetTF;
    // Start is called before the first frame update
    void Start()
    {
        targetTF = followTarget.transform;
        noCorPos = transform.position;
        cam = gameObject.GetComponent<Camera>();
        cameraFOV = cam.fieldOfView;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cameraFOV = cam.fieldOfView;

        if (follow) {
            move = targetTF.position - noCorPos;
            noCorPos = move * followingLevel * Time.deltaTime + noCorPos;

            transform.position = new Vector3(noCorPos.x , noCorPos.y + yCorrection , cameraDist);

        }

        //範囲外を映さないぎりぎりの位置を計算
        verticalFOV = cameraFOV * 1.246516f;
        //Debug.Log(verticalFOV);
        moveLeftLimit = Mathf.Tan((verticalFOV / 2) * (Mathf.PI / 180)) * (-cameraDist + 3f);

        moveRightLimit = wl.GetWorldSizeX() - moveLeftLimit;

        moveDownLimit = Mathf.Tan((cameraFOV / 2) * (Mathf.PI / 180)) * (-cameraDist + 1.5f);

        moveUpLimit = wl.GetWorldSizeY() - moveDownLimit;


        //MoveLimitを超えた時の処理

    }
}
