using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public ShadowBoxClientWrapper wrapper;
    public WorldLoader worldLoader;
    public CharacterController controller;
    public Animator anim;

    /// <summary>
    /// �v���C���[�f�[�^���M���[�g
    /// </summary>
    public float syncTimeLate = (float)0.2;

    /// <summary>
    /// 1�b������̈ړ��ʂ��L��
    /// </summary>
    private Vector3 movedir = new Vector3(0,0,0);
    
    private float jumpCnt = 0;
    private float fallCnt = 0;
    private float loadCnt = 0;


    //�ړ����䓙
    private bool runR = false, runL = false, jump = false, moveB = false, moveF = false;

    //���C���[�Ǘ�
    /// <summary>
    /// true:�A�E�g�T�C�h�@false:�C���T�C�h
    /// </summary>
    private bool isOutside = true;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //�ړ�
        //(�~�X�ɂ��runL���E�ړ��ArunR�����ړ��ɂȂ��Ă��܂�)
        runR = false;
        runL = false;

        if (Input.GetKey(KeyCode.A)) {
            runR = true;
        }
        if (Input.GetKey(KeyCode.D)) {
            runL = true;
        }

        //�W�����v
        if(Input.GetKey(KeyCode.Space)) {
            jumpCnt += Time.deltaTime;
            if(jumpCnt < 0.4 ) {
                if (controller.isGrounded) {
                    jump = true;
                }
            }
            else {
                jump = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            jump = false;
            jumpCnt = (float)0.5;
        }
        if (controller.isGrounded) {
            jumpCnt = 0;
        }

        //�L�������]
        if(Input.GetKeyDown(KeyCode.D)) {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if(Input.GetKeyDown(KeyCode.A)) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.A)) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            transform.localScale = new Vector3(1, 1, 1);
        }

        //���C���[�ړ�
        if (Input.GetKeyDown(KeyCode.W)) {
            if (worldLoader.CheckToBack(transform.position)) {
                Debug.Log(worldLoader.CheckToBack(transform.position));
                moveB = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            if (worldLoader.CheckToFront(transform.position)) {
                Debug.Log(worldLoader.CheckToFront(transform.position));
                moveF = true;
            }
        }


        //�`�����N���[�f�B���O
        loadCnt += Time.deltaTime;
        if(loadCnt > 0.5) {
            worldLoader.LoadChunks(transform.position);
            loadCnt = 0;
        }





    }


    void FixedUpdate() {

        //�ړ�
        if (runL) {
            if(movedir.x < 10) {
                movedir.x += 20 * Time.deltaTime;//0.5�b�����ĕb��10/s�܂ŉ���
                anim.SetBool("run", true);
            }
            
        }
        if (runR) {
            if(movedir.x > -10) {
                movedir.x -= 20 * Time.deltaTime;
                anim.SetBool("run", true);
            }
        }
        if(((!runR && !runL)||(runR && runL)) && controller.isGrounded) {
            anim.SetBool("run", false);
            if (Mathf.Abs(movedir.x) < 2) {
                movedir.x = 0;
            }
            else {
                if(movedir.x > 0) {
                    movedir.x -= 40 * Time.deltaTime;
                }
                else {
                    movedir.x += 40 * Time.deltaTime;
                }
            }
        }

        //����
        if (!controller.isGrounded) {
            fallCnt += Time.deltaTime;
            if(fallCnt > 0.1) {
                anim.SetBool("fall", true);
            }
            
            if(movedir.y > -9.8) {
                movedir.y -= (float)9.8 * 2 * Time.deltaTime;
            }
        }
        else {
            fallCnt = 0;
            anim.SetBool("fall", false);
            movedir.y = 0;
        }
        //�W�����v
        if (jump) {
            anim.SetBool("jump", true);
            movedir.y = 9;
        }
        else {
            anim.SetBool("jump", false);
        }

        //���C���[�ړ�
        if (moveF) {
            float md = 0 - transform.position.z;
            controller.Move(new Vector3(0, 0, md));
            moveF = false;
        }

        if (moveB) {
            float md = (float)0.8 - transform.position.z;
            controller.Move(new Vector3(0, 0, md));
            moveB = false;
        }
        //�ړ����f
        controller.Move(movedir * Time.deltaTime);

        

    }


}
