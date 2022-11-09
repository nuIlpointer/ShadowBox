using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    private float directionX = 0.0f;
    private bool run = false;
    private bool jump = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator)
        {
            float h = Input.GetAxisRaw("Horizontal");
            run = true;
            if(h > 0)
            {
                directionX = 1;
                transform.localScale = new Vector3(1, 1, 1);
            }else if(h < 0)
            {
                directionX = -1;
                transform.localScale = new Vector3(-1, 1, 1);
            }else
            {
                run = false;
            }
            if (run)
            {
                transform.Translate(new Vector2(directionX, 0) * Time.deltaTime * 0.8f);
            }
            if(Input.GetKeyDown(KeyCode.Space) && !jump){
                jump = true;
                GetComponent<Rigidbody>().AddForce(new Vector2(0, 200));
            }
            animator.SetBool("run", run);
            animator.SetBool("jump", jump);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        jump = false;
    }
}
