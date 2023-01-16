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
    /// <summary>
    /// カメラのY座標をフォロー対象からどれだけ上にずらすかを設定します（単位 : 画面サイズ）
    /// </summary>
    public float yDifferenceLevel = 0.2f;
    private float yDifference;
    public float camFOV;
    public WorldLoader wl;
    private Camera cam;
    public float moveLeftLimit, moveRightLimit, moveUpLimit, moveDownLimit;

    private float horFOVTan;
    private float verFOVTan;

    public Transform backGround;

    //計算用フィールド
    public float horizontalFOV;
    private Vector2 aspect;
    private float aspectRatio;
    private Vector3 move = new Vector3(), noCorPos = new Vector3();

    Transform targetTF;
    // Start is called before the first frame update
    void Start()
    {
        targetTF = followTarget.transform;
        noCorPos = transform.position;
        cam = gameObject.GetComponent<Camera>();

        calcAspect();
        calcYDiff();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        calcAspect();
        calcYDiff();

        if (follow) {
            move = targetTF.position - noCorPos;
            noCorPos = move * followingLevel * Time.deltaTime + noCorPos;

            transform.position = new Vector3(noCorPos.x , noCorPos.y + yDifference , cameraDist);

        }

        //範囲外を映さないぎりぎりの位置を計算
        moveLeftLimit = (-cameraDist + 1.2f) * horFOVTan;
        moveRightLimit = wl.GetWorldSizeX() - 1 - moveLeftLimit;
        moveDownLimit = (-cameraDist + 1.2f) * verFOVTan;
        moveUpLimit = wl.GetWorldSizeY() - 1 - moveDownLimit;

        moveDownLimit += yDifference;
        moveUpLimit -= yDifference;

        //MoveLimitを超えた時の処理

        Vector3 limitCalcPos = transform.position;
        if (limitCalcPos.x < moveLeftLimit) limitCalcPos.x = moveLeftLimit;
        if (limitCalcPos.x > moveRightLimit) limitCalcPos.x = moveRightLimit;
        if (limitCalcPos.y < moveDownLimit) limitCalcPos.y = moveDownLimit;
        if (limitCalcPos.y > moveUpLimit) limitCalcPos.y = moveUpLimit;

        transform.position = limitCalcPos;

        //背景追従
        int wSizex = wl.GetWorldSizeX();
        int wSizey = wl.GetWorldSizeY();
        int imageSize;


    }

    void calcAspect() {

        camFOV = cam.fieldOfView;
        aspect = new Vector2(Screen.width, Screen.height);
        aspectRatio = aspect.x / aspect.y;
        
        //縦視野のタンジェントにアスペクト比を掛けて横視野のタンジェントを求める
        verFOVTan = Mathf.Tan((camFOV / 2) * Mathf.Deg2Rad);
        horFOVTan = verFOVTan * aspectRatio;

        //横視野のタンジェントを角度に戻す
        horizontalFOV = Mathf.Atan(horFOVTan) * Mathf.Rad2Deg * 2;
    }

    void calcYDiff() {
        yDifference = (cameraDist * verFOVTan) * -yDifferenceLevel;
    }
}
