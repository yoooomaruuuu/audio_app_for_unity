using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public bool debug = false;

    GameObject audioSource;
    AudioManajor audioManajor;
    Rigidbody2D rigid2D;
    float jumpForce = 320.0f;
    float walkSpeed = 0.05f;
    private Sprite stateSprite;
    private Sprite runSprite;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GameObject.Find("Audio Source");
        //audioManajor = audioSource.GetComponent<AudioManajor>();
        //this.rigid2D = GetComponent<Rigidbody2D>();
        //stateSprite = Resources.Load<Sprite>("player_state");
        //runSprite = Resources.Load<Sprite>("player_run");
        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
// #if true
//         if(Input.GetKeyDown(KeyCode.Space))
// #else
//        if(audioManajor.getSamples()[0] > 0.0f)
// #endif
//         {
//             this.rigid2D.AddForce(transform.up * this.jumpForce); 
//         }
// 
// #if false
//         if (Input.GetKey(KeyCode.RightArrow))
// #else
//        if(audioManajor.getFFTOutReal()[0] > 10.0f)
// #endif
//         {
//             this.gameObject.transform.Translate(walkSpeed, 0, 0);
//             spriteRenderer.sprite = runSprite;
//         }
//         else
//         {
//             spriteRenderer.sprite = stateSprite;
//         }
//         if(debug)
//         {
//             if (Input.GetKey(KeyCode.LeftArrow)) this.gameObject.transform.Translate(-1.0f * walkSpeed, 0, 0);;
//         }
    }
}
