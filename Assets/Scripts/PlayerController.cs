#define DEBUG
#undef DEBUG

using UnityEngine;
using System;


namespace audio_app
{
    public class PlayerController : MonoBehaviour
    {
        public bool debug = false;
        public float jumpHz = 2000.0f;

        [SerializeField] private ContactFilter2D filter2d;

        AudioSensitivityController sensiController;
        float jumpSensitivity = 1.0f;
        float moveSensitivity = 2.0f;
        GameObject audioSource;
        audioManajor audioManajor;
        Rigidbody2D rigid2D;
        float walkSpeed = 0.07f;
        float jumpForceLow = 20.0f;
        float jumpForceRaise = 30.0f;

        private Animator animator;

        GameObject camera;
        GameObject gage;

        bool isWallTouch = false;
        public bool IsWallTouch { set { this.isWallTouch = value; } }

        // Start is called before the first frame update
        void Start()
        {
            sensiController = GameObject.Find("UI").GetComponent<AudioSensitivityController>();
            audioSource = GameObject.Find("NAudioData");
            audioManajor = audioSource.GetComponent<audioManajor>();
            rigid2D = GetComponent<Rigidbody2D>();

            camera = GameObject.Find("MainCamera");
            gage = GameObject.Find("gageController");

            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            bool isTouch = rigid2D.IsTouching(filter2d);
    #if DEBUG
             if(Input.GetKey(KeyCode.Space) && isTouch)
             {
                rigid2D.AddForce(transform.up * 500); 
                //this.gameObject.transform.Translate(0, 0.5f, 0);
             }
    #else
            if(isTouch)
            {
                // int jumpIndex = (int)(audioManajor.FFTSize * (jumpHz / (float)audioManajor.SamplingRate ));
                // float[] powerSp = audioManajor.PowerSpectre; 
                // float value = 0.0f;
                // for (int i = jumpIndex; i < audioManajor.FFTSize / 2; i++) value += powerSp[i];
                // value *= value * jumpSensitivity;
                // if(value > jumpForceLow)
                if(audioManajor.F0 > 500)
                 {
                    this.rigid2D.AddForce(transform.up * Math.Min(100, jumpForceRaise) * 10); 
                 }
            }
     #endif
     
     #if DEBUG
            if (Input.GetKey(KeyCode.RightArrow))
     #else
            if(audioManajor.PowerSpectre[0] > moveSensitivity)
     #endif
            {
                if(!isWallTouch)
                {
                    this.gameObject.transform.Translate(walkSpeed, 0, 0);
                    camera.transform.Translate(walkSpeed, 0, 0);
                    gage.transform.Translate(walkSpeed, 0, 0);
                }
                animator.SetTrigger("Run");
            }
            else
            {
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
            if(sensiController.Sensi == AudioSensitivityController.Sensitivity.WEAK)
            {
                jumpSensitivity = 300.0f;
                moveSensitivity = 0.01f;
            }
            else if(sensiController.Sensi == AudioSensitivityController.Sensitivity.MEDIUM)
            {
                jumpSensitivity = 3.0f;
                moveSensitivity = 1.0f;
            }
            else if(sensiController.Sensi == AudioSensitivityController.Sensitivity.STRONG)
            {
                jumpSensitivity = 1.0f;
                moveSensitivity = 2.0f;
            }
        }
    }
}

