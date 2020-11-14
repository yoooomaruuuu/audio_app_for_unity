using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Assets;
using System;


public class PlayerController : MonoBehaviour
{
    public bool debug = false;

    [SerializeField] private ContactFilter2D filter2d;

    GameObject audioSource;
    audioManajor audioManajor;
    Rigidbody2D rigid2D;
    bool isJump = false;
    float jumpForce = 300.0f;
    float walkSpeed = 0.05f;
    private Sprite stateSprite;
    private Sprite runSprite;
    private SpriteRenderer spriteRenderer;

    float jumpTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.Find("NAudioData");
        audioManajor = audioSource.GetComponent<audioManajor>();
        this.rigid2D = GetComponent<Rigidbody2D>();
        stateSprite = Resources.Load<Sprite>("player_state");
        runSprite = Resources.Load<Sprite>("player_run");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        bool isTouch = this.rigid2D.IsTouching(filter2d);
        Debug.Log(isTouch);
#if false
         if(Input.GetKey(KeyCode.Space) && isTouch)
         {
            this.rigid2D.AddForce(transform.up * this.jumpForce); 
            //this.gameObject.transform.Translate(0, 0.5f, 0);
         }
#else
        if(isTouch)
        {
            int jumpIndex = (int)(audioManajor.getFFTSize() * (300.0f / (float)audioManajor.getSamplingRate() ));
            float tmp = 0.0f;
            for (int i = jumpIndex; i < audioManajor.getFFTSize() / 2; i++) tmp += audioManajor.getPowerSpectre()[i];
            if(tmp > 20.0f)
             {
                this.rigid2D.AddForce(transform.up * Math.Min(tmp, 30.0f) * 10); 
                //this.gameObject.transform.Translate(0, 0.5f, 0);
             }
        }
 #endif
 
 #if false
         if (Input.GetKey(KeyCode.RightArrow))
 #else
        if(audioManajor.getPowerSpectre()[0] > 3.0f)
 #endif
         {
             this.gameObject.transform.Translate(walkSpeed, 0, 0);
             spriteRenderer.sprite = runSprite;
         }
         else
         {
             spriteRenderer.sprite = stateSprite;
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
}
