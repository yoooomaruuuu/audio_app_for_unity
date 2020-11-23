#define DEBUG
#undef DEBUG

using UnityEngine;
using System;
using System.Linq;


namespace audio_app
{
    public class PlayerController : MonoBehaviour
    {
        public bool debug = true;
        public float jumpHz = 2000.0f;

        [SerializeField] private ContactFilter2D filter2d;

        AudioSensitivityController sensiController;
        float jumpSensitivity = 300.0f;
        float moveSensitivity = 50.0f;
        GameObject audioSource;
        AudioManager AudioManager;
        Rigidbody2D rigid2D;
        float walkSpeed = 0.07f;
        float jumpForceRaise = 30.0f;

        private Animator animator;

        GameObject camera;
        GameObject gage;

        bool isWallTouch = false;
        public bool IsWallTouch { set { this.isWallTouch = value; } }

        void Start()
        {
            sensiController = GameObject.Find("UI").GetComponent<AudioSensitivityController>();
            audioSource = GameObject.Find("AudioManager");
            AudioManager = audioSource.GetComponent<AudioManager>();
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
            if(isTouch && this.rigid2D.velocity.y == 0.0f)
            {
                if(AudioManager.F0 > jumpSensitivity)
                 {
                    this.rigid2D.AddForce(transform.up * Math.Min(AudioManager.PowerSpectre.Max(), jumpForceRaise) * 20); 
                 }
            }
#endif

#if DEBUG
            if (Input.GetKey(KeyCode.RightArrow))
#else
            if(AudioManager.InputDb > moveSensitivity)
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

        void Update()
        {
            if(sensiController.Sensi == AudioSensitivityController.Sensitivity.STRONG)
            {
                moveSensitivity = 12.0f;
            }
            else if(sensiController.Sensi == AudioSensitivityController.Sensitivity.MEDIUM)
            {
                moveSensitivity = 36.0f;
            }
            else if(sensiController.Sensi == AudioSensitivityController.Sensitivity.WEAK)
            {
                moveSensitivity = 48.0f;
            }
        }

        private void OnDestroy()
        {
            sensiController = null;
            AudioManager = null;
        }
    }
}

