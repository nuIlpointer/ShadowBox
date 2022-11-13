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

    private Vector3 move = new Vector3(), noCorPos = new Vector3();
    Transform targetTF;
    // Start is called before the first frame update
    void Start()
    {
        targetTF = followTarget.transform;
        noCorPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (follow) {
            move = targetTF.position - noCorPos;
            noCorPos = move * followingLevel * Time.deltaTime + noCorPos;

            transform.position = new Vector3(noCorPos.x , noCorPos.y + yCorrection , cameraDist);

        }
    }
}
