using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Assets;
using System;


public class PlayerController : MonoBehaviour
{
    public bool debug = false;
    public float jumpHz = 2000.0f;

    [SerializeField] private ContactFilter2D filter2d;

    GameObject audioSource;
    audioManajor audioManajor;
    Rigidbody2D rigid2D;
    float walkSpeed = 0.07f;
    //private Sprite stateSprite;
    //private Sprite runSprite;
    //private SpriteRenderer spriteRenderer;

    private Animator animator;

    GameObject camera;
    GameObject gage;

    bool isWallTouch = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.Find("NAudioData");
        audioManajor = audioSource.GetComponent<audioManajor>();
        rigid2D = GetComponent<Rigidbody2D>();
        //stateSprite = Resources.Load<Sprite>("player_state");
        //runSprite = Resources.Load<Sprite>("player_run");
        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        camera = GameObject.Find("MainCamera");
        gage = GameObject.Find("gageController");

        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        bool isTouch = rigid2D.IsTouching(filter2d);
#if true
         if(Input.GetKey(KeyCode.Space) && isTouch)
         {
            rigid2D.AddForce(transform.up * 500); 
            //this.gameObject.transform.Translate(0, 0.5f, 0);
         }
#else
        if(isTouch)
        {
            int jumpIndex = (int)(audioManajor.getFFTSize() * (jumpHz / (float)audioManajor.getSamplingRate() ));
            float tmp = 0.0f;
            for (int i = jumpIndex; i < audioManajor.getFFTSize() / 2; i++) tmp += audioManajor.getPowerSpectre()[i];
            if(tmp > 20.0f)
             {
                this.rigid2D.AddForce(transform.up * Math.Min(tmp, 30.0f) * 10); 
                //this.gameObject.transform.Translate(0, 0.5f, 0);
             }
        }
 #endif
 
 #if true
        if (Input.GetKey(KeyCode.RightArrow))
 #else
        if(audioManajor.getPowerSpectre()[0] > 2.0f)
 #endif
        {
            if(!isWallTouch)
            {
                this.gameObject.transform.Translate(walkSpeed, 0, 0);
                camera.transform.Translate(walkSpeed, 0, 0);
                gage.transform.Translate(walkSpeed, 0, 0);
            }
            animator.SetTrigger("Run");
            //spriteRenderer.sprite = runSprite;
        }
        else
        {
            //spriteRenderer.sprite = stateSprite;
            animator.SetTrigger("State");
        }
        if(debug)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) this.gameObject.transform.Translate(-1.0f * walkSpeed, 0, 0);;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public void setIsWallTouch(bool flg)
    {
        isWallTouch = flg;
    }
}
